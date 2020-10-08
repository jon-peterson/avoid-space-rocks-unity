// Copyright 2020 Ideograph LLC. All rights reserved.
using UnityEngine;
using UnityEngine.SceneManagement;

public class AttractModeController : MonoBehaviour
{
    // Start new game on any key
    void Update()
    {
        if (Input.anyKeyDown) {
            SceneManager.LoadScene("PlayfieldScene", LoadSceneMode.Single);
        }
    }
}
