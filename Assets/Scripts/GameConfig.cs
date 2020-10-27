// Copyright 2020 Ideograph LLC. All rights reserved.

public struct GameConfig
{
    public struct PointsStruct {
        public int LargeRock { get; set; }
        public int MediumRock { get; set; }
        public int SmallRock { get; set; }
        public int TinyRock { get; set; }
        public int AlienBig { get; set; }
        public int AlienSmall { get; set; }
        public int ForNewLife { get; set; }
    }

    public struct SpawnStruct {
        public float Alien { get; set; }
        public float WaitAtLeast { get; set; }
    }
    public int StartingLives { get; set; }
    public PointsStruct Points { get; set; }
    public SpawnStruct SpawnTime { get; set; }
}
