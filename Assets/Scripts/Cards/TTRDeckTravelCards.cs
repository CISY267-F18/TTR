using System;
using System.Collections.Generic;
using UnityEngine;

public class TTRDeckTravelCards : MonoBehaviour {
    private List<TTRCardTravel> contents;
    private System.Random randomize;

    void Awake() {
        randomize = new System.Random();
        contents = new List<TTRCardTravel>();
    }

    public void AddCard(TTRCardTravel card) {
        contents.Add(card);
        card.transform.SetParent(this.transform);
    }

    public void AddCardToBottom(TTRCardTravel card) {
        contents.Insert(0, card);
        card.transform.SetParent(this.transform);
    }

    public void Shuffle() {
        List<TTRCardTravel> randomized = new List<TTRCardTravel>();

        while (contents.Count > 0) {
            int n = randomize.Next(contents.Count - 1);
            randomized.Add(contents[n]);
            contents.RemoveAt(n);
        }

        contents = randomized;
    }

    public TTRCardTravel Top() {
        if (contents.Count == 0) {
            throw new Exception("tried to check the top of an empty deck");
        }

        return contents[contents.Count - 1];
    }

    public TTRCardTravel Draw() {
        if (contents.Count == 0) {
            throw new Exception("tried to draw from an empty deck");
        }

        int n = contents.Count - 1;
        TTRCardTravel card = contents[n];
        contents.RemoveAt(n);
        return card;
    }

    public void FinalizeDeck() {
        foreach (TTRCardTravel card in contents) {
            card.enabled = false;
        }
    }
}
