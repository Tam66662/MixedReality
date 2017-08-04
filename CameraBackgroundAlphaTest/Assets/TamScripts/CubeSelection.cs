using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CubeSelection : MonoBehaviour, IInputClickHandler {

    public TextMesh textMesh;

    private float rotationAmount = 50;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (GazeManager.Instance.HitObject != null && GazeManager.Instance.HitObject.name == this.name)
        {
            if (this.name == "White")
            {
                Camera.main.backgroundColor = Color.white;
                if (Camera.main.clearFlags == CameraClearFlags.Skybox) Camera.main.clearFlags = CameraClearFlags.SolidColor;
            }
            else if (this.name == "Black")
            {
                Camera.main.backgroundColor = Color.black;
                if (Camera.main.clearFlags == CameraClearFlags.Skybox) Camera.main.clearFlags = CameraClearFlags.SolidColor;
            }
            else if (this.name == "Gray")
            {
                Camera.main.backgroundColor = Color.gray;
                if (Camera.main.clearFlags == CameraClearFlags.Skybox) Camera.main.clearFlags = CameraClearFlags.SolidColor;
            }
            else if (this.name == "Skybox")
            {
                Camera.main.clearFlags = CameraClearFlags.Skybox;
            }

            textMesh.text = string.Format("Background is set to {0}", this.name);
        }
    }

    private void Start()
    {
        if (this.name == "White")
        {
            textMesh.text = "Tap a color to change the background";
        }
    }

    private void Update()
    {
        this.transform.Rotate(Vector3.up, rotationAmount * Time.deltaTime);
    }
}
