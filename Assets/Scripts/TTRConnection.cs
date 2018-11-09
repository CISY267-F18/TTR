using UnityEngine;

public class TTRConnection : MonoBehaviour {
    public Material matConnection;

    private TTRNode source;
    private TTRNode destination;
    private LineRenderer line;
    private Color color;

    private void Awake() {
        source = null;
        destination = null;
        line = null;
        color = Color.gray;
    }

    public void Set(TTRNode source, TTRNode destination, Color color) {
        this.source = source;
        this.destination = destination;

        // we will probably want to do something fancier so that it's more obvious
        // that routes have different costs eventually, but this will do for now
        line = GetComponent<LineRenderer>();
        if (line == null) {
            line = gameObject.AddComponent<LineRenderer>();
        }

        line.positionCount = 2;
        line.SetPosition(0, source.transform.position);
        line.SetPosition(1, destination.transform.position);
        line.startWidth = 0.5f;
        line.endWidth = 0.5f;
        line.material = matConnection;
        line.material.color = color;
    }
}
