
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTRPlayer : MonoBehaviour {
    TTRHand<TTRCardTrain> hand;
    TTRHand<TTRCardTravel> travel;

    private const byte MAX_FREE_TRAINS = 45;
    byte freeTrains;

    void Awake() {
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
}
