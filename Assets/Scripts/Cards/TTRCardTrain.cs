using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTRCardTrain : MonoBehaviour {
    protected Color color;

    private GameObject front;
    private GameObject back;

    private bool isRevealed;
    private TTRPlayer owner;

    protected static Dictionary<string, Texture2D> cardTextures = new Dictionary<string, Texture2D>();

    public static TTRCardTrain Spawn(string color) {
        TTRCardTrain card = Instantiate(TTRBoard.me.prefabCard).AddComponent<TTRCardTrain>();

        card.fetchSides();
        card.SetCardColor(color);
        card.Revealed = false;
        card.owner = null;

        return card;
    }
    
    void OnMouseUpAsButton() {
        if (TTRUIBlocking.IsBlocked()) {
            return;
        }
        if (owner == null) {
            TTRPlayer active = TTRBoard.me.Active;
            // if face up
            if (Revealed) {
                // if the first card you're drawing
                if (active.FirstDraw) {
                    active.GrantTrainCard(this);
                    active.PositionMyCards();

                    TTRBoard.me.RemoveFreeCard(this);

                    if (!this.Color.Equals("Rainbow")) {
                        if (TTRBoard.me.ThereAreAnyFreeNonRainbowCards()) {
                            active.FirstDrawExecute();
                        }
                    }

                    TTRBoard.me.Next();
                }
                // if the second card you're drawing
                else {
                    if (!this.Color.Equals("Rainbow")) {
                        active.GrantTrainCard(this);
                        active.PositionMyCards();

                        TTRBoard.me.RemoveFreeCard(this);
                        TTRBoard.me.Next();
                    }
                }
            // if draw pile
            } else {
                active.GrantTrainCard(this);
                active.PositionMyCards();

                TTRBoard.me.DeckTrainCards.Draw(/*this*/);

                // no players in their right mind will ever let this happen though
                if (active.FirstDraw) {
                    if (TTRBoard.me.ThereAreAnyFreeNonRainbowCards()) {
                        active.FirstDrawExecute();
                    } else {
                        TTRBoard.me.Next();
                    }
                } else {
                    TTRBoard.me.Next();
                }
            }
        } else {
            Debug.Log("owner: " + owner.name);
        }
    }

    public virtual bool IsUsable() {
        return true;
    }

    public string Color {
        get {
            return name;
        }
    }

    public Color ColorValue {
        get {
            return color;
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

    protected void SetCardColor(string color) {
        this.name = color.ToString();
        this.color = TTRBoard.me.colorValue(color);

        if (!cardTextures.ContainsKey(color)) {
            cardTextures.Add(color, Resources.Load<Texture2D>("Cards/Trains/" + color));
        }
        Material mtcard = front.GetComponent<MeshRenderer>().material;

        mtcard.mainTexture = cardTextures[color];

        SetCardBack();
    }

    protected void SetCardBack() {
        string key = "Back";

        if (!cardTextures.ContainsKey(key)) {
            cardTextures.Add(key, Resources.Load<Texture2D>("Cards/Trains/" + key));
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

    public void Unclaim() {
        owner = null;
    }

    public void Discard() {
        owner = null;
        Revealed = false;
        TTRBoard.me.DeckTrainCardDiscard.AddCard(this);
        MoveTo(TTRBoard.me.pdecks.traindiscard);
    }

    public void MoveTo(Transform destination) {
        MoveTo(destination.position, destination.rotation, destination.localScale);
    }

    public void MoveTo(Vector3 position, Quaternion rotation, Vector3 scale) {
        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = scale;
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

public class TTRCardRainbowTrain : TTRCardTrain {
    public static new TTRCardRainbowTrain Spawn(string color) {
        TTRCardRainbowTrain card = Instantiate(TTRBoard.me.prefabCard).AddComponent<TTRCardRainbowTrain>();

        card.fetchSides();
        card.SetCardColor(color);
        card.Revealed = false;

        return card;
    }

    // rainbow cards are always usable on any color
    public override bool IsUsable() {
        return true;
    }
}