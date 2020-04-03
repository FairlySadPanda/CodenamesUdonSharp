using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[AddComponentMenu("")]
public class OperatorHintCommit : UdonSharpBehaviour
{
    public string colour;
    public Manager controller;

    // public void OnMouseDown()
    // {
    //     this.controller.CommitHintNumber();
    // }

    public override void Interact()
    {
        controller.SendCustomNetworkEvent(NetworkEventTarget.Owner, "CommitHintNumber" + colour);
    }
}