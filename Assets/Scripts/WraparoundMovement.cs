// Copyright 2020 Ideograph LLC. All rights reserved.
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

// If something flies off to the right, it should appear on the left; off the top, appear on the bottom; etc.
public class WraparoundMovement : MonoBehaviour {
    private Vector3 _screenDimensions;

    public void Update() {
        // Calculate the screen dimensions only once; they never change in this game
        if (_screenDimensions.x == 0) {
            if (Camera.current != null) {
                _screenDimensions = Camera.current.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, 0.0f));
            }
        }
        if (_screenDimensions.x > 0) {
            // Do the wraparound as needed
            Vector3 objectPosition = gameObject.transform.position;
            float x = objectPosition.x;
            float y = objectPosition.y;
            if (objectPosition.x > _screenDimensions.x) {
                x = -_screenDimensions.x;
            }
            else if (objectPosition.x < -_screenDimensions.x) {
                x = _screenDimensions.x;
            }

            if (objectPosition.y > _screenDimensions.y) {
                y = -_screenDimensions.y;
            }
            else if (transform.position.y < -_screenDimensions.y) {
                y = _screenDimensions.y;
            }

            gameObject.transform.position = new Vector3(x, y, objectPosition.z);
        }
    }
}