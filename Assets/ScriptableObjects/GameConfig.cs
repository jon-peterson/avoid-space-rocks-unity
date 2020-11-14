// Copyright 2020 Ideograph LLC. All rights reserved.

using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameConfig", order = 2)]
public class GameConfig : ScriptableObject
{
    [Serializable]
    public class PointsClass {
        public int largeRock;
        public int mediumRock;
        public int smallRock;
        public int tinyRock;
        public int alienBig;
        public int alienSmall;
        public int forNewLife;
    }

    [Serializable]
    public class SpawnClass {
        public float alien;
        public float waitAtLeast;
    }

    [SerializeField] public PointsClass Points;
    [SerializeField] public SpawnClass SpawnTime;

}
