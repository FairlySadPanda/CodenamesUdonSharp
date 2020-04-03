using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[AddComponentMenu("")]
public class LayoutManager : UdonSharpBehaviour
{
    public Manager manager;

    [UdonSynced]
    public string layout;

    private bool[] hits;

    public void Start()
    {
        this.reset();
    }

    public void Setup()
    {
        Debug.Log("Layout manager setup");
        this.reset();
        string[] layoutArray = layout.Split(',');
        for (int i = 0; i < layoutArray.Length; i++)
        {
            int rnd = Random.Range(0, layoutArray.Length);
            string a = layoutArray[i];
            string b = layoutArray[rnd];
            layoutArray[i] = b;
            layoutArray[rnd] = a;
        }

        layout = layoutArray[0];

        for (int i = 1; i < layoutArray.Length; i++)
        {
            string word = layoutArray[i];
            if (word == "")
            {
                continue;
            }

            layout = layout + "," + word;
        }
    }

    public void Teardown()
    {
        this.reset();
    }

    public bool IsCardSelected(int cardID)
    {
        if (this.hits[cardID] == true)
        {
            return false;
        }

        return true;
    }

    public void SelectCard(int cardID)
    {
        this.hits[cardID] = true;
        string[] layoutArray = layout.Split(',');

        Debug.Log("Layout: Card selected");
        int cardType = int.Parse(layoutArray[cardID]);

        if (cardType == 0)
        {
            this.manager.BlueHit(cardID);
        }
        else if (cardType == 1)
        {
            this.manager.RedHit(cardID);
        }
        else if (cardType == 2)
        {
            this.manager.CivilianHit(cardID);
        }
        else
        {
            this.manager.AssassinHit(cardID);
        }
    }

    public int CardType(int cardID)
    {
        string[] layoutArray = layout.Split(',');
        return int.Parse(layoutArray[cardID]);
    }

    private void reset()
    {
        int[] layoutArray = new int[25];
        hits = new bool[25];

        for (int i = 0; i < layoutArray.Length; i++)
        {
            if (i < 9)
            {
                layoutArray[i] = 0;
            }
            else if (i < 17)
            {
                layoutArray[i] = 1;
            }
            else if (i < 24)
            {
                layoutArray[i] = 2;
            }
            else
            {
                layoutArray[i] = 3;
            }
        }

        layout = layoutArray[0].ToString();

        for (int i = 1; i < layoutArray.Length; i++)
        {
            layout = layout + "," + layoutArray[i];
        }
    }
}