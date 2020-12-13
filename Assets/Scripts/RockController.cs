// Copyright 2020 Ideograph LLC. All rights reserved.

using System.Collections.Generic;
using UnityEngine;

public enum Size {
    Large,
    Medium,
    Small,
    Tiny
}

[RequireComponent(typeof(RandomDirection))]
[RequireComponent(typeof(WraparoundMovement))]    
public class RockController : MonoBehaviour
{
    [SerializeField] private Size size = Size.Large;
    private LevelController _levelController;

    public Size Size => size;

    void Start() {
        _levelController = WorldSpaceUtil.GetLevelController();
    }

    /**
     * Blow things up if this bullet hits it
     */
    public void OnTriggerEnter2D(Collider2D other) {
        SpaceshipController spaceship = other.gameObject.GetComponent<SpaceshipController>();
        if (spaceship) {
            _levelController.DestroySpaceship(spaceship);
            return;
        }
        AlienController alien = other.gameObject.GetComponent<AlienController>();
        if (alien) {
            _levelController.DestroyAlien(alien);
            return;
        }
    }

    /**
     * Returns a list of shrapnel from the explosion
     */
    public List<GameObject> GetShrapnel() {
        List<GameObject> pieces = new List<GameObject>();
        for (int i = 0; i < Random.Range(3, 10); i++) {
            GameObject piece = Instantiate(Resources.Load<GameObject>("Prefabs/shrapnel"));
            piece.transform.position = gameObject.transform.position;
            piece.transform.localScale = new Vector3(Random.Range(0.5f, 1.2f), Random.Range(0.5f, 1.2f), 1.0f);
            pieces.Add(piece);
        }
        return pieces;
    }
}
