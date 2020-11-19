// Copyright 2020 Ideograph LLC. All rights reserved.
using UnityEngine;

public enum Size {
    Large, Medium, Small, Tiny
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

}
