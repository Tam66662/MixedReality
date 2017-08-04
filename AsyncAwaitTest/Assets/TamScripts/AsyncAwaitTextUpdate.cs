using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AsyncAwaitTextUpdate : MonoBehaviour, IInputClickHandler {

    public TextMesh TextMesh;

    private bool paused;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        paused = !paused;
    }

    // Use this for initialization
    void Start () {
#if UNITY_UWP
        // System.Threading.Tasks.Task.Run(() => HeavyTaskAsync());
        //System.Threading.Tasks.Task.Run(async () => 
        //{
        //    var count = 1;
        //    while (count < 1000)
        //    {
        //        if (!paused)
        //        {
        //            UnityEngine.WSA.Application.InvokeOnAppThread(() =>
        //            {
        //                TextMesh.text = string.Format("AsyncAwait Text Update: Count {0}", count);
        //            }, true);

        //            await System.Threading.Tasks.Task.Delay(1000);
        //            count++;
        //        }
        //    }
        //});
        System.Threading.Tasks.Task.Run(async () => {
            UnityEngine.WSA.Application.InvokeOnAppThread(()=>{
                TextMesh.text = "Task.Run before : " + Time.time;
            }, true);

            await System.Threading.Tasks.Task.Delay(5000);

            UnityEngine.WSA.Application.InvokeOnAppThread(() => {
                TextMesh.text = "Task.Run after  : " + Time.time;
            }, true);
        });
#else
        TextMesh.text = "THIS\nIS\nAN ASYNC AWAIT\nTEST (Not defined with UNITY_UWP directive)";
#endif
    }

    // Update is called once per frame
    void Update () {
        if (paused)
        {
            TextMesh.text = string.Format("AsyncAwait Text Update: Paused (tap again to unpause)");
        }
    }

#if UNITY_UWP
    private async System.Threading.Tasks.Task HeavyTaskAsync()
    {
        var count = 1;
        while (count < 1000)
        {
            UnityEngine.WSA.Application.InvokeOnAppThread(() =>
            {
                TextMesh.text = string.Format("AsyncAwait Text Update: Count {0}", count);
            }, true);

            await System.Threading.Tasks.Task.Delay(1000);
            if (!paused)
            {
                count++;
            }
        }
    }
#endif
}
