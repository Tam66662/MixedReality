using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RegularTextUpdate : MonoBehaviour, IInputClickHandler {

    public TextMesh TextMesh;

    private int frameCounter = 0;

    private bool paused;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        paused = !paused;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (!paused)
        {
            frameCounter++;
            TextMesh.text = string.Format("Regular Text Update: Showing the current frame to be {0}", frameCounter);

            if (frameCounter >= 60)
            {
                frameCounter = 0;
            }
        }
        else
        {
            TextMesh.text = string.Format("Regular Text Update: Paused (tap again to unpause)");
        }
    }
}
