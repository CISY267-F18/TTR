using System.Collections.Generic;
using UnityEngine;

public class TTRBoard : MonoBehaviour {
    public static TTRBoard me = null;

    public GameObject prefabNode;
    public GameObject prefabConnection;

    private HashSet<TTRNode> nodes;

    private void Awake() {
        if (me != null) {
            throw new System.Exception("please don't spawn multiple boards");
        }
        me = this;
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

        first.AddOutboundNode(second, Color.red);
        first.AddOutboundNode(third, Color.blue);
        third.AddOutboundNode(fourth, Color.green);
        third.AddOutboundNode(fifth, Color.blue);
        fourth.AddOutboundNode(fifth, Color.blue);
        fifth.AddOutboundNode(sixth, Color.red);
        fifth.AddOutboundNode(seventh, Color.yellow);
        sixth.AddOutboundNode(seventh, Color.gray);

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
