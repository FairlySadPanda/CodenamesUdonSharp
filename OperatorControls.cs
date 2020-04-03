using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[AddComponentMenu("")]
public class OperatorControls : UdonSharpBehaviour
{
    public Manager manager;
    private int hintNumber;
    private bool isEnabled;
    private string teamName;

    public void Setup(string team)
    {
        Debug.Log("Operator setup");
        this.teamName = team;
        this.reset();
    }

    public void Teardown()
    {
        this.reset();
    }

    public void StartTurn()
    {
        manager.UpdateText("Operator turn started: " + this.teamName);
        this.isEnabled = true;
    }

    public void EndTurn()
    {
        if (this.isEnabled == false)
        {
            return;
        }
        manager.UpdateText("Operator turn stopped: " + this.teamName);
        this.isEnabled = false;
        this.manager.OperatorEndTurn(this.hintNumber);
        this.reset();
    }

    public void IncrementHint()
    {
        if (this.isEnabled == false)
        {
            return;
        }
        this.hintNumber = this.hintNumber + 1;
        if (hintNumber == 0)
        {
            manager.UpdateText("It's the " + this.teamName + " operator's turn! They have a bonus guess!");
        }
        else
        {
            manager.UpdateText("It's the " + this.teamName + " operator's turn! Current guesses remaining: " + this.hintNumber);
        }

    }

    public void DecrementHint()
    {
        if (this.isEnabled == false)
        {
            return;
        }
        if (this.hintNumber < 1)
        {
            this.hintNumber = 0;
            manager.UpdateText("It's the " + this.teamName + " operator's turn! Current number of guesses: " + this.hintNumber);
            return;
        }
        this.hintNumber = this.hintNumber - 1;
        manager.UpdateText("It's the " + this.teamName + " operator's turn! Current number of guesses: " + this.hintNumber);
    }

    private void reset()
    {
        this.hintNumber = 0;
        this.isEnabled = false;
    }
}