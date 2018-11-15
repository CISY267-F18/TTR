using System.Collections.Generic;
using UnityEngine;

public class TTRBoard : MonoBehaviour {
    public static TTRBoard me = null;

    public GameObject prefabNode;
    public GameObject prefabConnection;
    // remind me to make this some kind of trapezoidal model at some point in time so the
    // edges are shaded a bit and it looks less flag
    public GameObject prefabConnectionMarker;

    public static Dictionary<int, int> pointValues = new Dictionary<int, int>();

    private Transform containerNodes;
    private Transform containerConnections;

    private HashSet<TTRNode> nodes;

    private void Awake() {
        if (me != null) {
            throw new System.Exception("please don't spawn multiple boards");
        }
        me = this;

        containerNodes = new GameObject("all nodes go here").transform;
        containerConnections = new GameObject("all connections go here").transform;
        
        pointValues.Add(1, 1);
        pointValues.Add(2, 2);
        pointValues.Add(3, 4);
        pointValues.Add(4, 7);
        pointValues.Add(5, 10);
        pointValues.Add(6, 15);
    }

    void Start () {
        // I'm not sure if this is going to be useful in the end but I'll keep it for now
        nodes = new HashSet<TTRNode>();

        TTRNode first = Spawn(-4, -4, "first").GetComponent<TTRNode>();
        TTRNode second = Spawn(0, -4, "second").GetComponent<TTRNode>();
        TTRNode third = Spawn(0, 0, "third").GetComponent<TTRNode>();
        TTRNode fourth = Spawn(-4, 0, "fourth").GetComponent<TTRNode>();
        TTRNode fifth = Spawn(4, 0, "fifth").GetComponent<TTRNode>();
        TTRNode sixth = Spawn(-4, 4, "sixth").GetComponent<TTRNode>();
        TTRNode seventh = Spawn(4, 4, "seventh").GetComponent<TTRNode>();

        first.transform.SetParent(containerNodes);
        second.transform.SetParent(containerNodes);
        third.transform.SetParent(containerNodes);
        fourth.transform.SetParent(containerNodes);
        fifth.transform.SetParent(containerNodes);
        sixth.transform.SetParent(containerNodes);
        seventh.transform.SetParent(containerNodes);

        first.AddOutboundNode(second, Color.red, 2).transform.SetParent(containerConnections);
        first.AddOutboundNode(third, Color.blue, 2).transform.SetParent(containerConnections);
        third.AddOutboundNode(fourth, Color.green, 1).transform.SetParent(containerConnections);
        third.AddOutboundNode(fifth, Color.blue, 1).transform.SetParent(containerConnections);
        fifth.AddOutboundNode(sixth, Color.red, 4).transform.SetParent(containerConnections);
        fifth.AddOutboundNode(seventh, Color.yellow, 2).transform.SetParent(containerConnections);
        sixth.AddOutboundNode(seventh, Color.gray, 3).transform.SetParent(containerConnections);

        nodes.Add(first);
        nodes.Add(second);
        nodes.Add(third);
        nodes.Add(fourth);
        nodes.Add(fifth);
        nodes.Add(sixth);
        nodes.Add(seventh);
	}

    private GameObject Spawn(float x, float y, string name) {
        GameObject created = Instantiate(prefabNode);
        Vector3 position = created.transform.position;
        position.x = x;
        position.y = y;
        created.transform.position = position;

        return created;
    }
}
