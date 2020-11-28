﻿// Copyright 2020 Ideograph LLC. All rights reserved.
using System;
using System.Runtime.InteropServices;
using System.Threading;
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

    private Text _scoreText;
    private IAmazonDynamoDB _client;
    private AWSCredentials _credentials;
    private String _identityPoolId = "us-east-2:db7bd2f8-f47d-49d4-8adb-8011f1d1ca52";
    private String _gameTable = "avoid-space-rocks-player-dev";
    
    void Start() {
        // Display the current high score
        GameObject canvas = GameObject.Find("Canvas");
        _scoreText = canvas.transform.Find("PlayerScore/FinalScore").gameObject.GetComponent<Text>();
        int score = _gameStatus.Score;
        _scoreText.text = score.ToString("#,##0");
        
        // Prepare for AWS calls
        _credentials = new CognitoAWSCredentials(_identityPoolId, RegionEndpoint.USEast2);

        string playerId = PlayerPrefs.GetString("cognitoId", null);
        if (string.IsNullOrEmpty(playerId)) {
            playerId = GetNewCognitoPlayerId();
            Debug.Log("Cognito assigned identity: " + playerId);
            PlayerPrefs.SetString("cognitoId", playerId);
            PlayerPrefs.Save();
        }
        
        // After five seconds go to the attract mode
        Invoke("ReturnToAttractMode", 5.0f);
    }

    private string GetNewCognitoPlayerId() {
        Debug.Log("Calling Cognito for new Identity client ID");
        var identityClient = new AmazonCognitoIdentityClient(_credentials, RegionEndpoint.USEast2);
        GetIdRequest request = new GetIdRequest {IdentityPoolId = _identityPoolId};
        Task<GetIdResponse> response = identityClient.GetIdAsync(request);
        response.Wait();
        return response.Result.IdentityId; 
    }

    /**
     * Save the score info from the game status object to Dynamo for this player
     */
    private void SavePlayerScore(string playerId) {
        PlayerScore score = new PlayerScore(_gameStatus, playerId, "JEP");
        PersistScoreToCollection(playerId, score);
        PersistScoreToCollection("high-scores", score);
    }

    private void PersistScoreToCollection(string collectionName, PlayerScore score) {
        DynamoDBContext ddbContext = new DynamoDBContext(_client);
        DynamoDBOperationConfig cfg = new DynamoDBOperationConfig()
        {
            OverrideTableName = _gameTable
        };
        // ddbContext.LoadAsync<PlayerScoreCollection>(collectionName, cfg, (loadResult) => {
        //     if (loadResult.Exception != null) {
        //         Debug.LogError("Failed to load player " + collectionName + "score: " + loadResult.Exception);
        //         return;
        //     }
        //     PlayerScoreCollection collection = loadResult.Result;
        //     if (collection == null) {
        //         collection = new PlayerScoreCollection(collectionName);
        //     }
        //     collection.Add(score);
        //     ddbContext.SaveAsync(collection, cfg, (saveResult)=> {
        //         if (saveResult.Exception != null) {
        //             Debug.LogError("Failed to save player " + collectionName + "score: " + saveResult.Exception);
        //         }
        //     });
        // });
    }

    void ReturnToAttractMode() {
        SceneManager.LoadScene("AttractModeScene", LoadSceneMode.Single);
    }
}
