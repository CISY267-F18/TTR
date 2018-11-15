using UnityEngine;

public class TTRCardConnection {
    private TTRNode source;
    private TTRNode destination;
    private int points;

    public TTRCardConnection(TTRNode source, TTRNode destination, int points) {
        this.source = source;
        this.destination = destination;
        this.points = points;
    }
}
