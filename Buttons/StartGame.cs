
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[AddComponentMenu("")]
public class StartGameButton : UdonSharpBehaviour
{
    public Manager controller;

    public void OnMouseDown()
    {
        this.controller.StartGame();
    }

    public override void Interact()
    {
        controller.SendCustomNetworkEvent(NetworkEventTarget.Owner, "StartGame");
    }
}