using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TTRBoard : MonoBehaviour {
    public static TTRBoard me = null;

    public GameObject prefabNode;
    public GameObject prefabConnection;
    // remind me to make this some kind of trapezoidal model at some point in time so the
    // edges are shaded a bit and it looks less flat
    public GameObject prefabConnectionMarker;
    public GameObject prefabCard;
    public GameObject prefabBuiltConnection;

    public static Dictionary<int, int> pointValues;
    public static Dictionary<string, Color> colorValues;

    private List<TTRPlayer> players;
    private int playerCount;

    private Transform containerNodes;
    private Transform containerConnections;
    private Transform containerTrainCards;
    private Transform containerTravelCards;

    private Dictionary<string, TTRNode> nodes;
    private TTRDeckTravelCards deckTravelCards;
    private TTRDeckTravelCards deckTravelCardDiscard;
    private TTRDeckTrains deckTrainCards;
    private TTRDeckTrains deckCardTrainDiscard;

    private TTRCardTrain[] freeTrainCards;

    // game data
    private const string gdTravelRoutes = "Assets/Data/travelcards.csv";
    private const string gdConnections = "Assets/Data/connections.csv";
    private const string gdNodes = "Assets/Data/cities.csv";
    private const string gdSettings = "Assets/Data/settings.txt";
    private const string gdColors = "Assets/Data/colors.csv";
    private const string gdTrainCards = "Assets/Data/deck.csv";

    // board settings
    private float boardWidth;
    private float boardHeight;

    // some other settings
    private const int playerStartingCards = 18;
    private const int playerStartingTravelCardsMax = 3;
    private const int playerStartingTravelCardsMin = 2;

    private const int freeCardLimit = 5;

    // ui/screen stuff
    public PositionFaceup pfaceup;
    public PositionDecks pdecks;
    public PositionActivePlayer pactive;
    public PositionOtherPlayers pother;

    // debug stuff
    private readonly string[] debugNames = {
        "Alice",
        "Bob",
        "Charlie",
        "Delta",
        "Echo",
    };

    private void Awake() {
        // singleton?
        if (me != null) {
            throw new System.Exception("please don't spawn multiple boards");
        }
        me = this;

        // game data
        List<string[]> ccdata;

        playerCount = 5;

        containerNodes = new GameObject("all nodes go here").transform;
        containerConnections = new GameObject("all connections go here").transform;
        containerTravelCards = new GameObject("all travel cards go here").transform;
        containerTrainCards = new GameObject("all train cards go here").transform;

        pointValues = new Dictionary<int, int>();
        colorValues = new Dictionary<string, Color>();
        // I'm not sure if this is going to be useful in the end but I'll keep it for now
        nodes = new Dictionary<string, TTRNode>();

        pointValues.Add(1, 1);
        pointValues.Add(2, 2);
        pointValues.Add(3, 4);
        pointValues.Add(4, 7);
        pointValues.Add(5, 10);
        pointValues.Add(6, 15);

        /*
         * Board setup
         */

        ccdata = TTRStatic.ReadCSV(gdColors);
        foreach (string[] line in ccdata) {
            Color nc = new Color(float.Parse(line[1]), float.Parse(line[2]), float.Parse(line[3]));
            colorValues.Add(line[0].Trim(), nc);

        }

        List<string> settings = TTRStatic.ReadText(gdSettings);
        boardWidth = transform.localScale.x;
        boardHeight = transform.localScale.y;

        ccdata = TTRStatic.ReadCSV(gdNodes);
        foreach (string[] line in ccdata) {
            nodes.Add(line[0], Spawn(TranslateCoordinateX(float.Parse(line[1])), TranslateCoordinateY(float.Parse(line[2])),
                    line[0], TranslateCoordinateX(float.Parse(line[3])), TranslateCoordinateY(float.Parse(line[4]))).GetComponent<TTRNode>());
        }

        ccdata = TTRStatic.ReadCSV(gdConnections);
        foreach (string[] line in ccdata) {
            Connect(nodes[line[0]], nodes[line[1]], int.Parse(line[2]), line[3].Trim());
        }

        /*
         * screen positions
         */

        Transform[] cp = new Transform[5];
        for (int i = 0; i < 5; i++) {
            cp[i] = GameObject.FindGameObjectWithTag("screen/up/" + i).transform;
        }

        pfaceup = new PositionFaceup(cp);
        Dictionary<string, Transform> cpd = new Dictionary<string, Transform>();
        foreach (string value in colorValues.Keys) {
            try {
                cpd.Add(value, GameObject.FindGameObjectWithTag("screen/active/" + value.ToLower()).transform);
            }
            catch (System.Exception e) {
                Debug.Log("guess the tag " + "screen/active/" + value.ToLower() + " doesn't exist: " + e);
            }
        }

        pdecks = new PositionDecks(GameObject.FindGameObjectWithTag("screen/traincarddeck").transform,
            GameObject.FindGameObjectWithTag("screen/ticketdeck").transform,
            GameObject.FindGameObjectWithTag("screen/traincardiscard").transform,
            GameObject.FindGameObjectWithTag("screen/ticketdiscard").transform);

        pactive = new PositionActivePlayer(GameObject.FindGameObjectWithTag("screen/active/tickets").transform,
            GameObject.FindGameObjectWithTag("screen/active/trains").transform, cpd);
        cp = new Transform[playerCount - 1];
        for (var i = 0; i < playerCount - 1; i++) {
            cp[i] = GameObject.FindGameObjectWithTag("screen/other/" + i).transform;
        }
        pother = new PositionOtherPlayers(cp);

        /*
         * Deck(s)
         */

        deckTrainCards = containerTrainCards.gameObject.AddComponent<TTRDeckTrains>();

        ccdata = TTRStatic.ReadCSV(gdTrainCards);
        
        foreach (string[] line in ccdata) {
            int n = int.Parse(line[1]);
            if (line[0].Equals("Rainbow")) {
                for (var i = 0; i < n; i++) {
                    TTRCardTrain nova = TTRCardRainbowTrain.Spawn(line[0]);
                    nova.MoveTo(pdecks.train.transform);
                    deckTrainCards.AddCard(nova);
                }
            } else {
                for (var i = 0; i < n; i++) {
                    TTRCardTrain nova = TTRCardTrain.Spawn(line[0]);
                    nova.MoveTo(pdecks.train.transform);
                    deckTrainCards.AddCard(nova);
                }
            }
        }

        deckTrainCards.Shuffle();

        deckCardTrainDiscard = new GameObject("Deck: Train Card Discard").AddComponent<TTRDeckTrains>();
        deckCardTrainDiscard.gameObject.SetActive(false);
        
        deckTravelCards = containerTravelCards.gameObject.AddComponent<TTRDeckTravelCards>();

        deckTravelCardDiscard = new GameObject("Deck: Travel Card Discard").AddComponent<TTRDeckTravelCards>();

        ccdata = TTRStatic.ReadCSV(gdTravelRoutes);
        foreach (string[] line in ccdata) {
            TTRCardTravel nova = TTRCardTravel.Spawn(nodes[line[1]], nodes[line[2]], int.Parse(line[0]));
            nova.MoveTo(pdecks.travel.transform);
            deckTravelCards.AddCard(nova);
        }

        deckTravelCards.Shuffle();

        /*
         * player(s)
         */

        players = new List<TTRPlayer>();

        Setup(playerCount);
        
        // for the sake of making everything slightly easier when it comes to animating things,
        // all of the players' cards are where you would physically expect them to be on the board
        // if you were playing the real game, even though there's no other reason to do that
        TTRPlayer.PositionAllCards(false);

        freeTrainCards = new TTRCardTrain[freeCardLimit];
        DealFreeCards();

        BeginTurn(0);
    }

    private GameObject Spawn(float x, float y, string name, float textx, float texty) {
        GameObject created = Instantiate(prefabNode);
        Vector3 position = created.transform.position;
        position.x = x;
        position.y = y;
        created.transform.position = position;
        created.name = name;

        TextMesh text = created.GetComponentInChildren<TextMesh>();
        text.text = name;
        position = text.transform.position;
        position.x = textx;
        position.y = texty;
        text.transform.position = position;

        created.transform.parent = containerNodes;
        return created;
    }

    private GameObject Spawn(float x, float y, string name) {
        return Spawn(x, y, name, x, y);
    }

    private TTRConnection Connect(TTRNode source, TTRNode destination, int distance, string color) {
        TTRConnection connection = source.AddOutboundNode(destination, color, distance);

        connection.name = "Connection - " + source.name + ":" + destination.name + "[" + color + ":" + distance + "]";
        connection.transform.parent = containerConnections;

        return connection;
    }

    private float TranslateCoordinateY(float n) {
        return boardHeight / 2 - n * boardHeight;
    }

    private float TranslateCoordinateX(float n) {
        return n * boardWidth - boardWidth / 2;
    }

    private void Setup(int playerCount) {
        List<string> playerColorNames = new List<string>();
        playerColorNames.Add("Black");
        playerColorNames.Add("Blue");
        playerColorNames.Add("Green");
        playerColorNames.Add("Yellow");
        playerColorNames.Add("Red");

        foreach (TTRPlayer player in players) {
            Destroy(player);
        }
        players.Clear();

        for (var i = 0; i < playerCount; i++) {
            TTRPlayer nova = new GameObject(debugNames[i]).AddComponent<TTRPlayer>();
            int index = Random.Range(0, playerColorNames.Count - 1);
            nova.Color = playerColorNames[index];
            playerColorNames.RemoveAt(index);
            players.Add(nova);
        }

        foreach (TTRPlayer player in players) {
            for (var i=0; i<playerStartingCards; i++) {
                player.GrantTrainCard(deckTrainCards.Draw());
            }
        }

        // these will need to be separated later anyway
        foreach (TTRPlayer player in players) {
            for (var i=0; i<playerStartingTravelCardsMax; i++) {
                player.GrantTravelCard(deckTravelCards.Draw());
            }
        }
    }

    public Color colorValue(string name) {
        return colorValues[name];
    }

    private void BeginTurn(int index) {
        TTRUIStatusText.Create(players[index].name + " is now acting");

        ActiveIndex = index;
        TTRPlayer.PositionAllCards(true, players[index]);
    }

    public void Next() {
        BeginTurn(++ActiveIndex % players.Count);
    }

    public struct PositionDecks {
        public Transform train;
        public Transform travel;
        public Transform traindiscard;
        public Transform traveldiscard;
        public PositionDecks(Transform train, Transform travel, Transform traindiscard, Transform traveldiscard) {
            this.train = train;
            this.travel = travel;
            this.traindiscard = traindiscard;
            this.traveldiscard = traveldiscard;
        }
    }

    public struct PositionFaceup {
        public Transform[] cardPositions;
        public PositionFaceup(Transform[] cardPositions) {
            this.cardPositions = cardPositions;
        }
    }

    public struct PositionActivePlayer {
        public Transform tickets;
        public Transform trains;
        public Dictionary<string, Transform> colors;
        public PositionActivePlayer(Transform tickets, Transform trains, Dictionary<string, Transform> colors) {
            this.tickets = tickets;
            this.trains = trains;
            this.colors = colors;
        }
    }

    public struct PositionOtherPlayers {
        public Transform[] otherPositions;
        public PositionOtherPlayers(Transform[] otherPositions) {
            this.otherPositions = otherPositions;
        }
    }

    public TTRPlayer Active {
        get {
            return players[ActiveIndex];
        }
    }

    public int ActiveIndex {
        get;
        private set;
    }

    public static Dictionary<string, int> ColorCountMap() {
        Dictionary<string, int> each = new Dictionary<string, int>();

        foreach (string cname in colorValues.Keys) {
            each.Add(cname, 0);
        }

        return each;
    }

    public void DealFreeCards() {
        while (true) {
            int index = MissingFreeCards();
            if (index == -1) {
                break;
            }
            // if there are no more cards in the deck, re-shuffle the discard pile
            if (!deckTrainCards.Has()) {
                deckTrainCards.Reassemble();
            }
            // if there are no more cards in the deck after re-shuffling the discard
            // pile, just don't bother because there aren't any more cards
            if (!deckTrainCards.Has()) {
                break;
            }
            TTRCardTrain top = deckTrainCards.Draw();
            freeTrainCards[index] = top;
            top.MoveTo(pfaceup.cardPositions[index]);
            top.Revealed = true;
        }
        // if there are no more cards in either pile and there are three free rainbows,
        // continually discarding, re-shuffling and re-dealing will give you a lovely
        // StackOverflowException because it will never end
        if (RainbowCardCount() > 2 && (deckTrainCards.NonRainbowCount() + DeckTrainCardDiscard.NonRainbowCount()) > 2) {
            foreach (TTRCardTrain card in freeTrainCards) {
                card.Discard();
                RemoveFreeCard(card, false);
            }
            DealFreeCards();
        }
    }

    public void RemoveFreeCard(TTRCardTrain card, bool autodeal = true) {
        for (int i=0; i<freeTrainCards.Length; i++) {
            if (freeTrainCards[i] == card) {
                freeTrainCards[i] = null;
                if (autodeal) {
                    DealFreeCards();
                }
                return;
            }
        }
    }

    private int MissingFreeCards() {
        for (int i=0; i<freeTrainCards.Length; i++) {
            if (freeTrainCards[i] == null) {
                return i;
            }
        }

        return -1;
    }

    private int RainbowCardCount() {
        int n = 0;
        foreach (TTRCardTrain card in freeTrainCards) {
            if (card != null && card.Color.ToLower().Equals("rainbow")) {
                n++;
            }
        }

        return n;
    }

    public bool ThereAreAnyFreeNonRainbowCards() {
        return (freeTrainCards.Length - RainbowCardCount()) > 0;
    }

    public TTRDeckTrains DeckTrainCards {
        get {
            return deckTrainCards;
        }
        set {

        }
    }

    public TTRDeckTravelCards DeckTravelCards {
        get {
            return deckTravelCards;
        }
        set {

        }
    }

    public TTRDeckTrains DeckTrainCardDiscard {
        get {
            return deckCardTrainDiscard;
        }
        set {

        }
    }

    public TTRDeckTravelCards DeckTravelCardDiscard {
        get {
            return deckTravelCardDiscard;
        }
        set {

        }
    }
}
