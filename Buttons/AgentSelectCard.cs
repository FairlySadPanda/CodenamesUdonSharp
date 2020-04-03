using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[AddComponentMenu("")]
public class AgentSelectCard : UdonSharpBehaviour
{
    public string colour;
    public Manager controller;

    // public void OnMouseDown()
    // {
    //     Debug.Log("on mouse down: select card");
    //     this.controller.ChooseCard();
    // }

    public override void Interact()
    {
        controller.SendCustomNetworkEvent(NetworkEventTarget.Owner, "ChooseCard" + colour);
    }
}