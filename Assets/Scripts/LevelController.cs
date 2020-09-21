// Copyright 2020 Ideograph LLC. All rights reserved.
using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LevelController : MonoBehaviour {
    
    private Canvas _hudCanvas;
    private Text _scoreText;
    private Vector3 _screenDimensions;
    private int _rocks;
    private int _lives;
    private int _score;
    
    void Start() {
        // Permanently store the dimensions of the screen in world coordinates
        Camera mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        _screenDimensions = mainCamera.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, 0.0f));
        _hudCanvas = GameObject.FindGameObjectWithTag("HUDCanvas").GetComponent<Canvas>();
        _scoreText = _hudCanvas.transform.Find("ScoreText").gameObject.GetComponent<Text>();
        _scoreText.text = "0";
        // Start the spaceship right in the middle
        GameObject spaceship = Instantiate(Resources.Load("Prefabs/Spaceship", typeof(GameObject))) as GameObject;
        spaceship.transform.position = new Vector3(0.0f, 0.0f);
        _score = 0;
        _lives = 3;
        // Create a bunch of large rocks to start the level
        _rocks = 3;
        for (int i = 0; i < _rocks; i++) {
            GameObject rock = Instantiate(Resources.Load("Prefabs/RockBig", typeof(GameObject))) as GameObject;
            if (Random.Range(0, 1) == 0) {
                // Along the right side
                rock.transform.position = new Vector3(_screenDimensions.x,
                    Random.Range(_screenDimensions.y, _screenDimensions.y));
            }
            else {
                // Along the bottom             
                rock.transform.position = new Vector3(Random.Range(_screenDimensions.x, _screenDimensions.x),
                    _screenDimensions.y);
            }
        }
    }

    public void DestroyRock(RockController rock) {
        if (rock.Size == Size.Large) {
            _score += 5;
            SpawnChildRocks(rock, "Prefabs/RockMedium", 2);
        } else if (rock.Size == Size.Medium) {
            _score += 10;
            SpawnChildRocks(rock, "Prefabs/RockSmall", 3);
        } else if (rock.Size == Size.Small) {
            _score += 20;
            SpawnChildRocks(rock, "Prefabs/RockTiny", 3);
        }
        Destroy(rock.gameObject);
        _scoreText.text = _score.ToString("#,##0");
        _rocks--;
    }

    private void SpawnChildRocks(RockController rock, String prefab, int count) {
        for (int i = 0; i < count; i++) {
            GameObject kid = Instantiate(Resources.Load(prefab, typeof(GameObject))) as GameObject;
            kid.transform.position = rock.transform.position;
            _rocks++;
        }
    }
}        
