using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[AddComponentMenu("")]
public class EndGameButton : UdonSharpBehaviour
{
    public Manager controller;

    public void OnMouseDown()
    {
        this.controller.EndGame();
    }

    public override void Interact()
    {
        controller.SendCustomNetworkEvent(NetworkEventTarget.Owner, "EndGame");
    }
}