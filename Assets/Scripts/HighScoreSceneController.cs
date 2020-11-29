// Copyright 2020 Ideograph LLC. All rights reserved.
using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Submits the user's score to the high scores database and shows the top list
public class HighScoreSceneController : MonoBehaviour
{
    [SerializeField] private GameStatus _gameStatus = default;

    private IAmazonDynamoDB _dynamoDBClient;
    private AWSCredentials _credentials;
    
    void Start() {
        // Display the current high score
        DisplayScore("PlayerScore/FinalScore", _gameStatus.Score);
        
        // Prepare for AWS calls
        _credentials = new CognitoAWSCredentials(Constants.identityPoolId, RegionEndpoint.USEast2);
        _dynamoDBClient = new AmazonDynamoDBClient(_credentials, RegionEndpoint.USEast2);

        // Save the player's score persistently and display it
        string playerId = GetPlayerId();
        SavePlayerScore(playerId);

        // After seven seconds go to the attract mode
        Invoke("ReturnToAttractMode", 7.0f);
    }

    // Returns the unique Player ID, asking Cognito for it if required. Persists to PlayerPrefs.
    private string GetPlayerId() {
        string playerId = PlayerPrefs.GetString("cognitoId", null);
        if (string.IsNullOrEmpty(playerId)) {
            playerId = GetNewCognitoPlayerId();
            Debug.Log("Cognito assigned identity: " + playerId);
            PlayerPrefs.SetString("cognitoId", playerId);
            PlayerPrefs.Save();
        }
        return playerId;
    }

    // Asks Cognito for a new unique ID for this player
    private string GetNewCognitoPlayerId() {
        Debug.Log("Calling Cognito for new Identity client ID");
        var identityClient = new AmazonCognitoIdentityClient(_credentials, RegionEndpoint.USEast2);
        GetIdRequest request = new GetIdRequest {IdentityPoolId = Constants.identityPoolId};
        Task<GetIdResponse> response = identityClient.GetIdAsync(request);
        response.Wait();
        return response.Result.IdentityId; 
    }

    /**
     * Save the score info from the game status object to Dynamo for this player
     */
    private void SavePlayerScore(string playerId) {
        PlayerScore score = new PlayerScore(_gameStatus, playerId, "JEP");
        PlayerScoreCollection playerScores = PersistScoreToCollection(playerId, score);
        PlayerScoreCollection highScores = PersistScoreToCollection("high-scores", score);
        Debug.Log("Got all tasks back");
        Debug.Log("Best personal score: " + playerScores.Scores[0].Score);
        Debug.Log("Best overall score: " + highScores.Scores[0].Score);
        DisplayScore("PlayerBestScore/BestScore", playerScores.Scores[0].Score);
        DisplayScore("AllTimeHighScore/HighScore", highScores.Scores[0].Score);
        DisplayWho("AllTimeHighScore/Who", highScores.Scores[0].Player);
    }

    private PlayerScoreCollection PersistScoreToCollection(string collectionName, PlayerScore ps) {
        DynamoDBContext ddbContext = new DynamoDBContext(_dynamoDBClient);
        DynamoDBOperationConfig cfg = new DynamoDBOperationConfig() {
            OverrideTableName = Constants.gameTable
        };
        Task<PlayerScoreCollection> task = ddbContext.LoadAsync<PlayerScoreCollection>(collectionName, cfg);
        PlayerScoreCollection scores = task.Result;
        if (scores == null) {
            scores = new PlayerScoreCollection(collectionName);
        }
        if (scores.BelongsInCollection(ps)) {
            scores.Add(ps);
            ddbContext.SaveAsync(scores, cfg);
        }
        return scores;
    }

    private void DisplayScore(string componentName, int score) {
        GameObject canvas = GameObject.Find("Canvas");
        Text scoreText = canvas.transform.Find(componentName).gameObject.GetComponent<Text>();
        scoreText.text = score.ToString("#,##0");
    }

    private void DisplayWho(string componentName, string who) {
        GameObject canvas = GameObject.Find("Canvas");
        Text t = canvas.transform.Find(componentName).gameObject.GetComponent<Text>();
        t.text = who;
    }
    
    void ReturnToAttractMode() {
        SceneManager.LoadScene("AttractModeScene", LoadSceneMode.Single);
    }
}
