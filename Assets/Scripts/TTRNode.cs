using System.Collections.Generic;
using UnityEngine;

public class TTRNode : MonoBehaviour {

    private List<TTRConnection> outbound;

    void Awake() {
        outbound = new List<TTRConnection>();
    }

    public TTRConnection AddOutboundNode(TTRNode destination, string color, int distance) {
        // creates a two-way link between this and destination, so you only
        // have to call it once for each pair

        TTRConnection connection = Instantiate(TTRBoard.me.prefabConnection).GetComponent<TTRConnection>();
        connection.Set(this, destination, color, distance);
        
        outbound.Add(connection);
        destination.outbound.Add(connection);

        return connection;
    }

    public TTRConnection ConnectsTo(TTRNode other) {
        foreach (TTRConnection connection in outbound) {
            if (connection.Source() == other || connection.Destination() == other) {
                return connection;
            }
        }

        return null;
    }
}
