// Copyright 2020 Ideograph LLC. All rights reserved.
using UnityEngine;

public class Util {

    /**
     * Returns the level controller that is part of the scene
     */
    public static LevelController GetLevelController() {
        GameObject lc = GameObject.FindWithTag("LevelController");
        if (lc != null) {
            return lc.GetComponent<LevelController>();
        }
        return null;
    }

    /**
     * Returns a velocity pointing in a random direction going a speed between the passed-in values
     */
    public static Vector2 GetRandomVelocity(float minSpeed, float maxSpeed) {
        float degree = Random.Range(0.0f, 360.0f);
        Vector2 direction = new Vector2(Mathf.Cos(degree * Mathf.Deg2Rad), Mathf.Sin(degree * Mathf.Deg2Rad));
        return direction * Random.Range(minSpeed, maxSpeed);
    } 
}
