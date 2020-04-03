using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[AddComponentMenu("")]
public class CardManager : UdonSharpBehaviour
{
    public Manager manager;
    public GameObject cardPrefab;
    public GameObject smallCardPrefab;
    public GameObject selectionIndicatorPrefab;
    public Canvas agentView;
    public Canvas operatorView;
    public LayoutManager layoutManager;

    private GameObject[] agentCards;
    private GameObject[] operatorCards;
    private GameObject selectionIndicator;
    private int blueHits;
    private int redHits;

    [UdonSynced]
    public int awaitCardID;
    private int cardID;

    [UdonSynced]
    public string asyncCardEvent;
    private string cardEvent;

    private bool awaitWordsUpdate;

    private float update;

    public void SetCardEvent(string incEvnt)
    {
        Debug.Log("Emitting card event: " + incEvnt);
        asyncCardEvent = incEvnt;
    }

    public void Start()
    {
        reset();
    }

    public void Update()
    {
        update += Time.deltaTime;
        if (update > 0.5f)
        {
            update = 0f;
            if (asyncCardEvent != cardEvent)
            {
                Debug.Log("Event has shown up: " + asyncCardEvent);
                string[] evnt = asyncCardEvent.Split(',');
                cardEvent = asyncCardEvent;
                switch (evnt[0])
                {
                    case "MakeAllCards":
                        string[] words = new string[evnt.Length - 1];
                        for (int i = 1; i < evnt.Length; i++)
                        {
                            words[i - 1] = evnt[i];
                        }
                        MakeAllCards(words);
                        break;
                    case "MoveCursor":
                        MoveCursor(int.Parse(evnt[1]));
                        break;
                    case "SetCardBlue":
                        setCardBlue(agentCards[int.Parse(evnt[1])]);
                        break;
                    case "SetCardRed":
                        setCardRed(agentCards[int.Parse(evnt[1])]);
                        break;
                    case "SetCardGrey":
                        setCardGrey(agentCards[int.Parse(evnt[1])]);
                        break;
                }
            }
        }
    }

    public void Setup()
    {
        Debug.Log("Setting up card manager");
        this.reset();
        this.createCards();
    }

    public void Teardown()
    {
        this.reset();

        if (selectionIndicator != null)
        {
            Destroy(selectionIndicator);
            selectionIndicator = null;
        }
    }

    public void BlueHit()
    {
        Debug.Log("Blue hit");
        this.blueHits = this.blueHits + 1;
        this.checkIfGameWon();
    }

    public void RedHit()
    {
        Debug.Log("Red hit");
        this.redHits = this.redHits + 1;
        this.checkIfGameWon();
    }

    public void CivilianHit()
    {
        Debug.Log("Civilian hit");
    }

    public void MoveCursor(int cardID)
    {
        Debug.Log("Moving cursor!");
        GameObject card = this.agentCards[cardID];
        if (this.selectionIndicator == null)
        {
            Debug.Log("Selector is null: instantiating");
            this.selectionIndicator = VRCInstantiate(this.selectionIndicatorPrefab);
        }

        this.selectionIndicator.transform.SetParent(agentView.transform);

        RectTransform transform = this.selectionIndicator.GetComponent<RectTransform>();
        transform.localRotation = new Quaternion();
        transform.anchoredPosition3D = new Vector3((float)(cardID % 5 * transform.sizeDelta.x + 0.5 * transform.sizeDelta.x), (float)(-cardID / 5 * transform.sizeDelta.y + -0.5 * transform.sizeDelta.y), 0);
    }

    public void DestroyCursor()
    {
        Destroy(this.selectionIndicator);
        this.selectionIndicator = null;
    }

    private void setCardBlue(GameObject card)
    {
        card.GetComponent<Image>().color = new Color(0, 0, 255, 255);
    }


    private void setCardRed(GameObject card)
    {
        card.GetComponent<Image>().color = new Color(255, 0, 0, 255);
    }


    private void setCardGrey(GameObject card)
    {
        card.GetComponent<Image>().color = new Color(120, 120, 120, 255);
    }

