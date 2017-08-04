using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CubeColorSwapper : MonoBehaviour, IInputClickHandler {

    public Material first;

    public Material second;

    private bool toggleColor;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        var meshRenderer = GetComponent<Renderer>();
        meshRenderer.material = toggleColor ? first : second;
        toggleColor = !toggleColor;
    }
}
