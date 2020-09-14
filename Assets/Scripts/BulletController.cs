// Copyright 2020 Ideograph LLC. All rights reserved.
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(WraparoundMovement))]    
public class BulletController : MonoBehaviour {

    private Rigidbody2D _rigidbody2D;
    
    // Start is called before the first frame update
    void Start() {
        Destroy(gameObject, 2.0f);        
    }

    public void Initialize(SpaceshipController spaceship) {
        _rigidbody2D = GetComponent<Rigidbody2D>(); 
        _rigidbody2D.velocity = spaceship.GetComponent<Rigidbody2D>().velocity;
        gameObject.transform.position = spaceship.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
