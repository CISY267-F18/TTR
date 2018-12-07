using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTRConnectionClickable : MonoBehaviour {
    private void OnMouseUpAsButton() {
        TTRBoard.me.Active.AttemptToBuild(transform.parent.GetComponent<TTRConnection>());
    }
}
