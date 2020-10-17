// Copyright 2020 Ideograph LLC. All rights reserved.

public struct GameConfig
{
    public struct PointsStruct {
        public int LargeRock { get; set; }
        public int MediumRock { get; set; }
        public int SmallRock { get; set; }
        public int TinyRock { get; set; }
    }

    public int StartingLives { get; set; }
    public int PointsForNewLife { get; set; }
    public PointsStruct Points { get; set; }
}
