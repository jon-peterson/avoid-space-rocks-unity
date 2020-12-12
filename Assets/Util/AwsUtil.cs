// Copyright 2020 Ideograph LLC. All rights reserved.

using System.Net.Sockets;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using UnityEngine;

public class AwsUtil {
    private IAmazonDynamoDB _dynamoDBClient;
    private AWSCredentials _credentials;

    // Returns the Cognito AWS credentials for this player, required to make other AWS calls
    private AWSCredentials GetAwsCredentials() {
        if (_credentials == null) {
            _credentials = new CognitoAWSCredentials(Constants.identityPoolId, RegionEndpoint.USEast2);
        }
        return _credentials;
    }

    // Returns a context for making DynamoDB calls
    private DynamoDBContext GetDynamoDBContext() {
        if (_dynamoDBClient == null) {
            _dynamoDBClient = new AmazonDynamoDBClient(GetAwsCredentials(), RegionEndpoint.USEast2);
        }
        return new DynamoDBContext(_dynamoDBClient);        
    } 

    // Returns the DynamoDB configuration: we want to fetch from a runtime table    
    private DynamoDBOperationConfig GetDyanmoDBOperationConfig() {
        DynamoDBOperationConfig cfg = new DynamoDBOperationConfig() {
            OverrideTableName = Constants.gameTable
        };
        return cfg;
    }

    // Asks Cognito for a new unique ID for this player
    private string GetNewCognitoPlayerId() {
        Debug.Log("Calling Cognito for new Identity client ID");
        var identityClient = new AmazonCognitoIdentityClient(GetAwsCredentials(), RegionEndpoint.USEast2);
        GetIdRequest request = new GetIdRequest {IdentityPoolId = Constants.identityPoolId};
        Task<GetIdResponse> response = identityClient.GetIdAsync(request);
        response.Wait();
        return response.Result.IdentityId; 
    }

    // Returns the unique Player ID, asking Cognito for it if required. Persists to PlayerPrefs.
    public string GetPlayerId() {
        string playerId = PlayerPrefs.GetString("cognitoId", null);
        if (string.IsNullOrEmpty(playerId)) {
            playerId = GetNewCognitoPlayerId();
            Debug.Log("Cognito assigned identity: " + playerId);
            PlayerPrefs.SetString("cognitoId", playerId);
            PlayerPrefs.Save();
        }
        return playerId;
    }
    
    // Returns the high scores for a specific collection, synchronously. The collection returned may be
    // empty but it will never be null.
    public PlayerScoreCollection GetPlayerScoreCollection(string collectionName) {
        Task<PlayerScoreCollection> task = GetDynamoDBContext().LoadAsync<PlayerScoreCollection>(collectionName, 
            GetDyanmoDBOperationConfig());
        PlayerScoreCollection scores = task.Result;
        return scores ?? new PlayerScoreCollection(collectionName);
    }
    
    // Given a collection name and a player score, adds that player score to the collection fetched from Dynamo.
    // If the score is one of the top scores, that collection is written back to Dynamo before being returned.
    public PlayerScoreCollection PersistScoreToCollection(string collectionName, PlayerScore ps) {
        PlayerScoreCollection scores = GetPlayerScoreCollection(collectionName);
        if (scores.BelongsInCollection(ps)) {
            scores.Add(ps);
            GetDynamoDBContext().SaveAsync(scores, GetDyanmoDBOperationConfig());
        }
        return scores;
    }
    
}
