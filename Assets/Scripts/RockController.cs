// Copyright 2020 Ideograph LLC. All rights reserved.
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(WraparoundMovement))]    
public class RockController : MonoBehaviour
{
    [SerializeField] private float minRotatePerSecond = 45.0f;
    [SerializeField] private float maxRotatePerSecond = 360.0f;
    [SerializeField] private float minSpeed = 2.0f;
    [SerializeField] private float maxSpeed = 10.0f;

    private Rigidbody2D _rigidbody2D;
    private float _rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        // Rocks never slow down
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.drag = 0.0f;
        _rigidbody2D.gravityScale = 0.0f;
        // Point the rock in a random direction and set it moving at a random speed
        float degree = Random.Range(0.0f, 360.0f);
        Vector2 direction = new Vector2(Mathf.Cos(degree * Mathf.Deg2Rad), Mathf.Sin(degree * Mathf.Deg2Rad));
        _rigidbody2D.velocity = direction * Random.Range(minSpeed, maxSpeed);
        // Give it an appropriate random rotation
        _rotationSpeed = Random.Range(minRotatePerSecond, maxRotatePerSecond);
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate based on its rotation speed
        _rigidbody2D.transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime); 
    }
}
