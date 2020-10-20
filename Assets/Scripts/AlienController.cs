// Copyright 2020 Ideograph LLC. All rights reserved.
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AlienController : MonoBehaviour
{
    // How fast the ship moves
    [SerializeField] private float speed = 3.0f;
    private Vector3 _destination;
    private Rigidbody2D _rigidbody2D;

    void Awake() {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }
    
    void Start()
    {
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
}
