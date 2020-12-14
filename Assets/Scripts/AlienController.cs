// Copyright 2020 Ideograph LLC. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum AlienSize {
    Big, Small
}

// Component that controls an Alien spacecraft in the playfield
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class AlienController : MonoBehaviour
{
    [SerializeField] private float _speed = default;
    [SerializeField] private float _minFireDelay = default;
    [SerializeField] private float _maxFireDelay = default;
    [SerializeField] private float _bulletDrift = default;
    [SerializeField] private AlienSize _size = default;
    
    private Vector3 _destination;
    private Vector3 _secondDestination;
    private Rigidbody2D _rigidbody2D;
    private AudioSource _audioSource;
    private AudioClip _audioAlienMove;
    private LevelController _levelController;

    public AlienSize Size => _size;
    
    void Awake() {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start() {
        _levelController = WorldSpaceUtil.GetLevelController();
        // Start randomly on one of the four sides going to the opposite side
        int fourSidedCoinFlip = Random.Range(0, 4);
        switch (fourSidedCoinFlip) {
            case 0:
                transform.position = WorldSpaceUtil.GetRandomLocationLeftEdge();
                _destination = WorldSpaceUtil.GetRandomLocationRightEdge();
                _secondDestination = _size == AlienSize.Small
                    ? WorldSpaceUtil.GetRandomLocation()
                    : WorldSpaceUtil.GetRandomLocationRightEdge();
                break;
            case 1:
                transform.position = WorldSpaceUtil.GetRandomLocationRightEdge();
                _destination = WorldSpaceUtil.GetRandomLocationLeftEdge();
                _secondDestination = _size == AlienSize.Small
                    ? WorldSpaceUtil.GetRandomLocation()
                    : WorldSpaceUtil.GetRandomLocationLeftEdge();
                break;
            case 2:
                transform.position = WorldSpaceUtil.GetRandomLocationTopEdge();
                _destination = WorldSpaceUtil.GetRandomLocationBottomEdge();
                _secondDestination = _size == AlienSize.Small
                    ? WorldSpaceUtil.GetRandomLocation()
                    : WorldSpaceUtil.GetRandomLocationBottomEdge();
                break;
            default:
                transform.position = WorldSpaceUtil.GetRandomLocationBottomEdge();
                _destination = WorldSpaceUtil.GetRandomLocationTopEdge();
                _secondDestination = _size == AlienSize.Small
                    ? WorldSpaceUtil.GetRandomLocation()
                    : WorldSpaceUtil.GetRandomLocationTopEdge();
                break;
        }

        // Point the alien at the new location at its appropriate speed
        _rigidbody2D.velocity = Vector3.Normalize(_destination - transform.position) * _speed;
        // Turn on the sound
        _audioSource.clip =
            Resources.Load<AudioClip>(_size == AlienSize.Small ? "Audio/move_alien_small" : "Audio/move_alien_big");
        _audioSource.loop = true;
        _audioSource.Play();
        // Start the shooting coroutine
        StartCoroutine(FireAtSpaceship());
        StartCoroutine(SetSecondDestination());
    }
    
    void Update()
    {
        // If this spaceship is near its destination, safe to kill it
        if (WorldSpaceUtil.IsOutsideWorldspace(transform.position)) {
            _levelController.OnAlienGone(this);
        }
    }

    /**
     * After a pseudo-random delay, fires at the player's spaceship
     */
    private IEnumerator FireAtSpaceship() {
        // Wait a random period of time
        yield return new WaitForSeconds(Random.Range(_minFireDelay, _maxFireDelay));
        // Fire a bullet at the player, if the spaceship is still around
        GameObject spaceship = GameObject.FindWithTag("Player");
        if (spaceship != null) {
            _levelController.PlaySound("fire_alien");
            BulletController bullet = Instantiate(Resources.Load<BulletController>("Prefabs/Bullet"));
            bullet.InitializeFromAlien(this, spaceship.GetComponent<SpaceshipController>(), _bulletDrift);
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
        yield return new WaitForSeconds(Random.Range(_minFireDelay+1, _maxFireDelay+1));
        _rigidbody2D.velocity = Vector3.Normalize(_secondDestination - transform.position) * _speed;
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
