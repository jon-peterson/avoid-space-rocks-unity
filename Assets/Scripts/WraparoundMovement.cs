// Copyright 2020 Ideograph LLC. All rights reserved.
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

// If something flies off to the right, it should appear on the left; off the top, appear on the bottom; etc.
public class WraparoundMovement : MonoBehaviour {
    
    public void Update() {
        // Calculate the screen dimensions only once; they never change in this game
        Util.WorldSpace world = Util.GetWorldSpace();
        if (world.Right > 0) {
            // Do the wraparound as needed
            Vector3 objectPosition = gameObject.transform.position;
            float x = objectPosition.x;
            float y = objectPosition.y;
            if (objectPosition.x > world.Right) {
                x = world.Left;
            }
            else if (objectPosition.x < world.Left) {
                x = world.Right;
            }
            if (objectPosition.y < world.Bottom) {
                y = world.Top;
            }
            else if (objectPosition.y > world.Top) {
                y = world.Bottom;
            }
            gameObject.transform.position = new Vector3(x, y, objectPosition.z);
        }
    }
}