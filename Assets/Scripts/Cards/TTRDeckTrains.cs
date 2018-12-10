using System;
using System.Collections.Generic;
using UnityEngine;

public class TTRDeckTrains : MonoBehaviour {
    private List<TTRCardTrain> contents;
    private System.Random randomize;

	void Awake() {
        randomize = new System.Random();
        contents = new List<TTRCardTrain>();
    }

    public void AddCard(TTRCardTrain card) {
        contents.Add(card);
        card.transform.SetParent(this.transform);
    }

    public void Shuffle() {
        List<TTRCardTrain> randomized = new List<TTRCardTrain>();
        
        while (contents.Count > 0) {
            int n = randomize.Next(contents.Count - 1);
            randomized.Add(contents[n]);
            contents.RemoveAt(n);
        }

        contents = randomized;
    }

    public TTRCardTrain Top() {
        if (!Has()) {
            throw new Exception("tried to check the top of an empty deck");
        }
        
        return contents[contents.Count - 1];
    }

    public TTRCardTrain Draw() {
        if (!Has()) {
            throw new Exception("tried to draw from an empty deck");
        }

        int n = contents.Count - 1;
        TTRCardTrain card = contents[n];
        contents.RemoveAt(n);
        return card;
    }

    public bool Has() {
        return contents.Count > 0;
    }

    public void Reassemble() {
        TTRDeckTrains other = TTRBoard.me.DeckTrainCardDiscard;
        while (other.Has()) {
            AddCard(other.Draw());
        }

        Shuffle();
    }

    public int RainbowCount() {
        int n = 0;
        foreach (TTRCardTrain card in contents) {
            if (card.Color.ToLower().Equals("rainbow")) {
                n++;
            }
        }

        return n;
    }

    public int NonRainbowCount() {
        return contents.Count - RainbowCount();
    }
}