    private void setCardBlack(GameObject card)
    {
        card.GetComponent<Image>().color = new Color(0, 0, 0, 255);
    }

    private void reset()
    {
        awaitCardID = -1;
        cardID = -1;
        if (this.agentCards != null && this.operatorCards != null)
        {
            for (int i = 0; i < this.agentCards.Length; i++)
            {
                if (this.agentCards != null)
                {
                    Destroy(this.agentCards[i]);
                }
                if (this.operatorCards[i] != null)
                {
                    Destroy(this.operatorCards[i]);
                }
            }
        }

        this.agentCards = new GameObject[25];
        this.operatorCards = new GameObject[25];
        this.blueHits = 0;
        this.redHits = 0;
    }

    private string[] possibleWords()
    {
        string wordStr = "Hollywood,Well,Foot,New York,Spring,Court,Tube,Point,Tablet,Slip,Date,Drill,Lemon,Bell,Screen,Fair,Torch,State,Match,Iron,Block,France,Australia,Limousine,Stream,Glove,Nurse,Leprechaun,Play,Tooth,Arm,Bermuda,Diamond,Whale,Comic,Mammoth,Green,Pass,Missile,Paste,Drop,Pheonix,Marble,Staff,Figure,Park,Centaur,Shadow,Fish,Cotton,Egypt,Theater,Scale,Fall,Track,Force,Dinosaur,Bill,Mine,Turkey,March,Contract,Bridge,Robin,Line,Plate,Band,Fire,Bank,Boom,Cat,Shot,Suit,Chocolate,Roulette,Mercury,Moon,Net,Lawyer,Satellite,Angel,Spider,Germany,Fork,Pitch,King,Crane,Trip,Dog,Conductor,Part,Bugle,Witch,Ketchup,Press,Spine,Worm,Alps,Bond,Pan,Beijing,Racket,Cross,Seal,Aztec,Maple,Parachute,Hotel,Berry,Soldier,Ray,Post,Greece,Square,Mass,Bat,Wave,Car,Smuggler,England,Crash,Tail,Card,Horn,Capital,Fence,Deck,Buffalo,Microscope,Jet,Duck,Ring,Train,Field,Gold,Tick,Check,Queen,Strike,Kangaroo,Spike,Scientist,Engine,Shakespeare,Wind,Kid,Embassy,Robot,Note,Ground,Draft,Ham,War,Mouse,Center,Chick,China,Bolt,Spot,Piano,Pupil,Plot,Lion,Police,Head,Litter,Concert,Mug,Vacuum,Atlantis,Straw,Switch,Skyscraper,Laser,Scuba Diver,Africa,Plastic,Dwarf,Lap,Life,Honey,Horseshoe,Unicorn,Spy,Pants,Wall,Paper,Sound,Ice,Tag,Web,Fan,Orange,Temple,Canada,Scorpion,Undertaker,Mail,Europe,Soul,Apple,Pole,Tap,Mouth,Ambulance,Dress,Ice Cream,Rabbit,Buck,Agent,Sock,Nut,Boot,Ghost,Oil,Superhero,Code,Kiwi,Hospital,Saturn,Film,Button,Snowman,Helicopter,Loch Ness,Log,Princess,Time,Cook,Revolution,Shoe,Mole,Spell,Grass,Washer,Game,Beat,Hole,Horse,Pirate,Link,Dance,Fly,Pit,Server,School,Lock,Brush,Pool,Star,Jam,Organ,Berlin,Face,Luck,Amazon,Cast,Gas,Club,Sink,Water,Chair,Shark,Jupiter,Copper,Jack,Platypus,Stick,Olive,Grace,Bear,Glass,Row,Pistol,London,Rock,Van,Vet,Beach,Charge,Port,Disease,Palm,Moscow,Pin,Washington,Pyramid,Opera,Casino,Pilot,String,Night,Chest,Yard,Teacher,Pumpkin,Thief,Bark,Bug,Mint,Cycle,Telescope,Calf,Air,Box,Mount,Thumb,Antarctica,Trunk,Snow,Penguin,Root,Bar,File,Hawk,Battery,Compound,Slug,Octopus,Whip,America,Ivory,Pound,Sub,Cliff,Lab,Eagle,Genius,Ship,Dice,Hood,Heart,Novel,Pipe,Himalayas,Crown,Round,India,Needle,Shop,Watch,Lead,Tie,Table,Cell,Cover,Czech,Back,Bomb,Ruler,Forest,Bottle,Space,Hook,Doctor,Ball,Bow,Degree,Rome,Plane,Giant,Nail,Dragon,Stadium,Flute,Carrot,Wake,Fighter,Model,Tokyo,Eye,Mexico,Hand,Swing,Key,Alien,Tower,Poison,Cricket,Cold,Knife,Church,Board,Cloak,Ninja,Olympus,Belt,Light,Death,Stock,Millionaire,Day,Knight,Pie,Bed,Circle,Rose,Change,Cap,Triangle";
        return wordStr.Split(',');
    }

