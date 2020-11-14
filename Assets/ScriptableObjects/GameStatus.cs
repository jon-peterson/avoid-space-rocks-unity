using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameStatus", order = 1)]
public class GameStatus : ScriptableObject {
    [SerializeField] private int score;
    [SerializeField] private int lives;
    [SerializeField] private int level;

    public int Score {
        get => score;
        set => score = value;
    }

    public int Lives {
        get => lives;
        set => lives = value;
    }

    public int Level {
        get => level;
        set => level = value;
    }

    public void Reset() {
        score = 0;
        lives = 3;
        level = 1;
    }
}
