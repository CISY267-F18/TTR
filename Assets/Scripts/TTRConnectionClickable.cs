using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTRConnectionClickable : MonoBehaviour {
    private void OnMouseUpAsButton() {
        //Debug.Log(transform.parent.name);
        TTRBoard.me.Active.Print();
        Debug.Log("Can build: "+TTRBoard.me.Active.CanBuild(transform.parent.GetComponent<TTRConnection>()));
    }
}
