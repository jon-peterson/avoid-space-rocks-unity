﻿// Copyright 2020 Ideograph LLC. All rights reserved.
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(WraparoundMovement))]    
public class BulletController : MonoBehaviour {

    [SerializeField] private float bulletSpeed = 5.0f;
    private Rigidbody2D _rigidbody2D = null;
    private LevelController _levelController = null;
    
    // Start is called before the first frame update
    void Start() {
        Destroy(gameObject, 2.0f);
        _levelController = Util.GetLevelController();
    }

    /**
     * Called when a spaceship fires the bullet: set the velocity based on how the ship is currently moving
     */
    public void InitializeFromSpaceship(SpaceshipController spaceship) {
        // The velocity of the bullet is the same direction as the spaceship points only faster
        _rigidbody2D = GetComponent<Rigidbody2D>(); 
        Vector2 forward = spaceship.transform.TransformDirection(Vector3.right);
        Vector2 boost = (forward.normalized * bulletSpeed);
        _rigidbody2D.velocity = spaceship.GetComponent<Rigidbody2D>().velocity + boost;
        gameObject.transform.position = spaceship.transform.position;
    }

    /**
     * Called when an Alien fires the bullet: point the bullet where the spaceship is now
     */
    public void InitializeFromAlien(AlienController alien) {
        _rigidbody2D = GetComponent<Rigidbody2D>(); 
        Vector2 forward = alien.transform.TransformDirection(Vector3.right);
        Vector2 boost = (forward.normalized * bulletSpeed);
        _rigidbody2D.velocity = alien.GetComponent<Rigidbody2D>().velocity + boost;
        gameObject.transform.position = alien.transform.position;
    }

    public void OnTriggerEnter2D(Collider2D other) {
        RockController rock = other.gameObject.GetComponent<RockController>();
        if (rock != null) {
            _levelController.DestroyRock(rock);
            Destroy(gameObject);
        }
    }
}