    private void checkIfGameWon()
    {
        Debug.Log("Score is: BLUE " + blueHits + " and RED " + redHits);
        if (this.blueHits >= 9)
        {
            this.manager.BlueWin();
        }
        else if (this.redHits >= 8)
        {
            this.manager.RedWin();
        }
    }

    private void createCards()
    {
        int[] idxArr = new int[25];
        string[] possWords = possibleWords();
        for (int i = 0; i < idxArr.Length; i++)
        {
            int rnd = Random.Range(0, possWords.Length);
            string word = possWords[rnd];
            if (word == "")
            {
                i--;
                continue;
            }

            possWords[rnd] = "";
            idxArr[i] = rnd;
        }

        string unfinishedWordIdxs = idxArr[0].ToString();

        for (int i = 1; i < idxArr.Length; i++)
        {
            unfinishedWordIdxs = unfinishedWordIdxs + "," + idxArr[i].ToString();
        }

        asyncCardEvent = "MakeAllCards," + unfinishedWordIdxs;

        Debug.Log("Emitting: " + asyncCardEvent);
    }

    public void MakeAllCards(string[] wordsArray)
    {
        Debug.Log("Creating for words: " + wordsArray.Length);
        for (int i = 0; i < wordsArray.Length; i++)
        {
            string wordIdx = wordsArray[i];
            if (wordIdx == "") { break; }

            var agCard = this.createCard(this.agentView, this.cardPrefab, int.Parse(wordIdx), i);
            var opCard = this.createCard(this.operatorView, this.smallCardPrefab, int.Parse(wordIdx), i);

            this.agentCards[i] = agCard;
            this.operatorCards[i] = opCard;

            int type = this.layoutManager.CardType(i);

            if (type == 0)
            {
                this.setCardBlue(this.operatorCards[i]);
            }
            else if (type == 1)
            {
                this.setCardRed(this.operatorCards[i]);
            }
            else if (type == 2)
            {
                this.setCardGrey(this.operatorCards[i]);
            }
            else
            {
                this.setCardBlack(this.operatorCards[i]);
            }
        }

        Debug.Log("Done!");
    }

    private GameObject createCard(Canvas canvas, GameObject prefab, int cardIdx, int cardIndex)
    {

        string[] possWordsArr = possibleWords();
        string word = possWordsArr[cardIdx];

        GameObject card = VRCInstantiate(prefab);
        card.transform.SetParent(canvas.transform);
        RectTransform transform = card.GetComponent<RectTransform>();
        transform.anchoredPosition3D = Vector3.zero;
        transform.localRotation = new Quaternion();

        transform.anchoredPosition3D = new Vector3((float)(cardIndex % 5 * transform.sizeDelta.x + 0.5 * transform.sizeDelta.x), (float)(-cardIndex / 5 * transform.sizeDelta.y + -0.5 * transform.sizeDelta.y), 0);

        Text text = card.GetComponentInChildren<Text>();
        text.text = word;

        return card;
    }
}