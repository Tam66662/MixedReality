using HoloToolkit.Sharing;
using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Persistence;
using System.Linq;
using System.Reflection;
using System.Text;
using HoloToolkit.Unity;

public class PotatoManager : Singleton<PotatoManager>, IInputClickHandler {

    public static bool HasPotato { get; set; }

    public static bool Placing { get; set; }

    public static bool HasWorldAnchor { get; set; }

    private float rotationAmount = 50f;

    public TextMesh textMesh;

    private WorldAnchorStore worldAnchorStore;

    private MeshRenderer meshRenderer;

	// Use this for initialization
	void Start () {
#if !UNITY_EDITOR
        WorldAnchorStore.GetAsync(LoadWorldAnchorStore);
#else
        //LogMe("SKIPPING: UNITY_EDITOR cannot load the world anchor store");
#endif
        meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
    }

    private void LoadWorldAnchorStore(WorldAnchorStore store)
    {
#if !UNITY_EDITOR
        LogMe("LoadWorldAnchorStore");

        this.worldAnchorStore = store;
        if (store.anchorCount > 0)
        {
            // If you have an anchor, place the potato at this anchor point.
            var allIds = store.GetAllIds();
            var potatoId = allIds.FirstOrDefault(id => id == this.gameObject.name);
            if (potatoId != null)
            {
                // Load the store's anchor back into this potato
                var anchor = store.Load(potatoId, this.gameObject);
                LogMe("\tStored anchor found");
            }
            else
            {
                LogMe(string.Format("\tNo anchor '{0} as found", this.gameObject.name));
            }
        }
        else
        {
            LogMe(string.Format("\tNo anchors exist"));
        }
#endif
    }

    // Update is called once per frame
    void Update () {
        // If you have the potato, then it should be placed at the spot of your gaze.
        if (Placing && HasPotato)
        {
            this.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1;
            this.transform.Rotate(Vector3.up, rotationAmount * Time.deltaTime);
            SharingStatus.Instance.SendNetworkUserMessage(SharingStatus.UserMessageId.ObjectTransform, this.transform);
        }
        else if (Placing && !HasPotato)
        {
            //this.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1;
            this.transform.Rotate(Vector3.up, rotationAmount * Time.deltaTime);
            //SharingStatus.Instance.SendNetworkUserMessage(SharingStatus.UserMessageId.ObjectTransform, this.transform);
        }
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (HasPotato)
        {
            // Prior to changing the state of Placing, remove or set the world anchor.  This way,
            // it doesn't crash because Placing affects the Update() method that runs 60 fps.
            if (!Placing)
            {
                // Release the world anchor and then broadcast it to other users.
                RemoveWorldAnchor();
            }
            else
            {
                // Set the world anchor and then broadcast it to other users.
                SetWorldAnchor();
            }

            Placing = !Placing;

            // Send a message that you are moving the object
            if (Placing)
            {
                SharingStatus.Instance.SendNetworkUserMessage(SharingStatus.UserMessageId.ObjectPickedUp);
            }
            else
            {
                SharingStatus.Instance.SendNetworkUserMessage(SharingStatus.UserMessageId.ObjectPlaced, this.gameObject.transform);

                // Once you place the potato, you no longer have control of it.  The other user is now given control.
                // Unless, you are the only user in the game.
                if (SharingStage.Instance.SessionUsersTracker.CurrentUsers.Count > 1)
                {
                    HasPotato = false;
                }
            }

            UpdatePotatoColor();

            LogMe(string.Format("You {0} the potato", HasPotato && Placing ? "picked up" : "dropped"));
        }
        else
        {
            LogMe("You can't do that because you do not have the potato");
        }

    }

    public void UpdatePotatoColor()
    {
        if (meshRenderer.material != null)
        {
            if (HasPotato)
            {
                if (Placing)
                {
                    meshRenderer.material.color = Color.red;
                }
                else
                {
                    meshRenderer.material.color = Color.yellow;
                }
            }
            else
            {
                if (Placing)
                {
                    meshRenderer.material.color = Color.yellow;
                }
                else
                {
                    meshRenderer.material.color = Color.green;
                }
            }
        }
    }

    private void Anchor_OnTrackingChanged(WorldAnchor self, bool located)
    {
        if (located)
        {
            worldAnchorStore.Save(this.gameObject.name, self);
            LogMe(string.Format("'{0}' anchor has been saved.", this.gameObject.name));
        }
    }

    internal void SetWorldAnchor()
    {
#if !UNITY_EDITOR
        // Set the world anchor and then broadcast it to other users.
        var anchor = this.gameObject.AddComponent<WorldAnchor>();
        anchor.OnTrackingChanged += this.Anchor_OnTrackingChanged;
#else
        //LogMe("SKIPPING: UNITY_EDITOR cannot set the world anchor");
#endif
    }

    internal void RemoveWorldAnchor()
    {
#if !UNITY_EDITOR
        var worldAnchor = this.gameObject.GetComponent<WorldAnchor>();
        if (worldAnchor != null)
        {
            DestroyImmediate(worldAnchor);
        }

        if (worldAnchorStore.Delete(this.gameObject.name))
        {
            LogMe(string.Format("'{0}' anchor has been deleted.", this.gameObject.name));
        }
#else
        //LogMe("SKIPPING: UNITY_EDITOR cannot remove the world anchor");
#endif
    }

    private void LogMe(string message)
    {
        if (textMesh != null)
        {
            if (textMesh.text.Select(t => t == '\n').Count() > 10)
            {
                var firstCarriageReturn = textMesh.text.IndexOf('\n', 0);
                textMesh.text.Remove(0, firstCarriageReturn + 1);
            }

            textMesh.text += string.Format("{0}\n", message);
        }
    }

    public void PotatoInitialize()
    {
        if (SharingStage.Instance.SessionUsersTracker != null)
        {
            var stringBuilder = new StringBuilder();
            foreach (var user in SharingStage.Instance.SessionUsersTracker.CurrentUsers)
            {
                var xName = user.GetName();
                if (stringBuilder.ToString() == string.Empty)
                {
                    stringBuilder.Append(string.Format("\tUsers: {0}", xName.ToString()));
                }
                else
                {
                    stringBuilder.Append(", " + xName.ToString());
                }
            }

            textMesh.text += string.Format("{0} (Count Now: {1})\n", stringBuilder.ToString(), SharingStage.Instance.SessionUsersTracker.CurrentUsers.Count);
        }

        if (SharingStatus.FirstUser)
        {
            // If the cube is in the original world position, move it to be in front of the user's gaze.
            if (this.gameObject.transform.position == Vector3.zero)
            {
                this.gameObject.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1;
            }

            HasPotato = true;
            LogMe("You have the hot potato");
        }
        else
        {
            // If the cube is in the original world position, move it to be in front of the user's gaze.
            if (this.gameObject.transform.position == Vector3.zero)
            {
                this.gameObject.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1;
            }

            LogMe("You do not have the hot potato");
        }

        UpdatePotatoColor();
    }
}
