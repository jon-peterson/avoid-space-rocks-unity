using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(WraparoundMovement))]    
public class SpaceshipController : MonoBehaviour {
    // Speed in degrees that the spaceship and rotate at once
    [SerializeField] private float rotateSpeed = 270.0f;

    // Force added when you hit the fuel
    [SerializeField] private float fuelBoost = 10.0f;

    // The maximum speed that you can make your ship
    [SerializeField] private float maxSpeed = 30.0f;
    
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    
    void Start() {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _rigidbody2D.drag = 0.2f;
    }

    void Update() {
        // Change rotation around the Z axis
        float rot = -Input.GetAxisRaw("Horizontal");
        transform.Rotate(0, 0, rot * rotateSpeed * Time.deltaTime);

        // Fuel burn
        _animator.SetBool("fuelBurn", false);
        if (Input.GetAxisRaw("Vertical") > 0) {
            _animator.SetBool("fuelBurn", true);
            Vector2 forward = transform.TransformDirection(Vector3.right);
            Vector2 velocity = _rigidbody2D.velocity + (forward.normalized * (fuelBoost * Time.deltaTime));
            _rigidbody2D.velocity = Vector2.ClampMagnitude(velocity, maxSpeed);
        }
    }
}