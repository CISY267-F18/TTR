using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

    private Transform containerNodes;
    private Transform containerConnections;

    private Dictionary<string, TTRNode> nodes;
    private TTRDeckTravelCards deckTravelCards;
    private TTRDeckTrains deckCardTrains;
    private TTRDeckTrains deckCardTrainDiscard;

    // game data
    private const string gdTravelRoutes = "Assets/Data/travelcards.csv";
    private const string gdConnections = "Assets/Data/connections.csv";
    private const string gdNodes = "Assets/Data/cities.csv";
    private const string gdSettings = "Assets/Data/settings.txt";
    private const string gdColors = "Assets/Data/colors.csv";
    private const string gdTrainCards = "Assets/Data/deck.csv";

    // board settings
    private int originalWidth;
    private int originalHeight;
    private float cameraWidth;
    private float cameraHeight;

    private void Awake() {
        if (me != null) {
            throw new System.Exception("please don't spawn multiple boards");
        }
        me = this;

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
            colorValues.Add(line[0].Trim(), new Color(float.Parse(line[1]), float.Parse(line[2]), float.Parse(line[3])));
        }

        List<string> settings = TTRStatic.ReadText(gdSettings);
        originalWidth = int.Parse(settings[0]);
        originalHeight = int.Parse(settings[1]);
        cameraHeight = Camera.main.orthographicSize * 2f;
        cameraWidth = cameraHeight * Camera.main.aspect;

        ccdata = TTRStatic.ReadCSV(gdNodes);
        foreach (string[] line in ccdata) {
            nodes.Add(line[0], Spawn(TranslateCoordinateX(int.Parse(line[1])), TranslateCoordinateY(int.Parse(line[2])),
                    line[0], TranslateCoordinateX(int.Parse(line[3])), TranslateCoordinateY(int.Parse(line[4]))).GetComponent<TTRNode>());
        }

        ccdata = TTRStatic.ReadCSV(gdConnections);
        foreach (string[] line in ccdata) {
            Connect(nodes[line[0]], nodes[line[1]], int.Parse(line[2]), colorValues[line[3].Trim()]);
        }

        /*
         * Deck(s)
         */

        deckCardTrains = new GameObject("Deck: Train Cards").AddComponent<TTRDeckTrains>();
        deckCardTrains.transform.SetParent(this.transform);

        ccdata = TTRStatic.ReadCSV(gdTrainCards);
        
        foreach (string[] line in ccdata) {
            int n = int.Parse(line[1]);
            if (line[0].Equals("Rainbow")) {
                for (var i = 0; i < n; i++) {
                    deckCardTrains.AddCard(TTRCardRainbowTrain.Spawn(colorValues[line[0]]));
                }
            } else {
                for (var i = 0; i < n; i++) {
                    deckCardTrains.AddCard(TTRCardTrain.Spawn(colorValues[line[0]]));
                }
            }
        }

        deckCardTrains.Shuffle();

        deckCardTrainDiscard = new GameObject("Deck: Train Card Discard").AddComponent<TTRDeckTrains>();
        deckCardTrainDiscard.transform.SetParent(this.transform);
        
        deckTravelCards = new GameObject("Deck: Travel Cards").AddComponent<TTRDeckTravelCards>();
        deckTravelCards.transform.SetParent(this.transform);

        ccdata = TTRStatic.ReadCSV(gdTravelRoutes);
        foreach (string[] line in ccdata) {
            deckTravelCards.AddCard(TTRCardConnection.Spawn(nodes[line[1]], nodes[line[2]], int.Parse(line[0])));
        }

        deckTravelCards.Shuffle();

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

    private TTRConnection Connect(TTRNode source, TTRNode destination, int distance, Color color) {
        TTRConnection connection = source.AddOutboundNode(destination, color, distance);

        connection.name = "Connection - " + source.name + ":" + destination.name + "[" + color + ":" + distance + "]";
        connection.transform.parent = containerConnections;

        return connection;
    }

    private float TranslateCoordinateY(float n) {
        return cameraHeight / 2 - n * cameraHeight / originalHeight;
    }

    private float TranslateCoordinateX(float n) {
        return n * cameraWidth / originalWidth - cameraWidth / 2;
    }

    private void ReassembleDeck() {
        deckCardTrains.Reassemble(deckCardTrainDiscard);
    }
}
