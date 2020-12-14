// Copyright 2020 Ideograph LLC. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private AwsUtil _aws;

    void Start() {
        _aws = new AwsUtil();
        // Create a list of text titles to cycle through
        _textContainers = new List<Transform>();
        GameObject canvas = GameObject.Find("Canvas");
        _textContainers.Add(canvas.transform.Find("Title"));
        _textContainers.Add(canvas.transform.Find("Instructions"));
        _textContainers.Add(canvas.transform.Find("HighScores"));
        _textContainers.Add(canvas.transform.Find("KeyBindings"));
        // Add some rocks for background and start the titles
        StartCoroutine(nameof(CycleThroughTitles));
        Invoke(nameof(AddRocks), 1.0f);
    }

    private void AddRocks() {
        foreach (var i in Enumerable.Range(0, 3)) {
            AddRock("RockBig");
            AddRock("RockMedium");
        }
    }

    private void AddRock(string type) {
        RockController rock = Instantiate(Resources.Load<RockController>("Prefabs/" + type));
        rock.transform.position = WorldSpaceUtil.GetRandomEdgeLocation();
        Color c = rock.GetComponent<SpriteRenderer>().color;
        c.a = 0.3f; // slightly transparent
        rock.GetComponent<SpriteRenderer>().color = c;
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
    private void UpdateHighScores() {
        // First zero out the existing players and scores
        GameObject canvas = GameObject.Find("Canvas");
        Text playersText = canvas.transform.Find("HighScores/Players").gameObject.GetComponent<Text>();
        playersText.text = "";
        Text scoresText = canvas.transform.Find("HighScores/Scores").gameObject.GetComponent<Text>();
        scoresText.text = "";
        // Fetch the high scores from AWS
        PlayerScoreCollection highScores = _aws.GetPlayerScoreCollection("high-scores");
        // Build up the list of players and scores
        string players = "", scores = "";
        foreach(PlayerScore score in highScores.Scores) {
            players += score.Player + "\n";
            scores += score.Score.ToString("#,##0") + "\n";
        }
        playersText.text = players;
        scoresText.text = scores;
    }

    // Start new game on any key
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
            return;
        }
        if (Input.anyKeyDown) {
            SceneManager.LoadScene("PlayfieldScene", LoadSceneMode.Single);
        }
    }
}
