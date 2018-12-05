using System.Collections.Generic;
using UnityEngine;

public class TTRPlayer : MonoBehaviour {
    TTRHand<TTRCardTrain> hand;
    TTRHand<TTRCardTravel> travel;

    private const byte MAX_FREE_TRAINS = 45;
    byte freeTrains;

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
        // if (animate) {
        if (false) {

        } else {

        }
    }

    public void RemoveMyCards(int index, bool animate = true) {
        // if (animate) {
        if (false) {

        } else {

        }
    }

    public static void PositionAllCards(bool animate = true, TTRPlayer active = null) {
        for (var i=0; i<allPlayers.Count; i++) {
            TTRPlayer player = allPlayers[i];
            if (player == active) {
                player.ActivateMyCards(animate);
            } else {
                player.RemoveMyCards(i, animate);
            }
        }
    }
}
