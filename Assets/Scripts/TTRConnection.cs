using System.Collections.Generic;
using UnityEngine;

public class TTRConnection : MonoBehaviour {
    public Material matConnection;

    private TTRNode source;
    private TTRNode destination;
    private LineRenderer line;

    private List<GameObject> connectionNodes;

    private void Awake() {
        source = null;
        destination = null;
        line = null;
        Color = Color.gray;
        ColorName = "Free";

        connectionNodes = new List<GameObject>();

        Owner = null;
        Partner = null;
    }

    public void Offset(Vector3 offset) {
        line.SetPosition(0, line.GetPosition(0) + offset);
        line.SetPosition(1, line.GetPosition(1) + offset);

        foreach (GameObject clickable in connectionNodes) {
            Vector3 newPosition = clickable.transform.position + offset;
            clickable.transform.position = newPosition;
        }
    }

    public void Set(TTRNode source, TTRNode destination, string color, int distance) {
        ColorName = color;
        Color colorValue = TTRBoard.me.colorValue(color);

        this.source = source;
        this.destination = destination;

        float angle = TTRStatic.AngleBetweenD(source, destination);

        Vector3 offset1 = Vector3.zero;
        Vector3 offset2 = Vector3.zero;
        TTRConnection existing=source.ConnectsTo(destination);
        if (existing != null) {
            float direction1 = TTRStatic.AngleBetween(existing.Source(), existing.Destination()) + Mathf.PI / 2f;
            float direction2 = TTRStatic.AngleBetween(existing.Source(), existing.Destination()) - Mathf.PI / 2f;
            float offsetDistance = 0.25f;
            offset1 = new Vector3(offsetDistance * Mathf.Cos(direction1), -offsetDistance * Mathf.Sin(direction1), 0f);
            offset2 = new Vector3(offsetDistance * Mathf.Cos(direction2), -offsetDistance * Mathf.Sin(direction2), 0f);

            existing.Offset(offset1);

            existing.Partner = this;
            Partner = existing;
        }
        
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
            novaRenderer.material.color = colorValue;

            connectionNodes.Add(nova);
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
        line.material.color = Color.Lerp(colorValue, Color.black, 0.15f);

        Offset(offset2);
    }

    public void Build(TTRPlayer builder) {
        Owner = builder;

        Quaternion trainRotation=Quaternion.Euler(0f, 0f, TTRStatic.AngleBetweenD(source, destination));

        foreach (GameObject clickable in connectionNodes) {
            GameObject nova = Instantiate(TTRBoard.me.prefabBuiltConnection);
            nova.transform.rotation = trainRotation;
            nova.transform.SetParent(clickable.transform.parent.transform);

            nova.name = builder.name + "'s Train";

            nova.transform.position = clickable.transform.position;

            Material novaMat = nova.GetComponentInChildren<MeshRenderer>().material;
            novaMat.color = builder.ColorValue;
        }
    }

    public TTRNode Source() {
        return source;
    }

    public TTRNode Destination() {
        return destination;
    }

    public int Score() {
        return TTRBoard.pointValues[Distance];
    }

    public TTRPlayer Owner { get; private set; }

    public Color Color { get; private set; }

    public string ColorName { get; private set; }

    public int Distance {
        get {
            return connectionNodes.Count;
        }
    }

    public TTRConnection Partner {
        get;
        private set;
    }
}
