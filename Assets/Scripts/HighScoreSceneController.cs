// Copyright 2020 Ideograph LLC. All rights reserved.
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Submits the user's score to the high scores database and shows the top list
public class HighScoreSceneController : MonoBehaviour
{
    [SerializeField] private GameStatus _gameStatus = default;
    private AwsUtil _aws;
    
    void Start() {
        // Save the player's score persistently and display it
        _aws = new AwsUtil();
        SavePlayerScore(_aws.GetPlayerId());

        // After seven seconds go to the attract mode
        Invoke(nameof(ReturnToAttractMode), 7.0f);
    }

    /**
     * Save the score info from the game status object to Dynamo for this player
     */
    private void SavePlayerScore(string playerId) {
        string playerName = PlayerPrefs.GetString("name", "???");
        PlayerScore score = new PlayerScore(_gameStatus, playerId, playerName);
        PlayerScoreCollection playerScores = _aws.PersistScoreToCollection(playerId, score);
        PlayerScoreCollection highScores = _aws.PersistScoreToCollection("high-scores", score);
        Debug.Log("Got all tasks back");
        Debug.Log("Best personal score: " + playerScores.Scores[0].Score);
        Debug.Log("Best overall score: " + highScores.Scores[0].Score);
        DisplayScore("PlayerScore/FinalScore", _gameStatus.Score);
        DisplayScore("PlayerBestScore/BestScore", playerScores.Scores[0].Score);
        DisplayScore("AllTimeHighScore/HighScore", highScores.Scores[0].Score);
        DisplayWho("AllTimeHighScore/Who", highScores.Scores[0].Player);
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
