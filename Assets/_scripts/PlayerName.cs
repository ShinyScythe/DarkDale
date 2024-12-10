using UnityEngine;
using PurrNet;
using System.Runtime.CompilerServices;
using TMPro;
using System;

public class PlayerName : NetworkBehaviour
{
    public SyncVar<string> playerName = new("", ownerAuth: true);
    public TextMeshProUGUI nameText;

    private void Start()
    {
        if (isOwner)
        {
            print("owner");
            SetUsernameToServer(GameManager.Instance.PlayerUsername);
        }
            
        else if (!isOwner)
        {
            print("not owner.");
            SetUsernameFromServer(GameManager.Instance.PlayerUsername);
        }
            
    }

    [ServerRpc]
    public void SetUsernameToServer(string username)
    {
        print("Username: [" + username + "] sent to Server.");
        // Client sends this command to the server
        SetUsernameFromServer(username);
    }

    [ObserversRpc]
    public void SetUsernameFromServer(string username)
    {
        print("Username: [" + username + "] received from Server.");
        // Server sends this command to the clients
        // Update SyncVar
        playerName.value = username;
        print("Setting playerName.value: [" + playerName.value + "] to username: " + username);
        //update UI for all
        nameText.text = username;
        print("Updating TMP display with: " + username);
    }

    private void Update()
    {
        // Ensure the name text always faces the camera
        if (Camera.main != null && nameText != null)
        {
            Vector3 direction = nameText.transform.position - Camera.main.transform.position;
            nameText.transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}


