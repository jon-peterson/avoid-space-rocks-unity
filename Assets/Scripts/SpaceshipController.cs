// Copyright 2020 Ideograph LLC. All rights reserved.

using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(WraparoundMovement))] 
public class SpaceshipController : MonoBehaviour {
    // Speed in degrees that the spaceship and rotate at once
    [SerializeField] private float rotateSpeed = 270.0f;

    // Force added when you hit the fuel
    [SerializeField] private float fuelBoost = 10.0f;

    // The maximum speed that you can make your ship
    [SerializeField] private float maxSpeed = 20.0f;
    
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private AudioSource _audioSource;
    private AudioClip _audioFuelBurn;
    private LevelController _levelController;
    private bool _fuelBurning;

    void Start() {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _levelController = Util.GetLevelController();
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioFuelBurn = Resources.Load<AudioClip>("Audio/fuel_burn");
        _fuelBurning = false;
    }

    void Update() {
        // Rotate around the Z axis
        float rot = -Input.GetAxisRaw("Horizontal");
        transform.Rotate(0, 0, rot * rotateSpeed * Time.deltaTime);

        // Fuel burn
        if (Input.GetAxisRaw("Vertical") > 0) {
            Vector2 forward = transform.TransformDirection(Vector3.right);
            Vector2 velocity = _rigidbody2D.velocity + (forward.normalized * (fuelBoost * Time.deltaTime));
            _rigidbody2D.velocity = Vector2.ClampMagnitude(velocity, maxSpeed);
            if (!_fuelBurning) {
                // Just started burning fuel so set up the audio
                _fuelBurning = true;
                _animator.SetBool("fuelBurn", true);
                _audioSource.clip = _audioFuelBurn;
                _audioSource.loop = true;
                _audioSource.Play();
            }
        } else {
            if (_fuelBurning) {
                _fuelBurning = false;
                _animator.SetBool("fuelBurn", false);
                _audioSource.Stop();
            }
        }

        // Fire
        if (Input.GetKeyDown("space")) {
            _levelController.PlaySound("fire");
            BulletController bullet = Instantiate(Resources.Load("Prefabs/Bullet", typeof(BulletController))) as BulletController;
            bullet.Initialize(this);
        }
    }
    
    // Blow up if I get hit by a rock
    public void OnTriggerEnter2D(Collider2D other) {
        RockController rock = other.gameObject.GetComponent<RockController>();
        if (rock != null) {
            _levelController.DestroySpaceship(this);
        }
    }
}