using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTRCardTrain : MonoBehaviour {
    protected Color color;

    public static void Spawn(Color color) {
        TTRCardTrain card = Instantiate(TTRBoard.me.prefabCard).GetComponent<TTRCardTrain>();
        card.SetColor(color);
    }

    public virtual bool IsUsable() {
        return true;
    }

    public void SetColor(Color color) {
        this.color = color;
        // todo cards aren't recolored, the different colors actually have different
        // faces, so put the sprites in a hashmap of Color or something
    }
}

public class TTRCardRainbowTrain : TTRCardTrain {

    // rainbow cards are always usable on any color
    public override bool IsUsable() {
        return true;
    }
}