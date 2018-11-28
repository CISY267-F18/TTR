using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TTRBoard : MonoBehaviour {
    public static TTRBoard me = null;

    public GameObject prefabNode;
    public GameObject prefabConnection;
    // remind me to make this some kind of trapezoidal model at some point in time so the
    // edges are shaded a bit and it looks less flag
    public GameObject prefabConnectionMarker;

    public static Dictionary<int, int> pointValues;
    public static Dictionary<string, Color> colorValues;

    private Transform containerNodes;
    private Transform containerConnections;

    private Dictionary<string, TTRNode> nodes;
    private List<TTRCardConnection> deckTravelCards;

    // game data
    private const string gdTravelRoutes = "Assets/Data/travelcards.csv";
    private const string gdConnections = "Assets/Data/connections.csv";
    private const string gdNodes = "Assets/Data/cities.csv";
    private const string gdSettings = "Assets/Data/settings.txt";
    private const string gdColors = "Assets/Data/colors.csv";

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
        deckTravelCards = new List<TTRCardConnection>();

        pointValues.Add(1, 1);
        pointValues.Add(2, 2);
        pointValues.Add(3, 4);
        pointValues.Add(4, 7);
        pointValues.Add(5, 10);
        pointValues.Add(6, 15);

        ccdata = TTRStatic.ReadCSV(gdColors);
        foreach (string[] line in ccdata) {
            colorValues.Add(line[0].Trim(), new Color(float.Parse(line[1]), float.Parse(line[2]), float.Parse(line[3])));
        }

        List<string> settings = TTRStatic.ReadText(gdSettings);
        int ow = int.Parse(settings[0]);
        int oh = int.Parse(settings[1]);
        float ch = Camera.main.orthographicSize * 2f;
        float cw = ch * Camera.main.aspect;

        ccdata = TTRStatic.ReadCSV(gdNodes);
        foreach (string[] line in ccdata) {
            nodes.Add(line[0], Spawn(int.Parse(line[1])*cw/ow-cw/2, ch / 2-int.Parse(line[2])*ch/oh, line[0]).GetComponent<TTRNode>());
        }
        
        ccdata = TTRStatic.ReadCSV(gdTravelRoutes);
        foreach (string[] line in ccdata) {
            deckTravelCards.Add(new TTRCardConnection(nodes[line[1]], nodes[line[2]], int.Parse(line[0])));
        }
        
        ccdata = TTRStatic.ReadCSV(gdConnections);
        foreach (string[] line in ccdata) {
            if (!nodes.ContainsKey(line[0])) {
                Debug.Log("Did not find: " + line[0]);
            }
            if (!nodes.ContainsKey(line[1])) {
                Debug.Log("Did not find: " + line[1]);
            }
            /*if (colorValues.ContainsKey(line[3].Trim())) {
                Debug.Log("Found color: " + line[3]);
            } else {
                Debug.Log("Did not find color: " + line[3]);
            }*/
            Connect(nodes[line[0]], nodes[line[1]], int.Parse(line[2]), colorValues[line[3].Trim()]);
        }
    }

    private GameObject Spawn(float x, float y, string name) {
        GameObject created = Instantiate(prefabNode);
        Vector3 position = created.transform.position;
        position.x = x;
        position.y = y;
        created.transform.position = position;
        created.name = name;
        created.GetComponentInChildren<TextMesh>().text = name;

        created.transform.parent = containerNodes;
        return created;
    }

    private TTRConnection Connect(TTRNode source, TTRNode destination, int distance, Color color) {
        TTRConnection connection = source.AddOutboundNode(destination, color, distance);

        connection.name = "Connection - " + source.name + ":" + destination.name + "[" + color + ":" + distance + "]";
        connection.transform.parent = containerConnections;

        return connection;
    }
}
