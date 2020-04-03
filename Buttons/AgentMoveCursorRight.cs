using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[AddComponentMenu("")]
public class AgentMoveCursorRight : UdonSharpBehaviour
{
    public string colour;
    public Manager controller;

    // public void OnMouseDown()
    // {
    //     this.controller.IncrementCard();
    // }

    public override void Interact()
    {
        controller.SendCustomNetworkEvent(NetworkEventTarget.Owner, "IncrementCard" + colour);
    }
}