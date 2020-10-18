// Copyright 2020 Ideograph LLC. All rights reserved.
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public class Util {

    /**
     * Simple structure representing the limits of the world space
     */
    public readonly struct WorldSpace {
        public WorldSpace(float left, float right, float top, float bottom) {
            Assert.IsTrue(left < right);
            Assert.IsTrue(top > bottom);
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        public float Left { get; }
        public float Right { get; }
        public float Top { get; }
        public float Bottom { get; }
    }

    private static WorldSpace _worldSpace;

    /**
     * Returns the screen dimensions in world space
     */
    public static WorldSpace GetWorldSpace() {
        if (_worldSpace.Left == 0) {
            if (Camera.current != null) {
                Vector2 screen = Camera.current.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, 0.0f));
                _worldSpace = new WorldSpace(-screen.x, screen.x, screen.y, -screen.y);
            }
        }
        return _worldSpace;
    }

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

    /**
     * Returns a random location in worldspace
     */
    public static Vector2 GetRandomLocation() {
        WorldSpace world = GetWorldSpace();
        return new Vector2(Random.Range(world.Left, world.Right), Random.Range(world.Bottom, world.Top));
    }
}
