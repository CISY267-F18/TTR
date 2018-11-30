using System;
using System.Collections.Generic;
using UnityEngine;

public class TTRHand<T> where T : MonoBehaviour {
    private List<T> contents;

    public TTRHand() {
        contents = new List<T>();
    }

    public void AddCard(T card) {
        contents.Add(card);
        Rearrange();
    }

    public void RemoveCard(T card) {
        for (int i=0; i<contents.Count; i++) {
            if (contents[i].Equals(card)) {
                contents.RemoveAt(i);
                Rearrange();
                return;
            }
        }

        throw new Exception("Card that you tried to remove was not found in the hand");
    }

    public void Rearrange() {
        // graphically on the screen
    }

    public string Print() {
        string str = "[" + contents.Count + "] ";

        foreach (T thing in contents) {
            str = str + thing.name + ",";
        }

        return str;
    }
}
