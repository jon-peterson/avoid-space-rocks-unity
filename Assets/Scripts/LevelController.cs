// Copyright 2020 Ideograph LLC. All rights reserved.
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YamlDotNet.Serialization;
using Random = UnityEngine.Random;

public class LevelController : MonoBehaviour {
    
    private Canvas _hudCanvas;
    private Text _scoreText;
    private GameObject _centerTextUI;
    private Text _centerText;
    private GameObject _livesUI;
    private Vector3 _screenDimensions;
    private GameConfig _config;
    private int _rocks;
    private int _lives;
    private int _score;
    private int _level;
    private AudioSource _audioSource;

    void Awake() {
        _audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    void Start() {
        _config = LoadGameConfig();
        _lives = _config.StartingLives;
        _score = 0;
        _level = 1;
        _hudCanvas = GameObject.FindGameObjectWithTag("HUDCanvas").GetComponent<Canvas>();
        _scoreText = _hudCanvas.transform.Find("ScoreText").gameObject.GetComponent<Text>();
        _livesUI = _hudCanvas.transform.Find("Lives").gameObject;
        UpdateHUD();
        _centerTextUI = _hudCanvas.transform.Find("CenterText").gameObject;
        _centerTextUI.SetActive(false);
        _centerText = _centerTextUI.GetComponent<Text>(); 
        // Start the spaceship right in the middle
        StartCoroutine(StartLevel());
        StartCoroutine(SpawnSpaceship(5.0f));
    }

    void Update() {
#if UNITY_EDITOR	
        if (Input.GetKeyDown("0")) {
            // Call destroy rock for every rock. Spawns the children and stuff
            RockController[] rocks = GameObject.FindObjectsOfType<RockController>();
            foreach(RockController rock in rocks)
                DestroyRock(rock);
        }
        if (Input.GetKeyDown("1"))
            // Spawn a big alien ship
            StartCoroutine(SpawnAlienBig(0.0f));
#endif        
    }

    /**
     * Sets the score and number of lives in the HUD 
     */
    private void UpdateHUD() {
        _scoreText.text = _score.ToString("#,##0");
        int existingShipIcons = _livesUI.transform.childCount; 
        if (existingShipIcons < _lives) {
            // Add child icons until they match
            for (int i = existingShipIcons; i < Math.Min(_lives, 20); i++) {
                GameObject icon = Instantiate(Resources.Load("Prefabs/LifeIcon", typeof(GameObject)), _livesUI.transform) as GameObject;
                Vector3 ori = icon.transform.position;
                icon.transform.position = new Vector3(ori.x - (20.0f * i), ori.y, ori.z);
            }
        } else if (existingShipIcons > _lives) {
            // Drop children icons until they match
            for (int i = existingShipIcons; i > _lives; i--) {
                Destroy(_livesUI.transform.GetChild(i-1).gameObject);
            }
        }
    }

    /**
     * Plays the specified sound file.
     */
    public void PlaySound(String sound) {
        _audioSource.PlayOneShot(Resources.Load<AudioClip>("Audio/" + sound));
    }
    
    /**
     * Destroy the passed-in rock, spawning new smaller ones as needed. Increases score.
     */
    public void DestroyRock(RockController rock) {
        int pieces = (int)Math.Floor(_level / 4.0f) + 2;
        switch (rock.Size) {
            case Size.Large:
                ScorePoints(_config.Points.LargeRock);
                PlaySound("explosion_large");
                SpawnChildRocks("RockMedium", pieces, rock.transform.position);
                break;
            case Size.Medium:
                ScorePoints(_config.Points.MediumRock);
                PlaySound("explosion_medium");
                if(_level > 1)
                    SpawnChildRocks("RockSmall", pieces, rock.transform.position);
                break;
            case Size.Small:
                ScorePoints(_config.Points.SmallRock);
                PlaySound("explosion_small");
                if(_level > 3)
                    SpawnChildRocks("RockTiny", pieces + 1, rock.transform.position);
                break;
            case Size.Tiny:
            default:
                ScorePoints(_config.Points.TinyRock);
                PlaySound("explosion_small");
                break;
        }
        Destroy(rock.gameObject);
        _rocks--;
        if (_rocks <= 0) {
            _level++;
            StartCoroutine(StartLevel());
        }
    }

    /**
     * Increases the player's score. Gives extra life if appropriate.
     */
    private void ScorePoints(int points) {
        int nextRewardLevel = (int) Math.Floor((double) _score / _config.PointsForNewLife) + 1;
        int pointsForNewLife = (nextRewardLevel * _config.PointsForNewLife);
        _score += points;
        if (_score >= pointsForNewLife) {
            PlaySound("extra_life");
            _lives += 1;
        }
        UpdateHUD();
    }
    
    /**
     * Destroys the passed-in spaceship, decreasing the number of lives. 
     */
    public void DestroySpaceship(SpaceshipController spaceshipController) {
        PlaySound("explosion_ship");
        Destroy(spaceshipController.gameObject);
        // Spawn four pieces of the spaceship flying off in different directions, then they go away
        spaceshipController.GetSpaceshipPieces().ForEach(piece => Destroy(piece, Random.Range(1.5f, 3.0f)));
        _lives--;
        UpdateHUD();
        if (_lives > 0) {
            StartCoroutine(SpawnSpaceship(3.0f));
        } else {
            StartCoroutine(GameOver());
        }
    }

    /**
     * Destroys the passed-in alien, adding to score
     */
    public void DestroyAlien(AlienController alienController) {
        PlaySound("explosion_alien");
        Destroy(alienController.gameObject);
        // Spawn four pieces of the spaceship flying off in different directions, then they go away
        alienController.GetAlienPieces().ForEach(piece => Destroy(piece, Random.Range(1.5f, 3.0f)));
    }

    /**
     * Spawns a number of rock types at a specific position
     */
    private void SpawnChildRocks(String prefab, int count, Vector3 pos) {
        for (int i = 0; i < count; i++) {
            SpawnRock(prefab, pos);
            _rocks++;
        }
    }
    
    /**
     * Creates a new spaceship in the middle of the screen
     */
    private IEnumerator SpawnSpaceship(float delay) {
        yield return new WaitForSeconds(delay);
        SpaceshipController spaceship = Instantiate(Resources.Load<SpaceshipController>("Prefabs/Spaceship"));
        spaceship.transform.position = new Vector3(0f, 0f);
        spaceship.transform.eulerAngles = new Vector3(0f, 0f, 90f);
    }

    private IEnumerator SpawnAlienBig(float delay) {
        yield return new WaitForSeconds(delay);
        AlienController alien = Instantiate(Resources.Load<AlienController>("Prefabs/AlienBig"));
    }

    /**
     * Displays the game over text, then transitions back to attract mode
     */
    private IEnumerator GameOver() {
        yield return new WaitForSeconds(3.0f);
        ShowCenterText("Game Over");
        yield return new WaitForSeconds(3.0f);
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

    private static GameConfig LoadGameConfig() {
        var textFile = Resources.Load<TextAsset>("Config/game-configuration");
        var input = new StringReader(textFile.text);
        var deserializer = new DeserializerBuilder().Build();
        return deserializer.Deserialize<GameConfig>(input);
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
            SpawnRock("RockBig",
                Random.Range(0, 1) == 0 ? Util.GetRandomLocationTopEdge() : Util.GetRandomLocationLeftEdge());
        }
    }

    /**
     * Creates and returns a new rock with the given type (like "RockBig") at the specified location
     */
    private GameObject SpawnRock(String type, Vector2 position) {
        GameObject rock = Instantiate(Resources.Load("Prefabs/" + type, typeof(GameObject))) as GameObject;
        rock.transform.position = position;
        rock.GetComponent<RandomDirection>().SpeedBoost = (_level * 0.1f);
        return rock;
    }

}        
