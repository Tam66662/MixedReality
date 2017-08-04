using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayedText : MonoBehaviour {

    [Tooltip("Reference to TextMesh component where the FPS should be displayed.")]
    [SerializeField]
    private TextMesh textMesh;

    private SphereBasedTagalong sphereBasedTagalong;

    // Use this for initialization
    void Start () {
        sphereBasedTagalong = GetComponentInParent<SphereBasedTagalong>();
        textMesh.text = string.Format(
            "This text will follow your gaze, within a Sphere " + "\n" +
            "Radius of {0} and a Move Speed of {1}. To tweak " + "\n" +
            "these values, edit the \"Sphere Based Tagalong\" script " + "\n" +
            "on the \"FPSDisplay\" GameObject of your scene to see " + "\n" +
            "how those values affect the canvas's behavior.", sphereBasedTagalong.SphereRadius, sphereBasedTagalong.MoveSpeed);
    }

    private void Update()
    {
    }
}
