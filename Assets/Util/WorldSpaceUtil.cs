// Copyright 2020 Ideograph LLC. All rights reserved.

using System;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class WorldSpaceUtil {

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

    /**
     * Returns a random location along one of the sides of the worldspace
     */
    public static Vector2 GetRandomEdgeLocation() {
        return Random.Range(0, 1) == 0
            ? WorldSpaceUtil.GetRandomLocationTopEdge()
            : WorldSpaceUtil.GetRandomLocationLeftEdge();
    }
    
    public static Vector2 GetRandomLocationTopEdge() {
        WorldSpace world = GetWorldSpace();
        return new Vector2(Random.Range(world.Left, world.Right), world.Top);
    }

    public static Vector2 GetRandomLocationBottomEdge() {
        WorldSpace world = GetWorldSpace();
        return new Vector2(Random.Range(world.Left, world.Right), world.Bottom);
    }

    public static Vector2 GetRandomLocationLeftEdge() {
        WorldSpace world = GetWorldSpace();
        return new Vector2(world.Left, Random.Range(world.Bottom, world.Top));
    }

    public static Vector2 GetRandomLocationRightEdge() {
        WorldSpace world = GetWorldSpace();
        return new Vector2(world.Right, Random.Range(world.Bottom, world.Top));
    }

    public static bool IsOutsideWorldspace(Vector2 pos) {
        WorldSpace world = GetWorldSpace();
        return (pos.x < (world.Left - 0.1)) ||
               (pos.x > (world.Right + 0.1)) ||
               (pos.y > (world.Top + 0.1)) ||
               (pos.y < (world.Bottom - 0.1));
    }
}