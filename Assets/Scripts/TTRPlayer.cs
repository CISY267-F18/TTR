using System.Collections.Generic;
using UnityEngine;

public class TTRPlayer : MonoBehaviour {
    TTRHand<TTRCardTrain> hand;
    TTRHand<TTRCardTravel> travel;

    private const int MAX_FREE_TRAINS = 45;
    public int freeTrains;
    
    private static List<TTRPlayer> allPlayers = new List<TTRPlayer>();
    
    void Awake() {
        allPlayers.Add(this);

        hand = new TTRHand<TTRCardTrain>();
        travel = new TTRHand<TTRCardTravel>();

        freeTrains = MAX_FREE_TRAINS;
    }

    public void GrantTrainCard(TTRCardTrain card) {
        hand.AddCard(card);
        card.Claim(this);
    }

    public void GrantTravelCard(TTRCardTravel card) {
        travel.AddCard(card);
        card.Claim(this);
    }

    public void Print() {
        Debug.Log(gameObject.name + "\n" +
            "\tHand cards: " + hand.Print() + "\n" +
            "\tTravel cards: " + travel.Print());
    }

    public void ActivateMe(bool animate = true) {
        FirstDraw = true;
        PositionMyCards(animate);
    }

    public void PositionMyCards(bool animate = true) {
        // when activated, all cards emerge from the front of the queue at the top of the board.
        // you don't physically see them move around between turns (maybe a polish goal, though).
        Transform[] positions = TTRBoard.me.pother.otherPositions;
        foreach (TTRCardTravel tc in travel.Contents) {
            tc.transform.position = positions[0].position;
        }
        foreach (TTRCardTrain tc in hand.Contents) {
            tc.transform.position = positions[0].position;
        }
        // if (animate) {
        if (false) {

        }
        else {
            TTRBoard board = TTRBoard.me;
            for (int i = 0; i < travel.Contents.Count; i++){
                TTRCardTravel tc=travel.Contents[i];
                Vector3 ticketPosition = board.pactive.tickets.position;
                tc.MoveTo(new Vector3(ticketPosition.x, ticketPosition.y + i, ticketPosition.z + i), board.pactive.tickets.rotation, board.pactive.tickets.localScale);
                tc.Revealed = true;
            }
            foreach (TTRCardTrain tc in hand.Contents) {
                tc.MoveTo(board.pactive.colors[tc.Color]);
                tc.Revealed = true;
            }
            // todo something with the note that says how many available trains you have remaining
        }
    }

    public void RemoveMyCards(bool animate = true) {
        // when removed, all cards go to the end of the queue at the top of the board.
        // you don't physically see them move around between turns (maybe a polish goal,
        // though).
        // if (animate) {
        if (false) {

        } else {
            Transform[] positions = TTRBoard.me.pother.otherPositions;
            foreach (TTRCardTravel tc in travel.Contents) {
                tc.MoveTo(positions[positions.Length - 1]);
                tc.Revealed = false;
            }
            foreach (TTRCardTrain tc in hand.Contents) {
                tc.MoveTo(positions[positions.Length - 1]);
                tc.Revealed = false;
            }
        }
    }

    public static void PositionAllCards(bool animate = true, TTRPlayer active = null) {
        for (var i = 0; i < allPlayers.Count; i++) {
            TTRPlayer player = allPlayers[i];
            if (player == active) {
                player.ActivateMe(animate);
            } else {
                player.RemoveMyCards(animate);
            }
        }
    }

    public bool AttemptToBuild(TTRConnection connection) {
        string validColorName = CanBuild(connection);
        if (validColorName!=null) {
            connection.Build(this);
            RemoveCardsOfColor(validColorName, connection.Distance);
            freeTrains = freeTrains - connection.Distance;
            TTRBoard.me.Next();
            return true;
        }

        return false;
    }

    // todo document the way this returns the color of the card that can build on
    // the connection, or null if one does not exist
    public string CanBuild(TTRConnection connection) {
        // has already drawn a card?
        if (!FirstDraw) {
            return null;
        }

        // already owned?
        if (connection.Owner != null) {
            TTRUIStatusText.Create("Already owned by " + connection.Owner.name + "!");
            return null;
        }

        // not enough trains?
        if (connection.Distance > freeTrains) {
            TTRUIStatusText.Create("Not enough trains!");
            return null;
        }

        // do you have the right cards?
        Dictionary<string, int> each = CardCount();

        // gray connections can be built upon by any color (or rainbow)
        if (connection.ColorName.ToLower().Equals("free")) {
            foreach (string cname in each.Keys) {
                if (each[cname] >= connection.Distance) {
                    // currently, returns a random color if there are multiple colors
                    // that will work; in the future the player should be allowed to
                    // choose
                    return cname;
                }
            }
            // colored connections can only be built upon by their color
        } else {
            if (each[connection.ColorName] >= connection.Distance) {
                return connection.ColorName;
            }
        }

        TTRUIStatusText.Create("Not enough train cards!");
        return null;
    }

    public void RemoveCardsOfColor(string color, int n) {
        List<TTRCardTrain> toRemove = new List<TTRCardTrain>();
        // seek out cards of exactly that color
        for (int i = 0; i < hand.Contents.Count; i++) {
            if (toRemove.Count >= n) {
                break;
            }
            if (hand.Contents[i].Color.Equals(color)) {
                toRemove.Add(hand.Contents[i]);
            }
        }
        // seek out rainbow cards, if you need them
        for (int i = 0; i < hand.Contents.Count; i++) {
            if (toRemove.Count >= n) {
                break;
            }
            if (hand.Contents[i].Color.ToLower().Equals("rainbow")) {
                toRemove.Add(hand.Contents[i]);
            }
        }
        // do the actual removal
        foreach (TTRCardTrain c in toRemove) {
            hand.RemoveCard(c);
        }
    }

    private Dictionary<string, int> CardCount() {
        Dictionary<string, int> each = TTRBoard.ColorCountMap();
        List<string> eachKeys = new List<string>(each.Keys);

        foreach (TTRCardTrain c in hand.Contents) {
            // rainbow cards count for each color
            if (c.Color.ToLower().Equals("rainbow")) {
                foreach (string cname in eachKeys) {
                    each[cname]++;
                }
                // not-rainbow cards only count once
            }
            else {
                each[c.Color]++;
            }
        }

        return each;
    }

    public string Color { get; set; }

    public Color ColorValue {
        get {
            return TTRBoard.me.colorValue(Color);
        }
    }

    public bool FirstDraw {
        get;
        private set;
    }

    public void FirstDrawExecute() {
        FirstDraw = false;
    }
}
