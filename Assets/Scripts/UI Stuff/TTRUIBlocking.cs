using UnityEngine;
using UnityEngine.UI;

public class TTRUIBlocking : MonoBehaviour {
    private static GameObject[] blocking;

    private static Transform CENTER;
    private static TTRCardTravel[] focused;

    void Awake() {
        CENTER = GameObject.FindGameObjectWithTag("center").transform;

        // this has to go at the end because you can't find deactivate game objects
        blocking = GameObject.FindGameObjectsWithTag("ui/semidark");
        foreach (GameObject thing in blocking) {
            thing.SetActive(false);
        }
    }

    public static void Block(string message, TTRCardTravel[] tc) {
        // Pretty sure this can never happen, since you can't do this if there are no cards to
        // be clicked on, but just in case
        if (tc.Length == 0) {
            TTRUIStatusText.Create("No travel cards to set!");
            return;
        }

        foreach (GameObject thing in blocking) {
            thing.SetActive(true);
        }

        GameObject.FindGameObjectWithTag("ui/semidark/text").GetComponent<Text>().text = message;

        float half = tc.Length / 2f;
        float separation=12f;

        for (int i = 0; i < tc.Length; i++) {
            TTRCardTravel card = tc[i];

            card.Revealed = true;
            card.Pending = true;
            card.MoveTo(new Vector3(CENTER.position.x - separation * (i - half + 0.5f), CENTER.position.y, CENTER.position.z), CENTER.rotation, CENTER.localScale);
        }

        focused = tc;

        // TextMeshes are drawn on top of everything else, even things that are in front of them,
        // because Unity apparentlyd doesn't know what the "3D" part of "3D Text" means.
        // So we just hide them while there's stuff at the forefront of the game because it's
        // way easier than fighting with Unity to do it "the right way," and nobody's going to
        // notice anyway.
        GameObject[] textlabels = GameObject.FindGameObjectsWithTag("cityname");

        foreach (GameObject label in textlabels) {
            label.SetActive(false);
        }
    }

    public static void Unblock() {
        foreach (GameObject thing in blocking) {
            thing.SetActive(false);
        }

        GameObject[] textlabels = GameObject.FindGameObjectsWithTag("cityname");

        foreach (GameObject label in textlabels) {
            label.SetActive(false);
        }

        foreach (TTRCardTravel card in focused) {
            if (card != null && card.Owner == null) {
                card.Pending = false;
                card.Discard();
                card.MoveTo(TTRBoard.me.pdecks.travel);
            }
        }
    }

    public static bool IsBlocked() {
        // theyre all handled together so just grab the first one
        return blocking[0].activeInHierarchy;
    }
}