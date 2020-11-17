// Copyright 2020 Ideograph LLC. All rights reserved.

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Submits the user's score to the high scores database and shows the top list
public class HighScoreSceneController : MonoBehaviour
{
    [SerializeField] private GameStatus _gameStatus = default;

    private Text _scoreText;
    
    void Start()
    {
        GameObject canvas = GameObject.Find("Canvas");
        _scoreText = canvas.transform.Find("PlayerScore/FinalScore").gameObject.GetComponent<Text>();
        _scoreText.text = _gameStatus.Score.ToString("#,##0");
        Invoke("ReturnToAttractMode", 5.0f);
    }

    void ReturnToAttractMode() {
        SceneManager.LoadScene("AttractModeScene", LoadSceneMode.Single);
    }
}
