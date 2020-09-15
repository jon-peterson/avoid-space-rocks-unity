// Copyright 2020 Ideograph LLC. All rights reserved.
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(WraparoundMovement))]    
public class BulletController : MonoBehaviour {

    [SerializeField] private float bulletSpeed = 5.0f;
    
    private Rigidbody2D _rigidbody2D;
    
    // Start is called before the first frame update
    void Start() {
        Destroy(gameObject, 2.0f);        
    }

    public void Initialize(SpaceshipController spaceship) {
        // The velocity of the bullet is the same direction as the spaceship points only faster
        _rigidbody2D = GetComponent<Rigidbody2D>(); 
        Vector2 forward = spaceship.transform.TransformDirection(Vector3.right);
        Vector2 boost = (forward.normalized * bulletSpeed);
        _rigidbody2D.velocity = spaceship.GetComponent<Rigidbody2D>().velocity + boost;
        gameObject.transform.position = spaceship.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
