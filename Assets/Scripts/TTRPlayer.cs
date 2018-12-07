using System.Collections.Generic;
using UnityEngine;

public class TTRPlayer : MonoBehaviour {
    TTRHand<TTRCardTrain> hand;
    TTRHand<TTRCardTravel> travel;

    private const byte MAX_FREE_TRAINS = 45;
    public byte freeTrains;
    
    private static List<TTRPlayer> allPlayers = new List<TTRPlayer>();

    void Awake() {
        allPlayers.Add(this);

        hand = new TTRHand<TTRCardTrain>();
        travel = new TTRHand<TTRCardTravel>();

        freeTrains = MAX_FREE_TRAINS;
    }

    public void GrantTrainCard(TTRCardTrain card) {
        hand.AddCard(card);
    }

    public void GrantTravelCard(TTRCardTravel card) {
        travel.AddCard(card);
    }

    public void Print() {
        Debug.Log(gameObject.name + "\n" +
            "\tHand cards: " + hand.Print() + "\n" +
            "\tTravel cards: " + travel.Print());
    }

    public void ActivateMyCards(bool animate = true) {
        // when activated, all cards emerge from the front of the queue at the top of the board.
        // you don't physically see them move around between turns (maybe a polish goal, though).
        Vector3[] positions = TTRBoard.me.pother.otherPositions;
        foreach (TTRCardTravel tc in travel.Contents) {
            tc.transform.position = positions[0];
        }
        foreach (TTRCardTrain tc in hand.Contents) {
            tc.transform.position = positions[0];
        }
        // if (animate) {
        if (false) {

        } else {
            TTRBoard board = TTRBoard.me;
            foreach (TTRCardTravel tc in travel.Contents) {
                tc.transform.position = board.pactive.tickets;
            }
            foreach (TTRCardTrain tc in hand.Contents) {
                tc.transform.position = board.pactive.colors[tc.Color];
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
            Vector3[] positions = TTRBoard.me.pother.otherPositions;
            foreach (TTRCardTravel tc in travel.Contents) {
                tc.transform.position = positions[positions.Length-1];
            }
            foreach (TTRCardTrain tc in hand.Contents) {
                tc.transform.position = positions[positions.Length-1];
            }
        }
    }

    public static void PositionAllCards(bool animate = true, TTRPlayer active = null) {
        for (var i = 0; i < allPlayers.Count; i++) {
            TTRPlayer player = allPlayers[i];
            if (player == active) {
                player.ActivateMyCards(animate);
            } else {
                player.RemoveMyCards(animate);
            }
        }
    }

    public bool CanBuild(TTRConnection connection) {
        if (connection.Owner != null) {
            return false;
        }

        Dictionary<string, int> each = TTRBoard.ColorCountMap();
        List<string> eachKeys = new List<string>(each.Keys);

        foreach (TTRCardTrain c in hand.Contents) {
            // rainbow cards count for each color
            if (c.Color.ToLower().Equals("rainbow")) {
                foreach (string cname in eachKeys) {
                    each[cname]++;
                }
            // not-rainbow cards only count once
            } else {
                each[c.Color]++;
            }
        }

        // gray connections can be built upon by any color (or rainbow)
        if (connection.ColorName.ToLower().Equals("free")) {
            foreach (string cname in each.Keys) {
                if (each[cname] >= connection.Distance) {
                    return true;
                }
            }
        // colored connections can only be built upon by their color
        } else {
            return each[connection.ColorName] >= connection.Distance;
        }

        return false;
    }
}
