using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    // Speed in degrees that the spaceship and rotate at once
    [SerializeField]
    private float RotateSpeed = 270.0f; 
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        // Change rotation around the Z axis
        float rot = -Input.GetAxisRaw("Horizontal");
        this.gameObject.transform.Rotate(0, 0, rot * RotateSpeed * Time.deltaTime);
        
        // Fuel burn
        // if(Input.GetAxisRaw("Vertical"))
    }
}
