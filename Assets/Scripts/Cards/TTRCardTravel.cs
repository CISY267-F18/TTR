using System.Collections.Generic;
using UnityEngine;

public class TTRCardTravel : MonoBehaviour {
    private TTRNode source;
    private TTRNode destination;
    private int points;

    private GameObject front;
    private GameObject back;

    private bool isRevealed;

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

        ct.Pending = false;
        ct.Owner = null;

        return ct;
    }

    public void OnMouseUpAsButton() {
        // the checks have to bein this order because this is spaghetti

        // if youve already committed to drawing from the train card deck, you should finish doing that
        if (!TTRBoard.me.Active.FirstDraw) {
            return;
        }

        if (Owner == TTRBoard.me.Active) {
            if (!Owner.EvaluatedTravelCards) {
                Owner.EvaluatedTravelCards = true;
                Owner.RemoveTravelCard(this);
                Owner.PositionMyCards(true);

                // on discard, owner gets set to null, so if you need to do anything with the
                // owner do it before you discard the card
                Discard();
                TTRUIBlocking.CancelBlockTicketClaim();
                TTRBoard.me.Next();
                return;
            }
        }
        // if the card is available for claim, claim it, discard the other(s) and continue
        if (Pending) {
            TTRBoard.me.Active.GrantTravelCard(this);
            MoveTo(TTRBoard.me.pactive.tickets);
            TTRUIBlocking.Unblock();
            TTRBoard.me.Next();
            return;
        }
        if (TTRUIBlocking.IsBlocked()) {
            return;
        }
        if (Owner != null) {
            return;
        }

        if (!TTRBoard.me.DeckTravelCards.Has()) {
            TTRBoard.me.DeckTravelCards.Reassemble();
        }

        TTRCardTravel[] drawn = new TTRCardTravel[Mathf.Min(TTRBoard.me.DeckTravelCards.Size(), 3)];
        for (int i = 0; i < drawn.Length; i++) {
            drawn[i] = TTRBoard.me.DeckTravelCards.Draw();
        }

        TTRUIBlocking.BlockTicketClaim("Choose a travel card.", drawn);
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
        if (Owner != null) {
            throw new System.Exception("tried to claim a card that's already claimed by "+Owner.name);
        }
        Owner = claimant;
        if (claimant == TTRBoard.me.Active) {

        } // i don't know if it's possible to do this otherwise
    }

    public void MoveTo(Transform destination) {
        MoveTo(destination.position, destination.rotation, destination.localScale);
    }

    public void MoveTo(Vector3 position, Quaternion rotation, Vector3 scale) {
        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = scale;
    }

    public void Discard() {
        Owner = null;
        Revealed = false;
        Pending = false;
        TTRBoard.me.DeckTravelCardDiscard.AddCard(this);
        MoveTo(TTRBoard.me.pdecks.traveldiscard);
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

    public Texture Texture {
        get {
            return front.GetComponent<MeshRenderer>().material.mainTexture;
        }
        set {

        }
    }

    public bool Pending {
        get;
        set;
    }

    public TTRPlayer Owner {
        get;
        private set;
    }
}
