// Copyright 2020 Ideograph LLC. All rights reserved.
using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;

public class PlayerScore : IComparable<PlayerScore> {
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

    // Sort them reverse order (highest first)
    public int CompareTo(PlayerScore other) => other == null ? 1 : other.Score.CompareTo(Score);
    
    // This constructor required for DynamoDb persistence layer
    public PlayerScore() {}
}

public class PlayerScoreCollection {
    
    [DynamoDBHashKey("UserId")] public string UserId { get; set; }
    [DynamoDBProperty] public List<PlayerScore> Scores { get; set; }
    private int _max;
    
    public PlayerScoreCollection(String uid) {
        UserId = uid;
        Scores = new List<PlayerScore>();
        _max = 10;
    }

    public PlayerScoreCollection() : this("high-scores") { }

    // A score belongs in the collection if the collection isn't full or if it beats some existing score
    public bool BelongsInCollection(PlayerScore candidate) {
        if (Scores.Count < _max) {
            return true;
        }
        return Scores.Exists(existing => candidate.Score > existing.Score);
    }

    public void Add(PlayerScore score) {
        Scores.Add(score);
        Scores.Sort();
        if (Scores.Count > _max) {
            Scores.RemoveAt(_max);
        }
    }
}