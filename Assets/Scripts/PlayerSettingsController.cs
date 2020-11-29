// Copyright 2020 Ideograph LLC. All rights reserved.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerSettingsController : MonoBehaviour
{
    [SerializeField] private GameStatus _gameStatus = default;

    void Start ()
    {
        GameObject canvas = GameObject.Find("Canvas");
        InputField field  = canvas.transform.Find("Initials").gameObject.GetComponent<InputField>();
        field.onEndEdit.AddListener(SubmitName);
    }

    private void SubmitName(string name)
    {
        if (name.Length == 3) {
            PlayerPrefs.SetString("name", name);
            PlayerPrefs.Save();
            SceneManager.LoadScene("HighScoreScene", LoadSceneMode.Single);
        }        
    }

}
