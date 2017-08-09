# HoloSharingNetworkingApp
This app will demonstrate a lot of features for a shared experience with other devices:
* Running a shared service
* Connecting to the service via IP address (SharingStage)
* NetworkConnection and NetworkConnectionAdapter (HoloToolkit classes) for passing messages between devices
* World Anchor Store and persistence across app termination

## Hot Potato
The concept of this game is simple.  It's a layman's version of hot potato.  The first user to join will be given the hot potato.  Only this user can place the potato.  The other user can only know when the potato is in the act of being placed (Yellow and spinning) or when it got placed (Red and not spinning).  The user who is placing the potato can only know when the potato is in the act of being placed (Red and spinning) or when it got placed (Green and not spinning).

## Unity Editor
This app will work with the Unity Editor, but directive statements were put in the code to prevent it from doing anything with spatial anchors.
