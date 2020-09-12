// Copyright 2020 Ideograph LLC. All rights reserved.

using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelController : MonoBehaviour {
    
    [SerializeField] private Camera _camera;
    private Vector3 _screenDimensions;
    
    void Start() {
        // Permanently store the dimensions of the screen in world coordinates
        _screenDimensions = _camera.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, 0.0f));
        // Start the spaceship right in the middle
        GameObject spaceship = Instantiate(Resources.Load("Prefabs/Spaceship", typeof(GameObject))) as GameObject;
        spaceship.transform.position = new Vector3(0.0f, 0.0f);
        // Create a bunch of large rocks to start the level
        for (int i = 0; i < 5; i++) {
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
    
    
}        
