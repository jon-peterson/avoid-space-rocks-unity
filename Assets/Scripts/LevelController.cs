// Copyright 2020 Ideograph LLC. All rights reserved.
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LevelController : MonoBehaviour {
    
    private Canvas _hudCanvas;
    private Text _scoreText;
    private GameObject _centerTextUI;
    private Text _centerText;
    private GameObject _livesUI;
    private Vector3 _screenDimensions;
    private int _rocks;
    private int _lives;
    private int _score;
    private int _level;
    private AudioSource _audioSource;

    void Awake() {
        _audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    void Start() {
        _score = 0;
        _lives = 3;
        _level = 1;
        // Permanently store the dimensions of the screen in world coordinates
        Camera mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        _screenDimensions = mainCamera.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, 0.0f));
        _hudCanvas = GameObject.FindGameObjectWithTag("HUDCanvas").GetComponent<Canvas>();
        _scoreText = _hudCanvas.transform.Find("ScoreText").gameObject.GetComponent<Text>();
        _scoreText.text = _score.ToString();
        _livesUI = _hudCanvas.transform.Find("Lives").gameObject;
        setLivesUI();
        _centerTextUI = _hudCanvas.transform.Find("CenterText").gameObject;
        _centerTextUI.SetActive(false);
        _centerText = _centerTextUI.GetComponent<Text>(); 
        // Start the spaceship right in the middle
        StartCoroutine(StartLevel());
        StartCoroutine(SpawnSpaceship(5.0f));
    }

    /**
     * Adds a spaceship icon for each of the current lives 
     */
    private void setLivesUI() {
        int existing = _livesUI.transform.childCount; 
        if (existing < _lives) {
            // Add child icons until they match
            for (int i = existing; i < Math.Min(_lives, 20); i++) {
                GameObject icon = Instantiate(Resources.Load("Prefabs/LifeIcon", typeof(GameObject)), _livesUI.transform) as GameObject;
                Vector3 ori = icon.transform.position;
                icon.transform.position = new Vector3(ori.x - (20.0f * i), ori.y, ori.z);
            }
        } else if (existing > _lives) {
            // Drop children icons until they match
            for (int i = existing; i > _lives; i--) {
                Destroy(_livesUI.transform.GetChild(i-1).gameObject);
            }
        }
    }

    /**
     * Destroy the passed-in rock, spawning new smaller ones as needed. Increases score.
     */
    public void DestroyRock(RockController rock) {
        
        int pieces = (int)Math.Floor(_level / 4.0f) + 2;
        switch (rock.Size) {
            case Size.Large:
                _score += 5;
                PlaySound("explosion_large");
                SpawnChildRocks("RockMedium", pieces, rock.transform.position);
                break;
            case Size.Medium:
                _score += 10;
                PlaySound("explosion_medium");
                SpawnChildRocks("RockSmall", pieces, rock.transform.position);
                break;
            case Size.Small:
                _score += 20;
                PlaySound("explosion_small");
                SpawnChildRocks("RockTiny", pieces + 1, rock.transform.position);
                break;
            case Size.Tiny:
            default:
                _score += 30;
                PlaySound("explosion_small");
                break;
        }
        Destroy(rock.gameObject);
        _scoreText.text = _score.ToString("#,##0");
        _rocks--;
        if (_rocks <= 0) {
            _level++;
            StartCoroutine(StartLevel());
        }
    }

    /**
     * Plays the specified sound file.
     */
    public void PlaySound(String sound) {
        _audioSource.PlayOneShot(Resources.Load<AudioClip>("Audio/" + sound));
    }
    
    /**
     * Destroys the passed-in spaceship, decreasing the number of lives. 
     */
    public void DestroySpaceship(SpaceshipController spaceshipController) {
        PlaySound("explosion_ship");
        Destroy(spaceshipController.gameObject);
        // Spawn four pieces of the spaceship flying off in different directions
        for (int i = 0; i < 4; i++) {
            String part = i % 2 == 1 ? "SpaceshipPartLarge" : "SpaceshipPartSmall";
            GameObject piece = Instantiate(Resources.Load("Prefabs/" + part, typeof(GameObject))) as GameObject;
            piece.transform.position = spaceshipController.transform.position;
            Destroy(piece, Random.Range(1.5f, 4.0f));
        }
        _lives--;
        setLivesUI();
        if (_lives > 0) {
            StartCoroutine(SpawnSpaceship(3.0f));
        } else {
            StartCoroutine(GameOver());
        }
    }

    /**
     * Spawns a number of rock types at a specific position
     */
    private void SpawnChildRocks(String prefab, int count, Vector3 pos) {
        for (int i = 0; i < count; i++) {
            GameObject childRock = SpawnRock(prefab);
            childRock.transform.position = pos;
            _rocks++;
        }
    }
    
    /**
     * Creates a new spaceship in the middle of the screen
     */
    private IEnumerator SpawnSpaceship(float delay) {
        yield return new WaitForSeconds(delay);
        GameObject spaceship = Instantiate(Resources.Load("Prefabs/Spaceship", typeof(GameObject))) as GameObject;
        spaceship.transform.position = new Vector3(0.0f, 0.0f);
    }

    /**
     * Displays the game over text, then transitions back to attract mode
     */
    private IEnumerator GameOver() {
        yield return new WaitForSeconds(3.0f);
        ShowCenterText("Game Over");
        yield return new WaitForSeconds(4.0f);
        HideCenterText();
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("AttractModeScene", LoadSceneMode.Single);
    }

    private void ShowCenterText(String c) {
        _centerText.text = c;
        _centerTextUI.SetActive(true);
    }

    private void HideCenterText() {
        _centerText.text = "";
        _centerTextUI.SetActive(false);
    }

    /**
     * Start new level L by spawning the correct space rocks
     */
    private IEnumerator StartLevel() {
        yield return new WaitForSeconds(1.0f);
        ShowCenterText("Level " + _level);
        yield return new WaitForSeconds(3.0f);
        HideCenterText();
        yield return new WaitForSeconds(1.0f);
        _rocks = (int)Math.Floor(_level / 2.0f) + 2;
        for (int i = 0; i < _rocks; i++) {
            GameObject rock = SpawnRock("RockBig");
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

    /**
     * Creates and returns a new rock with the given type (like "RockBig")
     */
    private GameObject SpawnRock(String type) {
        GameObject rock = Instantiate(Resources.Load("Prefabs/" + type, typeof(GameObject))) as GameObject;
        rock.GetComponent<RandomDirection>().SpeedBoost = (_level * 0.1f);
        return rock;
    }
}        
