using HoloToolkit.Sharing;
using HoloToolkit.Unity;
using System;
using UnityEngine;

public class SharingStatus : Singleton<SharingStatus> {

    public TextMesh textMesh;

    public static bool FirstUser { get; set; }

    public enum UserMessageId : byte
    {
        ObjectPickedUp = MessageID.UserMessageIDStart,
        ObjectPlaced,
    }

    /// <summary>
    /// Used to route incoming messages from the network back to methods defined in this class.
    /// </summary>
    NetworkConnectionAdapter adapter;

    /// <summary>
    /// A cache of the network connection used by the sharing service.
    /// </summary>
    NetworkConnection connection;

    /// <summary>
    /// An identifier of the current user on the active server connection
    /// </summary>
    int localUserId;

    // Use this for initialization
    void Start () {
        LogMe(string.Format("{0}: Waiting for server connection", "Start"));

        if (Camera.main != null)
        {
            if (UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque)
            {
                Camera.main.clearFlags = CameraClearFlags.Skybox;
            }
            else
            {
                Camera.main.clearFlags = CameraClearFlags.SolidColor;
            }
        }

        if (SharingStage.Instance != null)
        {
            SharingStage.Instance.SharingManagerConnected += this.Instance_SharingManagerConnected;
            SharingStage.Instance.SessionsTracker.CurrentUserJoined += this.SessionsTracker_CurrentUserJoined;
            SharingStage.Instance.SessionsTracker.CurrentUserLeft += this.SessionsTracker_CurrentUserLeft;
            SharingStage.Instance.SessionsTracker.ServerConnected += this.SessionsTracker_ServerConnected;
            SharingStage.Instance.SessionsTracker.ServerDisconnected += this.SessionsTracker_ServerDisconnected;
            SharingStage.Instance.SessionsTracker.SessionAdded += this.SessionsTracker_SessionAdded;
            SharingStage.Instance.SessionsTracker.SessionClosed += this.SessionsTracker_SessionClosed;
            SharingStage.Instance.SessionsTracker.SessionCreated += this.SessionsTracker_SessionCreated;
            SharingStage.Instance.SessionsTracker.UserChanged += this.SessionsTracker_UserChanged;
            SharingStage.Instance.SessionsTracker.UserJoined += this.SessionsTracker_UserJoined;
            SharingStage.Instance.SessionsTracker.UserLeft += this.SessionsTracker_UserLeft;
            SharingStage.Instance.SessionUsersTracker.UserJoined += this.SessionUsersTracker_UserJoined;
            SharingStage.Instance.SessionUsersTracker.UserLeft += this.SessionUsersTracker_UserLeft;

            // Get the current sharing service's network connection
            connection = SharingStage.Instance.Manager.GetServerConnection();
            LogMe("Network connection is now cached");

            // Create a new network adapter so that you can route messages to and from this active user.
            adapter = new NetworkConnectionAdapter();

            // Register to the MessageReceived event
            adapter.MessageReceivedCallback += this.Adapter_MessageReceivedCallback;

            // Cache this user's id from the network
            this.localUserId = SharingStage.Instance.Manager.GetLocalUser().GetID();

            // Loop through each user message id that you plan to support, and listen for its id.
            foreach (var supportedMessageId in Enum.GetValues(typeof(UserMessageId)))
            {
                LogMe(string.Format("Registering: 0x{0:X4}", (byte)supportedMessageId));
                connection.AddListener((byte)supportedMessageId, adapter);
            }
        }
        else
        {
            Debug.Log("ruh roh!");
        }
    }

