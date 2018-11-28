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

    private Transform containerNodes;
    private Transform containerConnections;

    private Dictionary<string, TTRNode> nodes;
    private List<TTRCardConnection> deckConnections;

    // game data
    private const string gdConnections = "Assets/Data/travelcards.csv";
    private const string gdNodes = "Assets/Data/cities.csv";
    private const string gdSettings = "Assets/Data/settings.txt";

    private void Awake() {
        if (me != null) {
            throw new System.Exception("please don't spawn multiple boards");
        }
        me = this;

        containerNodes = new GameObject("all nodes go here").transform;
        containerConnections = new GameObject("all connections go here").transform;

        pointValues = new Dictionary<int, int>();
        // I'm not sure if this is going to be useful in the end but I'll keep it for now
        nodes = new Dictionary<string, TTRNode>();
        deckConnections = new List<TTRCardConnection>();

        pointValues.Add(1, 1);
        pointValues.Add(2, 2);
        pointValues.Add(3, 4);
        pointValues.Add(4, 7);
        pointValues.Add(5, 10);
        pointValues.Add(6, 15);

        List<string> settings = TTRStatic.ReadText(gdSettings);
        int ow = int.Parse(settings[0]);
        int oh = int.Parse(settings[1]);
        float ch = Camera.main.orthographicSize * 2f;
        float cw = ch * Camera.main.aspect;

        List<string[]> ccdata = TTRStatic.ReadCSV(gdNodes);
        foreach (string[] line in ccdata) {
            nodes.Add(line[0], Spawn(int.Parse(line[1])*cw/ow-cw/2, ch / 2-int.Parse(line[2])*ch/oh, line[0]).GetComponent<TTRNode>());
        }

        // This currently crashes, but only because the nodes that the game is trying
        // to pull out of the file don't exist yet (so next you should look up the
        // city-node connecions)
        ccdata = TTRStatic.ReadCSV(gdConnections);
        foreach (string[] line in ccdata) {
            deckConnections.Add(new TTRCardConnection(nodes[line[1]], nodes[line[2]], int.Parse(line[0])));
        }
    }

    private GameObject Spawn(float x, float y, string name) {
        GameObject created = Instantiate(prefabNode);
        Vector3 position = created.transform.position;
        position.x = x;
        position.y = y;
        created.transform.position = position;
        created.name = "Node" + name;
        created.GetComponentInChildren<TextMesh>().text = name;

        created.transform.parent = containerNodes;
        return created;
    }
}
