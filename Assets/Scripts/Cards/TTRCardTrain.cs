using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTRCardTrain : MonoBehaviour {
    protected Color color;

    public static TTRCardTrain Spawn(Color color) {
        TTRCardTrain card = Instantiate(TTRBoard.me.prefabCard).AddComponent<TTRCardTrain>();
        card.SetColor(color);

        return card;
    }

    public virtual bool IsUsable() {
        return true;
    }

    public void SetColor(Color color) {
        this.name = color.ToString();
        this.color = color;
        // todo cards aren't recolored, the different colors actually have different
        // faces, so put the sprites in a hashmap of Color or something
    }
}

public class TTRCardRainbowTrain : TTRCardTrain {
    public static new TTRCardRainbowTrain Spawn(Color color) {
        TTRCardRainbowTrain card = Instantiate(TTRBoard.me.prefabCard).AddComponent<TTRCardRainbowTrain>();
        card.SetColor(color);

        return card;
    }

    // rainbow cards are always usable on any color
    public override bool IsUsable() {
        return true;
    }
}