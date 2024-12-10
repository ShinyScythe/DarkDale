using UnityEngine;
using PurrNet;
using TMPro;
using Unity.VisualScripting;

public class PlayerChatController : NetworkBehaviour
{
    protected override void OnSpawned()
    {
        base.OnSpawned();

        enabled = isOwner;
    }

    private struct Message
    {
        private string text;
    }
}
