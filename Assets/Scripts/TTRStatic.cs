using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

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
            if (line.Length > 0 && line[0] != '#') {
                output.Add(line.Split(new char[] { ',' }));
            }
        }
        
        return output;
    }

    public static List<string> ReadText(string file) {
        string[] lines = File.ReadAllText(file).Split(new char[] { '\n' });

        List<string> output = new List<string>();
        foreach (string line in lines) {
            if (line[0] != '#') {
                output.Add(line);
            }
        }

        return output;
    }

    public static int TextWidth(string message, Font font, int fontSize) {
        int len = 0;
        
        CharacterInfo characterInfo = new CharacterInfo();
        char[] arr = message.ToCharArray();

        foreach (char c in arr) {
            font.GetCharacterInfo(c, out characterInfo, fontSize);
            len = len + characterInfo.advance;
        }

        return len;
    }

    // "borrowed" this from a java project which "borrowed" this from somewhere else on the internet
    public static float AngleBetween(GameObject a, GameObject b) {
        float r = b.transform.position.x - a.transform.position.x;
        float u = b.transform.position.y - a.transform.position.y;
        if (Mathf.Abs(r) < Mathf.Epsilon && Mathf.Abs(u) < Mathf.Epsilon) {
            return 0.0f;
        }
        return (Mathf.Atan2(-u, r) + 2 * Mathf.PI) % (2 * Mathf.PI);
    }

    public static float AngleBetweenD(GameObject a, GameObject b) {
        return Mathf.Rad2Deg * AngleBetween(a, b);
    }
}