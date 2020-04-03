using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[AddComponentMenu("")]
public class OperatorHintDecrement : UdonSharpBehaviour
{
    public string colour;
    public Manager controller;

    // public void OnMouseDown()
    // {
    //     this.controller.DecrementHint();
    // }

    public override void Interact()
    {
        controller.SendCustomNetworkEvent(NetworkEventTarget.Owner, "DecrementHint" + colour);
    }
}