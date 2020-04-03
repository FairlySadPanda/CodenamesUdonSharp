
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using VRC.Udon.Common.Interfaces;

/**
 * Manager manages the Codename game's state machine
 */
[AddComponentMenu("")]
public class Manager : UdonSharpBehaviour
{
    public OperatorControls redOperatorControls;
    public OperatorControls blueOperatorControls;
    public AgentControls redAgentControls;
    public AgentControls blueAgentControls;
    public LayoutManager layoutManager;
    public CardManager cardManager;
    public Text status;

    [UdonSynced]
    public string statusText;
    private bool gameRunning;
    private bool isRedTurn;

    private float update;

    public void Update()
    {
        update += Time.deltaTime;
        if (update > 0.5f)
        {
            status.text = statusText;
            update = 0f;
        }

    }

    public void IncrementHintRed()
    {
        if (this.isRedTurn)
        {
            redOperatorControls.IncrementHint();
        }
    }

    public void IncrementHintBlue()
    {
        if (!this.isRedTurn)
        {
            blueOperatorControls.IncrementHint();
        }
    }

    public void DecrementHintRed()
    {
        if (this.isRedTurn)
        {
            redOperatorControls.DecrementHint();
        }
    }

    public void DecrementHintBlue()
    {
        if (!this.isRedTurn)
        {
            blueOperatorControls.DecrementHint();
        }
    }

    public void CommitHintNumberRed()
    {
        if (this.isRedTurn)
        {
            redOperatorControls.EndTurn();
        }
    }

    public void CommitHintNumberBlue()
    {
        if (!this.isRedTurn)
        {
            blueOperatorControls.EndTurn();
        }
    }

    public void IncrementCardRed()
    {
        if (this.isRedTurn)
        {
            redAgentControls.IncrementCard();
        }
    }

    public void IncrementCardBlue()
    {
        if (!this.isRedTurn)
        {
            blueAgentControls.IncrementCard();
        }
    }

    public void DecrementCardRed()
    {
        if (this.isRedTurn)
        {
            redAgentControls.DecrementCard();
        }
    }

    public void DecrementCardBlue()
    {
        if (!this.isRedTurn)
        {
            blueAgentControls.DecrementCard();
        }
    }


    public void ChooseCardRed()
    {
        if (this.isRedTurn)
        {
            redAgentControls.SelectCard();
        }
    }

    public void ChooseCardBlue()
    {
        if (!this.isRedTurn)
        {
            blueAgentControls.SelectCard();
        }
    }

    public void EndTurnRed()
    {
        if (this.isRedTurn)
        {
            AgentEndTurn();
        }
    }

    public void EndTurnBlue()
    {
        if (!this.isRedTurn)
        {
            AgentEndTurn();
        }
    }

    public void StartGame()
    {
        if (this.gameRunning == false)
        {
            Debug.Log("Game started");
            this.gameRunning = true;
            this.redOperatorControls.Setup("red");
            this.blueOperatorControls.Setup("blue");
            this.redAgentControls.Setup("red");
            this.blueAgentControls.Setup("blue");
            this.layoutManager.Setup();
            this.cardManager.Setup();
            this.blueOperatorControls.StartTurn();
        }
    }

    public void EndGame()
    {
        Debug.Log("Game ended");
        this.redOperatorControls.Teardown();
        this.blueOperatorControls.Teardown();
        this.redOperatorControls.Teardown();
        this.blueOperatorControls.Teardown();
        this.layoutManager.Teardown();
        this.cardManager.Teardown();
        this.gameRunning = false;
        this.isRedTurn = false;
    }

    public void OperatorEndTurn(int hintNumber)
    {
        if (this.isRedTurn == true)
        {
            Debug.Log("Red operator end turn");
            this.redOperatorControls.EndTurn();
            this.redAgentControls.StartTurn(hintNumber);
        }
        else
        {
            Debug.Log("Blue operator end turn");
            this.blueOperatorControls.EndTurn();
            this.blueAgentControls.StartTurn(hintNumber);
        }
    }

    public void AgentEndTurn()
    {
        if (this.isRedTurn == true)
        {
            Debug.Log("Red agent end turn");
            this.redAgentControls.EndTurn();
            this.isRedTurn = false;
            this.blueOperatorControls.StartTurn();
        }
        else
        {
            Debug.Log("Blue agent end turn");
            this.blueAgentControls.EndTurn();
            this.isRedTurn = true;
            this.redOperatorControls.StartTurn();
        }
    }

    public void UpdateCursor(int cardID)
    {
        Debug.Log("UpdateCursor called with cardID: " + cardID);
        this.cardManager.SetCardEvent("MoveCursor," + cardID);
    }

    public void DestroyCursor()
    {
        this.cardManager.DestroyCursor();
    }

    public bool IsCardSelected(int cardID)
    {
        bool isCardSelected = this.layoutManager.IsCardSelected(cardID);
        Debug.Log("Manager: Card was successfully selected? " + isCardSelected);
        return isCardSelected;
    }

    public void SelectCard(int cardID)
    {
        this.layoutManager.SelectCard(cardID);
    }

    public void BlueHit(int cardID)
    {
        Debug.Log("Blue card hit");
        this.cardManager.BlueHit();

        if (gameRunning)
        {
            this.cardManager.SetCardEvent("SetCardBlue," + cardID);

            if (this.isRedTurn)
            {
                this.AgentEndTurn();
            }
            else
            {
                this.blueAgentControls.ContinueTurn();
            }
        }

    }

    public void RedHit(int cardID)
    {
        Debug.Log("Red card hit");
        this.cardManager.RedHit();

        if (gameRunning)
        {
            this.cardManager.SetCardEvent("SetCardRed," + cardID);

            if (!this.isRedTurn)
            {
                this.AgentEndTurn();
            }
            else
            {
                this.redAgentControls.ContinueTurn();
            }
        }
    }

    public void CivilianHit(int cardID)
    {
        Debug.Log("Civilian hit");
        this.cardManager.CivilianHit();
        this.cardManager.SetCardEvent("SetCardGrey," + cardID);

        this.AgentEndTurn();
    }

    public void AssassinHit(int cardID)
    {
        Debug.Log("Assassin hit");
        gameRunning = false;
        SendCustomNetworkEvent(NetworkEventTarget.All, "EndGame");
    }

    public void BlueWin()
    {
        Debug.Log("Blue wins");
        gameRunning = false;
        SendCustomNetworkEvent(NetworkEventTarget.All, "EndGame");
    }

    public void RedWin()
    {
        Debug.Log("Red wins");
        gameRunning = false;
        SendCustomNetworkEvent(NetworkEventTarget.All, "EndGame");
    }

    public void UpdateText(string txt)
    {
        statusText = txt;
    }

}
