﻿using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TTRStatic {
    // "borrowed" this from SpaceShmup which "borrowed" this from a java project
    // which "borrowed" this from somewhere else on the internet
    public static float AngleBetween(TTRNode source, TTRNode destination) {
        float r = destination.gameObject.transform.position.x - source.gameObject.transform.position.x;
        float u = destination.gameObject.transform.position.y - source.gameObject.transform.position.y;
        if (Mathf.Abs(r) < Mathf.Epsilon && Mathf.Abs(u) < Mathf.Epsilon) {
            return 0.0f;
        }
        return (Mathf.Atan2(u, r) + 2 * Mathf.PI) % (2 * Mathf.PI);
    }

    public static float AngleBetweenD(TTRNode source, TTRNode destination) {
        return Mathf.Rad2Deg * AngleBetween(source, destination);
    }

    public static List<string[]> ReadCSV(string csv) {
        string[] lines = File.ReadAllText(csv).Split(new char[] { '\n' });

        List<string[]> output = new List<string[]>();
        foreach (string line in lines) {
            if (line[0] != '#') {
                output.Add(line.Split(new char[] { ',' }));
            }
        }
        
        return output;
    }
}