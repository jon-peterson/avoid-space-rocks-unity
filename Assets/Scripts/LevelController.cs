// Copyright 2020 Ideograph LLC. All rights reserved.
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelController : MonoBehaviour {
    
    [SerializeField] private Camera _camera;
    private Vector3 _screenDimensions;
    private int _rocks;
    
    void Start() {
        // Permanently store the dimensions of the screen in world coordinates
        _screenDimensions = _camera.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, 0.0f));
        // Start the spaceship right in the middle
        GameObject spaceship = Instantiate(Resources.Load("Prefabs/Spaceship", typeof(GameObject))) as GameObject;
        spaceship.transform.position = new Vector3(0.0f, 0.0f);
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
            SpawnChildRocks(rock, "Prefabs/RockMedium", 2);
        } else if (rock.Size == Size.Medium) {
            SpawnChildRocks(rock, "Prefabs/RockSmall", 3);
        } else if (rock.Size == Size.Small) {
            SpawnChildRocks(rock, "Prefabs/RockTiny", 3);
        }
        Destroy(rock.gameObject);
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
