// Copyright 2020 Ideograph LLC. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AttractModeController : MonoBehaviour {
    private List<Transform> _textContainers;

    void Start() {
        _textContainers = new List<Transform>();
        GameObject canvas = GameObject.Find("Canvas");
        _textContainers.Add(canvas.transform.Find("Title"));
        _textContainers.Add(canvas.transform.Find("Instructions"));
        _textContainers.Add(canvas.transform.Find("HighScores"));
        _textContainers.Add(canvas.transform.Find("KeyBindings"));
        StartCoroutine(CycleThroughTitles());
    }

    private IEnumerator CycleThroughTitles() {
        while (true) {
            UpdateHighScores();
            for (int i = 0; i < _textContainers.Count; i++) {
                _textContainers[i].gameObject.SetActive(true);
                yield return new WaitForSeconds(4.0f);
                _textContainers[i].gameObject.SetActive(false);
                yield return new WaitForSeconds(0.75f);
            }
        }
    }

    // Get the latest high scores from Dynamo and display them in the high scores fields
    void UpdateHighScores() {
        // First zero out the existing players and scores
        GameObject canvas = GameObject.Find("Canvas");
        Text playersText = canvas.transform.Find("HighScores/Players").gameObject.GetComponent<Text>();
        playersText.text = "";
        Text scoresText = canvas.transform.Find("HighScores/Scores").gameObject.GetComponent<Text>();
        scoresText.text = "";
        // Fetch the high scores from AWS
        CognitoAWSCredentials credentials = new CognitoAWSCredentials(Constants.identityPoolId, RegionEndpoint.USEast2);
        AmazonDynamoDBClient client = new AmazonDynamoDBClient(credentials, RegionEndpoint.USEast2);
        DynamoDBContext ddbContext = new DynamoDBContext(client);
        DynamoDBOperationConfig cfg = new DynamoDBOperationConfig() {
            OverrideTableName = Constants.gameTable
        };
        PlayerScoreCollection highScores = ddbContext.LoadAsync<PlayerScoreCollection>("high-scores", cfg).Result;
        // Build up the list of players and scores
        string players = "", scores = "";
        foreach(PlayerScore score in highScores.Scores) {
            players += score.Player + "\n";
            scores += score.Score + "\n";
        }
        playersText.text = players;
        scoresText.text = scores;
    }

    // Start new game on any key
    void Update() {
        if (Input.anyKeyDown) {
            SceneManager.LoadScene("PlayfieldScene", LoadSceneMode.Single);
        }
    }
}
