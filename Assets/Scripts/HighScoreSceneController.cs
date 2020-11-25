﻿// Copyright 2020 Ideograph LLC. All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
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
        UnityInitializer.AttachToGameObject(this.gameObject);
        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
        _credentials = new CognitoAWSCredentials(_identityPoolId, RegionEndpoint.USEast2);

        AmazonDynamoDBConfig ddbConfig = new AmazonDynamoDBConfig();
        ddbConfig.RegionEndpoint = RegionEndpoint.USEast2;
        _client = new AmazonDynamoDBClient(_credentials, ddbConfig);

        // Display the current high score
        GameObject canvas = GameObject.Find("Canvas");
        _scoreText = canvas.transform.Find("PlayerScore/FinalScore").gameObject.GetComponent<Text>();
        int score = _gameStatus.Score;
        _scoreText.text = score.ToString("#,##0");
        
        // Save the score that the player just made
        SavePlayerScore();
        
        // After five seconds go to the attract mode
        Invoke("ReturnToAttractMode", 5.0f);
    }

    /**
     * Save the score info from the game status object to Dynamo for this player
     */
    private void SavePlayerScore() {
        PlayerScore score = new PlayerScore(_gameStatus, "1", "JEP");
        DynamoDBContext ddbContext = new DynamoDBContext(_client);
        DynamoDBOperationConfig cfg = new DynamoDBOperationConfig()
        {
            OverrideTableName = _gameTable
        };
        ddbContext.SaveAsync(score, cfg, (result)=> {
            Debug.Log(result.State);
            Debug.Log(result.Exception.Message);
        });
    }

    void ReturnToAttractMode() {
        SceneManager.LoadScene("AttractModeScene", LoadSceneMode.Single);
    }
}
