using System.Collections.Generic;
using UnityEngine;

public class TTRCardTravel : MonoBehaviour {
    private TTRNode source;
    private TTRNode destination;
    private int points;

    private GameObject front;
    private GameObject back;

    private bool isRevealed;
    private TTRPlayer owner;

    protected static Dictionary<string, Texture2D> cardTextures = new Dictionary<string, Texture2D>();

    public static TTRCardTravel Spawn(TTRNode source, TTRNode destination, int points) {
        TTRCardTravel ct = Instantiate(TTRBoard.me.prefabCard).AddComponent<TTRCardTravel>();
        ct.name = source.name + "-" + destination.name;
        ct.source = source;
        ct.destination = destination;
        ct.points = points;

        ct.fetchSides();
        ct.SetCardTexture();
        ct.Revealed = false;

        return ct;
    }

    private void OnMouseUpAsButton() {
        if (TTRUIBlocking.IsBlocked()) {
            return;
        }
    }

    protected void fetchSides() {
        foreach (Transform t in transform.GetComponentsInChildren<Transform>()) {
            if (t.name.Equals("Front")) {
                front = t.gameObject;
            }
            else if (t.name.Equals("Back")) {
                back = t.gameObject;
            }
        }
    }

    protected void SetCardTexture() {
        string key = (source.name + destination.name).Replace(" ", "");

        if (!cardTextures.ContainsKey(key)) {
            cardTextures.Add(key, Resources.Load<Texture2D>("Cards/Travel/" + key));
        }

        Material mtcard = front.GetComponent<MeshRenderer>().material;
        mtcard.mainTexture = cardTextures[key];

        SetCardBack();
    }

    protected void SetCardBack() {
        string key = "Back";

        if (!cardTextures.ContainsKey(key)) {
            cardTextures.Add(key, Resources.Load<Texture2D>("Cards/Travel/" + key));
        }

        Material mtcard = back.GetComponent<MeshRenderer>().material;
        mtcard.mainTexture = cardTextures[key];
    }

    public void Claim(TTRPlayer claimant) {
        if (owner != null) {
            throw new System.Exception("tried to claim a card that's already claimed");
        }
        owner = claimant;
        if (claimant == TTRBoard.me.Active) {

        } // i don't know if it's possible to do this otherwise
    }

    public void MoveTo(Transform destination) {
        transform.position = destination.position;
        transform.rotation = destination.rotation;
    }

    public void Discard() {
        owner = null;
        TTRBoard.me.DeckTravelCards.AddCardToBottom(this);
    }

    public bool Revealed {
        get {
            return isRevealed;
        }
        set {
            isRevealed = value;
            front.SetActive(isRevealed);
            back.SetActive(!isRevealed);
        }
    }
}
