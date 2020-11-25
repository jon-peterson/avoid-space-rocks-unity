// Copyright 2020 Ideograph LLC. All rights reserved.
using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;

public class PlayerScore {
    [DynamoDBHashKey("UserId")] public string UserId { get; set; }
    [DynamoDBProperty] public string Player { get; set; }
    [DynamoDBProperty] public int Score { get; set; }
    [DynamoDBProperty] public int Level { get; set; }
    [DynamoDBProperty] public String Date { get; set; }

    public PlayerScore(GameStatus gameStatus, string userId, string playerName) {
        UserId = userId;
        Player = playerName;
        Score = gameStatus.Score;
        Level = gameStatus.Level;
        Date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
    }
    
    // public PlayerScore(Dictionary<String, AttributeValue> att) {
    //     UserId = att["user-id"].S;
    //     Player = att["player"].S;
    //     Date = att["date"].S;
    //     Score = Int32.Parse(att["score"].N);
    //     Level = Int32.Parse(att["level"].N);
    // }

    // This constructor required for DynamoDb persistence layer
    public PlayerScore() {}
}