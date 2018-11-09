using System.Collections.Generic;
using UnityEngine;

public class TTRNode : MonoBehaviour {

    private List<TTRConnection> outbound;

    void Awake() {
        outbound = new List<TTRConnection>();
    }

    public void AddOutboundNode(TTRNode destination, Color color) {
        // creates a two-way link between this and destination, so you only
        // have to call it once for each pair

        TTRConnection connection = Instantiate(TTRBoard.me.prefabConnection).GetComponent<TTRConnection>();
        connection.Set(this, destination, color);
        
        outbound.Add(connection);
        destination.outbound.Add(connection);
    }
}
