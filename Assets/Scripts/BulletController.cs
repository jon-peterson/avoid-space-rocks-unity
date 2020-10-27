// Copyright 2020 Ideograph LLC. All rights reserved.
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(WraparoundMovement))]    
public class BulletController : MonoBehaviour {

    [SerializeField] private float bulletSpeed = 5.0f;
    [SerializeField] private float bulletLifetime = 2.0f;
    private Rigidbody2D _rigidbody2D;
    private LevelController _levelController;
    private bool _firedFromSpaceship;

    public float BulletLifetime {
        get {
            return bulletLifetime;
        }
    }
    
    // Start is called before the first frame update
    void Start() {
        Destroy(gameObject, bulletLifetime);
        _levelController = Util.GetLevelController();
    }
    
    void Awake() {        
        _rigidbody2D = GetComponent<Rigidbody2D>(); 
    }

    /**
     * Called when a spaceship fires the bullet: set the velocity based on how the ship is currently moving
     */
    public void InitializeFromSpaceship(SpaceshipController spaceship) {
        _firedFromSpaceship = true;
        // The velocity of the bullet is the same direction as the spaceship points only faster
        Vector2 forward = spaceship.transform.TransformDirection(Vector3.right);
        Vector2 boost = (forward.normalized * bulletSpeed);
        _rigidbody2D.velocity = spaceship.GetComponent<Rigidbody2D>().velocity + boost;
        gameObject.transform.position = spaceship.transform.position;
    }

    /**
     * Called when an Alien fires the bullet: point the bullet where the spaceship is now. Lower loose numbers
     * are better -- a loose of zero shoots directly at where the spaceship is now; a higher loose means further
     * away from the spaceship, in unit terms, but random.
     */
    public void InitializeFromAlien(AlienController alien, SpaceshipController spaceship, float loose) {
        float wiggleX = Random.Range(-loose, loose);
        float wiggleY = Random.Range(-loose, loose);
        Vector2 alienPos = alien.transform.position;
        Vector2 spaceshipPos = spaceship.transform.position;
        Vector2 targetPos = new Vector2(spaceshipPos.x + wiggleX, spaceshipPos.y + wiggleY);
        Vector2 toSpaceship = targetPos - alienPos;
        _rigidbody2D.velocity = toSpaceship.normalized * bulletSpeed;
        gameObject.transform.position = alienPos;
    }

    public void OnTriggerEnter2D(Collider2D other) {
        RockController rock = other.gameObject.GetComponent<RockController>();
        if (rock) {
            _levelController.DestroyRock(rock);
            Destroy(gameObject);
            return;
        }
        SpaceshipController spaceship = other.gameObject.GetComponent<SpaceshipController>();
        if (spaceship && !_firedFromSpaceship) {
            _levelController.DestroySpaceship(spaceship);
            Destroy(gameObject);
            return;
        }
        AlienController alien = other.gameObject.GetComponent<AlienController>();
        if (alien && _firedFromSpaceship) {
            _levelController.DestroyAlien(alien);
            Destroy(gameObject);
            return;
        }
    }
}
