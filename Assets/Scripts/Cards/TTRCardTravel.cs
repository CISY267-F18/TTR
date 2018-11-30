using UnityEngine;

public class TTRCardTravel : MonoBehaviour {
    private TTRNode source;
    private TTRNode destination;
    private int points;

    public static TTRCardTravel Spawn(TTRNode source, TTRNode destination, int points) {
        TTRCardTravel ct = Instantiate(TTRBoard.me.prefabCard).AddComponent<TTRCardTravel>();
        ct.name = source.name + "-" + destination.name;
        ct.source = source;
        ct.destination = destination;
        ct.points = points;

        return ct;
    }
}
