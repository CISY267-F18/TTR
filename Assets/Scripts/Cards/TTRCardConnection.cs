using UnityEngine;

public class TTRCardConnection : MonoBehaviour {
    private TTRNode source;
    private TTRNode destination;
    private int points;

    public static TTRCardConnection Spawn(TTRNode source, TTRNode destination, int points) {
        TTRCardConnection cc = new GameObject(source.name + "-" + destination.name).AddComponent<TTRCardConnection>();
        cc.source = source;
        cc.destination = destination;
        cc.points = points;

        return cc;
    }
}
