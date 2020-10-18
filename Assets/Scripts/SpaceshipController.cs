// Copyright 2020 Ideograph LLC. All rights reserved.

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(WraparoundMovement))] 
public class SpaceshipController : MonoBehaviour {
    // Speed in degrees that the spaceship and rotate at once
    [SerializeField] private float rotateSpeed = 270.0f;

    // Force added when you hit the fuel
    [SerializeField] private float fuelBoost = 10.0f;

    // The maximum speed that you can make your ship
    [SerializeField] private float maxSpeed = 20.0f;
    
    private Rigidbody2D _rigidbody2D;
    private PolygonCollider2D _collider;
    private SpriteRenderer _renderer;
    private Animator _animator;
    private AudioSource _audioSource;
    private AudioClip _audioFuelBurn;
    private LevelController _levelController;
    private bool _fuelBurning;

    void Awake() {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider = GetComponent<PolygonCollider2D>();
        _renderer = GetComponent<SpriteRenderer>();
    }
    
    void Start() {
        _levelController = Util.GetLevelController();
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioFuelBurn = Resources.Load<AudioClip>("Audio/fuel_burn");
        _fuelBurning = false;
    }

    void Update() {
        if (!_renderer.enabled) {
            // We are in hyperspace so don't allow any input
            return;
        }
        
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
        
        // Hyperspace
        if (Input.GetAxisRaw("Vertical") < 0) {
            StartCoroutine(HyperspaceJump());
        }
    }

    /**
     * Do a hyperspace jump: make the spaceship invisible and impossible to hit. Move it somewhere random on the screen.
     * Make it visible and hittable again.
     */
    private IEnumerator HyperspaceJump() {
        EnterHyperspace();
        yield return new WaitForSeconds(2);
        LeaveHyperspace();
    }
    
    private void EnterHyperspace() {
        _levelController.PlaySound("hyperspace");
        transform.position = Util.GetRandomLocation();
        transform.eulerAngles = new Vector3(0f, 0f, Random.Range(0, 360f));
        _rigidbody2D.velocity = Vector2.zero;
        _fuelBurning = false;
        _audioSource.Stop();
        _collider.enabled = false;
        _renderer.enabled = false;
    }

    private void LeaveHyperspace() {
        _collider.enabled = true;        
        _renderer.enabled = true;
    }

    // Blow up if I get hit by a rock
    public void OnTriggerEnter2D(Collider2D other) {
        RockController rock = other.gameObject.GetComponent<RockController>();
        if (rock != null) {
            _levelController.DestroySpaceship(this);
        }
    }
}