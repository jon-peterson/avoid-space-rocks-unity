// Copyright 2020 Ideograph LLC. All rights reserved.
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

// If something flies off to the right, it should appear on the left; off the top, appear on the bottom; etc.
public class WraparoundMovement : MonoBehaviour {

    public void Update() {

        Vector3 spaceshipPosition = gameObject.transform.position;
        Vector3 screenDimensions = Camera.current.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, 0.0f));

        float x = spaceshipPosition.x;
        float y = spaceshipPosition.y;
        if (spaceshipPosition.x > screenDimensions.x) {
            x = -screenDimensions.x;
        } else if (spaceshipPosition.x < -screenDimensions.x) {
            x = screenDimensions.x;
        }
        if (spaceshipPosition.y > screenDimensions.y) {
            y = -screenDimensions.y;
        } else if (transform.position.y < -screenDimensions.y) {
            y = screenDimensions.y;
        }

        gameObject.transform.position = new Vector3(x, y, spaceshipPosition.z);
    }
}
