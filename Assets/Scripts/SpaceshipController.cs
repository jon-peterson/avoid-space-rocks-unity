// Copyright 2020 Ideograph LLC. All rights reserved.

using System.Collections;
using System.Collections.Generic;
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
    private List<GameObject> _pieces;

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
    
    /**
     * Enter hyperspace: hide the spaceship and replace it with four pieces of it, moving away quickly for one second.
     */
    private void EnterHyperspace() {
        _levelController.PlaySound("hyperspace");
        // Spawn four fast-moving pieces to represent the effect
        _pieces = GetSpaceshipPieces();
        _pieces.ForEach(piece => StartCoroutine(HyperspacePiece(piece)));
        // Move the spaceship somewhere in the world (but not at the very edge), not moving, pointing randomly
        transform.position = Util.GetRandomLocation() * 0.9f;
        transform.eulerAngles = new Vector3(0f, 0f, Random.Range(0, 360f));
        _rigidbody2D.velocity = Vector2.zero;
        // Stop all the various effects
        _fuelBurning = false;
        _audioSource.Stop();
        _collider.enabled = false;
        _renderer.enabled = false;
    }

    /**
     * Control a piece of the spaceship during hyperspace: move it rapidly away for one second then rapidly back
     */
    private IEnumerator HyperspacePiece(GameObject piece) {
        piece.GetComponent<RandomDirection>().SpeedBoost = 8f;
        yield return new WaitForSeconds(1f);
        Rigidbody2D body = piece.GetComponent<Rigidbody2D>();
        body.velocity = gameObject.transform.position - body.transform.position;  
        yield return new WaitForSeconds(1f);
        Destroy(piece);
    }

    /**
     * Leave hyperspace: make the spaceship visible and controllable again
     */
    private void LeaveHyperspace() {
        _pieces.Clear();
        _collider.enabled = true;        
        _renderer.enabled = true;
    }

    /**
     * Returns a list of four pieces of the spaceship, moving in random directions
     */
    public List<GameObject> GetSpaceshipPieces() {
        List<GameObject> pieces = new List<GameObject>();
        for (int i = 0; i < 4; i++) {
            string part = i % 2 == 1 ? "SpaceshipPartLarge" : "SpaceshipPartSmall";
            GameObject piece = Instantiate(Resources.Load("Prefabs/" + part, typeof(GameObject))) as GameObject;
            piece.transform.position = gameObject.transform.position;
            pieces.Add(piece);
        }
        return pieces;
    } 
    
    // Blow up if I get hit by a rock
    public void OnTriggerEnter2D(Collider2D other) {
        RockController rock = other.gameObject.GetComponent<RockController>();
        if (rock != null) {
            _levelController.DestroySpaceship(this);
        }
    }
}