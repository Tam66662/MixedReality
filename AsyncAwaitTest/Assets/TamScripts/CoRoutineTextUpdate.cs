using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CoRoutineTextUpdate : MonoBehaviour, IInputClickHandler {

    public TextMesh TextMesh;

    private bool paused;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        paused = !paused;
    }

    // Use this for initialization
    void Start () {
        TextMesh.text = "THIS\nIS\nCOROUTINE ASYNC\nTEST";
        StartCoroutine(HeavyLoopRoutine());
	}

    private IEnumerator HeavyLoopRoutine()
    {
#if UNITY_UWP
        var count = 1;
        while (count < 1000)
        {
            TextMesh.text = string.Format("CoRoutine Text Update: Count {0}", count);
            var task = System.Threading.Tasks.Task.Delay(1000);
            yield return new WaitWhile(() => !task.IsCompleted);
            if (!paused)
            {
                count++;
            }
        }
#else
        var count = 1;
        while (count < 1000)
        {
            TextMesh.text = string.Format("CoRoutine Text Update: Count {0}", count);
            yield return new WaitForSeconds(1);
            if (!paused)
            {
                count++;
            }
        }
#endif
    }

    // Update is called once per frame
    void Update () {
        if (paused)
        {
            TextMesh.text = string.Format("CoRoutine Text Update: Paused (tap again to unpause)");
        }
    }
}
