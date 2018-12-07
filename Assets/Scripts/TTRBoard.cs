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

    public static Dictionary<int, int> pointValues;
    public static Dictionary<string, Color> colorValues;

    private List<TTRPlayer> players;

    private Transform containerNodes;
    private Transform containerConnections;

    private Dictionary<string, TTRNode> nodes;
    private TTRDeckTravelCards deckTravelCards;
    private TTRDeckTrains deckTrainCards;
    private TTRDeckTrains deckCardTrainDiscard;

    private TTRPlayer myTurn;

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
    private const int playerStartingCards = 4;
    private const int playerStartingTravelCardsMax = 3;
    private const int playerStartingTravelCardsMin = 2;

    // ui/screen stuff
    private GameObject scrTicketDeck;
    private GameObject scrTrainCardDeck;

    public PositionFaceup pfaceup;
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

        /*
         * screen definitions
         */

        scrTicketDeck = GameObject.FindGameObjectWithTag("screen/ticketdeck");
        scrTrainCardDeck = GameObject.FindGameObjectWithTag("screen/traincarddeck");

        // game data
        List<string[]> ccdata;

        containerNodes = new GameObject("all nodes go here").transform;
        containerConnections = new GameObject("all connections go here").transform;

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
         * Deck(s)
         */

        deckTrainCards = scrTrainCardDeck.AddComponent<TTRDeckTrains>();

        ccdata = TTRStatic.ReadCSV(gdTrainCards);
        
        foreach (string[] line in ccdata) {
            int n = int.Parse(line[1]);
            if (line[0].Equals("Rainbow")) {
                for (var i = 0; i < n; i++) {
                    deckTrainCards.AddCard(TTRCardRainbowTrain.Spawn(line[0]));
                }
            } else {
                for (var i = 0; i < n; i++) {
                    deckTrainCards.AddCard(TTRCardTrain.Spawn(line[0]));
                }
            }
        }

        deckTrainCards.Shuffle();

        deckCardTrainDiscard = new GameObject("Deck: Train Card Discard").AddComponent<TTRDeckTrains>();
        deckCardTrainDiscard.gameObject.SetActive(false);
        
        deckTravelCards = scrTicketDeck.AddComponent<TTRDeckTravelCards>();

        ccdata = TTRStatic.ReadCSV(gdTravelRoutes);
        foreach (string[] line in ccdata) {
            deckTravelCards.AddCard(TTRCardTravel.Spawn(nodes[line[1]], nodes[line[2]], int.Parse(line[0])));
        }

        deckTravelCards.Shuffle();

        /*
         * player(s)
         */

        players = new List<TTRPlayer>();

        Setup(5);

        /*
         * screen positions
         */

        Vector3[] cp = new Vector3[5];
        for (int i = 0; i < 5; i++) {
            cp[i] = GameObject.FindGameObjectWithTag("screen/up/" + i).transform.position;
        }
        pfaceup = new PositionFaceup(cp);
        Dictionary<string, Vector3> cpd = new Dictionary<string, Vector3>();
        foreach (string value in colorValues.Keys) {
            try {
                cpd.Add(value, GameObject.FindGameObjectWithTag("screen/active/" + value.ToLower()).transform.position);
            } catch (System.Exception e) {
                // guess not
            }
        }
        pactive = new PositionActivePlayer(GameObject.FindGameObjectWithTag("screen/active/tickets").transform.position,
            GameObject.FindGameObjectWithTag("screen/active/trains").transform.position, cpd);
        cp = new Vector3[players.Count-1];
        for (var i=0; i<players.Count-1; i++) {
            cp[i] = GameObject.FindGameObjectWithTag("screen/other/" + i).transform.position;
        }
        pother = new PositionOtherPlayers(cp);
        
        // for the sake of making everything slightly easier when it comes to animating things,
        // all of the players' cards are where you would physically expect them to be on the board
        // if you were playing the real game, even though there's no other reason to do that
        TTRPlayer.PositionAllCards(false);

        BeginTurn(players[0]);
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

    private void ReassembleDeck() {
        deckTrainCards.Reassemble(deckCardTrainDiscard);
    }

    private void Setup(int playerCount) {
        foreach (TTRPlayer player in players) {
            Destroy(player);
        }
        players.Clear();

        for (var i = 0; i < playerCount; i++) {
            players.Add(new GameObject(debugNames[i]).AddComponent<TTRPlayer>());
        }

        foreach (TTRPlayer player in players) {
            for (var i=0; i<playerStartingCards; i++) {
                player.GrantTrainCard(deckTrainCards.Draw());
            }
        }

        foreach (TTRPlayer player in players) {
            for (var i=0; i<playerStartingTravelCardsMax; i++) {
                player.GrantTravelCard(deckTravelCards.Draw());
            }
        }
    }

    public Color colorValue(string name) {
        return colorValues[name];
    }

    private void BeginTurn(TTRPlayer player) {
        TTRUIStatusText.Create(player.name + " is now acting");

        myTurn = player;

        TTRPlayer.PositionAllCards(player);
    }

    public struct PositionFaceup {
        public Vector3[] cardPositions;
        public PositionFaceup(Vector3[] cardPositions) {
            this.cardPositions = cardPositions;
        }
    }

    public struct PositionActivePlayer {
        public Vector3 tickets;
        public Vector3 trains;
        public Dictionary<string, Vector3> colors;
        public PositionActivePlayer(Vector3 tickets, Vector3 trains, Dictionary<string, Vector3> colors) {
            this.tickets = tickets;
            this.trains = trains;
            this.colors = colors;
        }
    }

    public struct PositionOtherPlayers {
        public Vector3[] otherPositions;
        public PositionOtherPlayers(Vector3[] otherPositions) {
            this.otherPositions = otherPositions;
        }
    }

    public TTRPlayer Active {
        get {
            return myTurn;
        }
    }

    public static Dictionary<string, int> ColorCountMap() {
        Dictionary<string, int> each = new Dictionary<string, int>();

        foreach (string cname in colorValues.Keys) {
            each.Add(cname, 0);
        }

        return each;
    }
}