    private void Adapter_MessageReceivedCallback(NetworkConnection connection, NetworkInMessage msg)
    {
        byte messageType = msg.ReadByte();
        var userId = msg.ReadInt32();

        // Do something here when you receive the message
        LogMe(string.Format("Message Received: 0x{0}", messageType.ToString("X2")));
        var enumValue = (UserMessageId)messageType;
        if (enumValue == UserMessageId.ObjectPickedUp)
        {
            LogMe(string.Format("Message: User {0} picked up the object", userId));
            PotatoManager.Placing = true;
        }
        else if (enumValue == UserMessageId.ObjectPlaced)
        {
            LogMe(string.Format("Message: User {0} placed the object", userId));

            // If the object was placed, then the position and rotation of the object was sent as well.  Retrieve that data.
            var vector = new Vector3(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
            var rotation = new Quaternion(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
            LogMe(string.Format("Vector: {0},{1},{2}", vector.x, vector.y, vector.z));
            LogMe(string.Format("Quaternion: {0},{1},{2},{3}", rotation.x, rotation.y, rotation.z, rotation.w));

            // Before you move the object, remove any world anchor you may have on it.
            PotatoManager.Instance.RemoveWorldAnchor();
            PotatoManager.Instance.gameObject.transform.localPosition = vector;
            PotatoManager.Instance.gameObject.transform.localRotation = rotation;
            PotatoManager.Placing = false;
            PotatoManager.HasPotato = true;
            PotatoManager.Instance.SetWorldAnchor();
        }

        PotatoManager.Instance.UpdatePotatoColor();
    }

    private void SessionUsersTracker_UserLeft(User obj)
    {
        LogMe("SessionUsersTracker_UserLeft");
        LogMe(string.Format("\tUser {0} left (Count Now: {1})", obj.GetName(), SharingStage.Instance.SessionUsersTracker.CurrentUsers.Count));
    }

    private void SessionUsersTracker_UserJoined(User obj)
    {
        LogMe("SessionUsersTracker_UserJoined");
        LogMe(string.Format("\tUser {0} joined (Count Now: {1})", obj.GetName(), SharingStage.Instance.SessionUsersTracker.CurrentUsers.Count));
    }

    private void SessionsTracker_UserLeft(Session arg1, User arg2)
    {
        LogMe("SessionsTracker_UserLeft");
        LogMe(string.Format("\tUser {0} left Session {1} (Count Now: {2})", arg2.GetName(), arg1.GetName(), arg1.GetUserCount()));
        UpdateUsersInfo(arg1);
    }

    private void SessionsTracker_UserJoined(Session arg1, User arg2)
    {
        LogMe("SessionsTracker_UserJoined");
        LogMe(string.Format("\tUser {0} joined Session {1} (Count Now: {2})", arg2.GetName(), arg1.GetName(), arg1.GetUserCount()));
        UpdateUsersInfo(arg1);
    }

    private void SessionsTracker_CurrentUserJoined(Session obj)
    {
        LogMe("SessionsTracker_CurrentUserJoined");
        LogMe(string.Format("\tYou have joined Session {0} (Count Now: {1})", obj.GetName(), obj.GetUserCount()));
        UpdateUsersInfo(obj);
    }

    private void SessionsTracker_CurrentUserLeft(Session obj)
    {
        LogMe("SessionsTracker_CurrentUserLeft");
        LogMe(string.Format("\tYou have left Session {0} (Count Now: {1})", obj.GetName(), obj.GetUserCount()));
    }

    private void SessionsTracker_UserChanged(Session arg1, User arg2)
    {
        LogMe("SessionsTracker_UserChanged");
    }

    private void SessionsTracker_SessionCreated(bool arg1, string arg2)
    {
        LogMe("SessionsTracker_SessionCreated");
    }

    private void SessionsTracker_SessionClosed(Session obj)
    {
        LogMe("SessionsTracker_SessionClosed");
    }

    private void SessionsTracker_SessionAdded(Session obj)
    {
        LogMe("SessionsTracker_SessionAdded");
    }

    private void SessionsTracker_ServerDisconnected()
    {
        LogMe("SessionsTracker_ServerDisconnected");
    }

    private void SessionsTracker_ServerConnected()
    {
        LogMe("SessionsTracker_ServerConnected");
    }

    private void Instance_SharingManagerConnected(object sender, EventArgs e)
    {
        LogMe("Instance_SharingManagerConnected");
    }

    private void LogMe(string message)
    {
        if (textMesh != null)
        {
            textMesh.text += string.Format("{0}\n", message);
        }
    }

    private void UpdateUsersInfo(Session session)
    {
        // When you join or leave, check to see if you are the first or only user.
        if (session.GetUserCount() == 1)
        {
            // You are the first, so you own the potato.
            FirstUser = true;
        }

        PotatoManager.Instance.PotatoInitialize();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (this.connection != null)
        {
            foreach (var index in Enum.GetValues(typeof(UserMessageId)))
            {
                this.connection.RemoveListener((byte)index, this.adapter);
            }

            this.adapter.MessageReceivedCallback -= Adapter_MessageReceivedCallback;
        }
    }

    public void SendNetworkUserMessage(UserMessageId messageId, Transform objectTransform = null)
    {
        LogMe(string.Format("Sending Message: {0}", messageId));
        if (this.connection != null)
        {
            var networkMessage = this.connection.CreateMessage((byte)messageId);

            // Write the message id first
            networkMessage.Write((byte)messageId);

            // Now write something else, like the user's id
            networkMessage.Write(this.localUserId);

            if (objectTransform != null)
            {
                // Now send the transform info (Vector3 + Quaternion)
                networkMessage.Write(objectTransform.localPosition.x);
                networkMessage.Write(objectTransform.localPosition.y);
                networkMessage.Write(objectTransform.localPosition.z);
                networkMessage.Write(objectTransform.localRotation.x);
                networkMessage.Write(objectTransform.localRotation.y);
                networkMessage.Write(objectTransform.localRotation.z);
                networkMessage.Write(objectTransform.localRotation.w);
            }

            // Broadcast this message to everyone on the network immediately
            this.connection.Broadcast(networkMessage, MessagePriority.Immediate, MessageReliability.ReliableOrdered, MessageChannel.UserMessageChannelStart);
        }
    }
}
