// Copyright 2020 Ideograph LLC. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AttractModeController : MonoBehaviour {
    private List<Transform> _textContainers;

    void Start() {
        _textContainers = new List<Transform>();
        GameObject canvas = GameObject.Find("Canvas");
        _textContainers.Add(canvas.transform.Find("Title"));
        _textContainers.Add(canvas.transform.Find("Instructions"));
        _textContainers.Add(canvas.transform.Find("Key Bindings"));
        StartCoroutine(CycleThroughTitles());
    }

    private IEnumerator CycleThroughTitles() {
        while (true) {
            for (int i = 0; i < _textContainers.Count; i++) {
                _textContainers[i].gameObject.SetActive(true);
                yield return new WaitForSeconds(4.0f);
                _textContainers[i].gameObject.SetActive(false);
                yield return new WaitForSeconds(0.75f);
            }
        }
    }

    // Start new game on any key
    void Update()
    {
        if (Input.anyKeyDown) {
            SceneManager.LoadScene("PlayfieldScene", LoadSceneMode.Single);
        }
    }
}
