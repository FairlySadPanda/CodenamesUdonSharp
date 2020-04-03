using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[AddComponentMenu("")]
public class AgentControls : UdonSharpBehaviour
{
    public Manager manager;

    [UdonSynced]
    public bool isEnabled;
    private int hintNumber;
    private int card;
    private string teamName;

    public void Setup(string team)
    {
        Debug.Log("Setting up agent");
        this.teamName = team;
        this.reset();
    }

    public void Teardown()
    {
        this.reset();
    }

    public void StartTurn(int hints)
    {
        Debug.Log("Starting agent turn: " + teamName);
        this.hintNumber = hints;
        this.isEnabled = true;
        this.updateText();
        manager.UpdateCursor(this.card);
    }

    public void EndTurn()
    {
        Debug.Log("Ending agent turn: " + teamName);
        this.reset();
        this.manager.DestroyCursor();
    }

    public void SelectCard()
    {
        if (this.isEnabled == false)
        {
            return;
        }

        bool isCardSelected = this.manager.IsCardSelected(this.card);

        if (isCardSelected == false)
        {
            Debug.Log(this.teamName + " agent: Card already selected");
            return;
        }

        Debug.Log(this.teamName + " agent: Card selected");
        this.hintNumber = this.hintNumber - 1;

        this.updateText();

        this.manager.SelectCard(this.card);
    }

    public void ContinueTurn()
    {
        if (this.hintNumber < 0)
        {
            this.manager.AgentEndTurn();
        }
    }

    public void IncrementCard()
    {
        if (this.isEnabled == false)
        {
            return;
        }

        Debug.Log("Incremented card");
        if (this.card >= 24)
        {
            this.card = 0;
        }
        else
        {
            this.card = this.card + 1;
        }

        Debug.Log("New cursor location: " + this.card);
        this.manager.UpdateCursor(this.card);
    }

    public void DecrementCard()
    {
        if (this.isEnabled == false)
        {
            return;
        }


        if (this.card <= 0)
        {
            this.card = 24;
        }
        else
        {
            this.card = this.card - 1;
        }

        Debug.Log("New cursor location: " + this.card);
        this.manager.UpdateCursor(this.card);
    }

    private void stopTurn()
    {
        this.isEnabled = false;
        this.manager.AgentEndTurn();
    }

    private void reset()
    {
        this.card = 0;
        this.hintNumber = 0;
        this.isEnabled = false;
    }

    private void updateText()
    {
        manager.UpdateText("It's the " + this.teamName + " agent's turn! Hints remaining: " + this.hintNumber);
    }
}