// Copyright 2020 Ideograph LLC. All rights reserved.

using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class AlienController : MonoBehaviour
{
    // How fast the ship moves
    [SerializeField] private float speed = 3.0f;
    private Vector3 _destination;
    private Rigidbody2D _rigidbody2D;
    private LevelController _levelController;

    void Awake() {
        _rigidbody2D = GetComponent<Rigidbody2D>();
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
                break;
            case 1:
                transform.position = Util.GetRandomLocationRightEdge();
                _destination = Util.GetRandomLocationLeftEdge();
                break;
            case 2:
                transform.position = Util.GetRandomLocationTopEdge();
                _destination = Util.GetRandomLocationBottomEdge();
                break;
            default:
                transform.position = Util.GetRandomLocationBottomEdge();
                _destination = Util.GetRandomLocationTopEdge();
                break;
        }
        // Calculate the velocity that the spaceship needs to hit it
        _rigidbody2D.velocity = (_destination - transform.position) * speed;
    }
    
    void Update()
    {
        // If this spaceship is near its destination, safe to kill it
        if (Util.IsOutsideWorldspace(transform.position)) {
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D other) {
        RockController rock = other.gameObject.GetComponent<RockController>();
        if (rock != null) {
            _levelController.DestroyAlien(this);
        }
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
}
