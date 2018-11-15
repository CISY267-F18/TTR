using System.Collections.Generic;
using UnityEngine;

public class TTRConnection : MonoBehaviour {
    public Material matConnection;

    private TTRNode source;
    private TTRNode destination;
    private LineRenderer line;
    private int distance;
    private Color color;

    private void Awake() {
        source = null;
        destination = null;
        line = null;
        distance = 1;
        color = Color.gray;
    }

    public void Set(TTRNode source, TTRNode destination, Color color, int distance) {
        this.source = source;
        this.destination = destination;
        this.distance = distance;

        float angle = TTRStatic.AngleBetweenD(source, destination);
        
        for (int i = 0; i < distance; i++) {
            GameObject nova = Instantiate(TTRBoard.me.prefabConnectionMarker);
            float f = ((i + 1) * 1.0f) / (distance + 1);
            float xx = (destination.transform.position.x - source.transform.position.x) * f + source.transform.position.x;
            float yy = (destination.transform.position.y - source.transform.position.y) * f + source.transform.position.y;

            nova.transform.position = new Vector3(xx, yy, -1.0f);
            nova.transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
            nova.transform.SetParent(transform);

            MeshRenderer novaRenderer = nova.GetComponent<MeshRenderer>();
            // need to make sure the renderer is instantiated, and the easiest way
            // to do that is to assign it to something
            novaRenderer.material = matConnection;
            novaRenderer.material.color = color;
        }
        // we will probably want to do something fancier so that it's more obvious
        // that routes have different costs eventually, but this will do for now
        line = GetComponent<LineRenderer>();
        if (line == null) {
            line = gameObject.AddComponent<LineRenderer>();
        }

        line.positionCount = 2;
        line.SetPosition(0, source.transform.position);
        line.SetPosition(1, destination.transform.position);
        line.startWidth = 0.15f;
        line.endWidth = 0.15f;
        line.material = matConnection;
        line.material.color = Color.Lerp(color, Color.black, 0.15f);
    }

    public int Score() {
        return TTRBoard.pointValues[distance];
    }
}
