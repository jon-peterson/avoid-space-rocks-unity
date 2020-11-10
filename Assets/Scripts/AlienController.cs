// Copyright 2020 Ideograph LLC. All rights reserved.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class AlienController : MonoBehaviour
{
    // How fast the ship moves
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private float minFireDelay = 1.0f;
    [SerializeField] private float maxFireDelay = 3.0f;
    
    private Vector3 _destination;
    private Vector3 _secondDestination;
    private Rigidbody2D _rigidbody2D;
    private AudioSource _audioSource;
    private AudioClip _audioAlienMove;
    private LevelController _levelController;

    void Awake() {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    void Start()
    {
        _levelController = Util.GetLevelController();
        // Start randomly on one of the four sides going to the opposite side
        int fourSidedCoinFlip = Random.Range(0, 3);
        switch (fourSidedCoinFlip) {
            case 0:
                transform.position = Util.GetRandomLocationLeftEdge();
                _destination = Util.GetRandomLocationRightEdge();
                _secondDestination = Util.GetRandomLocationRightEdge();
                break;
            case 1:
                transform.position = Util.GetRandomLocationRightEdge();
                _destination = Util.GetRandomLocationLeftEdge();
                _secondDestination = Util.GetRandomLocationLeftEdge();
                break;
            case 2:
                transform.position = Util.GetRandomLocationTopEdge();
                _destination = Util.GetRandomLocationBottomEdge();
                _secondDestination = Util.GetRandomLocationBottomEdge();
                break;
            default:
                transform.position = Util.GetRandomLocationBottomEdge();
                _destination = Util.GetRandomLocationTopEdge();
                _secondDestination = Util.GetRandomLocationTopEdge();
                break;
        }
        // Calculate the velocity that the spaceship needs to hit it
        _rigidbody2D.velocity = (_destination - transform.position) * speed;
        // Turn on the sound
        _audioSource.clip = Resources.Load<AudioClip>("Audio/move_alien");;
        _audioSource.loop = true;
        _audioSource.Play();
        // Start the shooting coroutine
        StartCoroutine(FireAtSpaceship());
        StartCoroutine(SetSecondDestination());
    }
    
    void Update()
    {
        // If this spaceship is near its destination, safe to kill it
        if (Util.IsOutsideWorldspace(transform.position)) {
            _levelController.OnAlienGone(this);
        }
    }

    /**
     * After a pseudo-random delay, fires at the player's spaceship
     */
    private IEnumerator FireAtSpaceship() {
        // Wait a random period of time
        yield return new WaitForSeconds(Random.Range(minFireDelay, maxFireDelay));
        // Fire a bullet at the player, if the spaceship is still around
        GameObject spaceship = GameObject.FindWithTag("Player");
        if (spaceship != null) {
            _levelController.PlaySound("fire_alien");
            BulletController bullet = Instantiate(Resources.Load<BulletController>("Prefabs/Bullet"));
            bullet.InitializeFromAlien(this, spaceship.GetComponent<SpaceshipController>(), 1.0f);
            // Wait for the bullet to finish, then shoot again
            yield return new WaitForSeconds(bullet.BulletLifetime);
            StartCoroutine(FireAtSpaceship());
        }
    }

    /**
     * After a period of time, changes the direction in which it is flying 
     */
    private IEnumerator SetSecondDestination() {
        // Wait a random period of time
        yield return new WaitForSeconds(Random.Range(minFireDelay+1, maxFireDelay+1));
        _rigidbody2D.velocity = (_secondDestination - transform.position) * speed;
    }

    /**
     * Returns a list of four pieces of the alien controller, moving in random directions
     */
    public List<GameObject> GetAlienPieces() {
        List<GameObject> pieces = new List<GameObject>();
        for (int i = 0; i < 8; i++) {
            string part = i % 2 == 1 ? "SpaceshipPartLarge" : "SpaceshipPartSmall";
            GameObject piece = Instantiate(Resources.Load<GameObject>("Prefabs/" + part));
            piece.transform.position = gameObject.transform.position;
            pieces.Add(piece);
        }
        return pieces;
    }
    
    /**
     * If this alien runs into the spaceship, destroy them both
     */
    public void OnTriggerEnter2D(Collider2D other) {
        SpaceshipController spaceship = other.gameObject.GetComponent<SpaceshipController>();
        if (spaceship) {
            _levelController.DestroySpaceship(spaceship);
            _levelController.DestroyAlien(this);
        }
    }
}
