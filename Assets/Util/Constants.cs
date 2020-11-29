// Copyright 2020 Ideograph LLC. All rights reserved.
public struct Constants {
    
#if UNITY_EDITOR	
    public const string identityPoolId = "us-east-2:db7bd2f8-f47d-49d4-8adb-8011f1d1ca52";
    public const string gameTable = "avoid-space-rocks-player-dev";
#else
    public const string identityPoolId = "us-east-2:11e565e2-5fc2-42b8-8d7c-933cf7150d5b";
    public const string gameTable = "avoid-space-rocks-player-prod";
#endif
    
}
