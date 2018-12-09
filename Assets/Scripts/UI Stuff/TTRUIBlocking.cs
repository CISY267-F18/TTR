using UnityEngine;
using UnityEngine.UI;

public class TTRUIBlocking : MonoBehaviour {
    private static GameObject blocking;

    private static Vector3 CENTER;

    void Awake() {
        CENTER = GameObject.FindGameObjectWithTag("center").transform.position;

        // this has to go at the end because you can't find deactivate game objects
        blocking = GameObject.FindGameObjectWithTag("ui/semidark");
        blocking.SetActive(false);
    }

    public static void Block(string message, TTRCardTravel[] tc) {
        if (tc == null) {
            TTRUIStatusText.Create("No travel cards to set!");
            return;
        }

        blocking.SetActive(true);

        GameObject.FindGameObjectWithTag("ui/semidark/text").GetComponent<Text>().text=message;

        int half=(int)(tc.Length/2);
        float separation=5f;

        for (int i = 0; i < tc.Length; i++) {
            TTRCardTravel card = tc[i];

            card.Revealed = true;
            card.MoveTo(new Vector3(CENTER.x - separation * (i - half), CENTER.y, CENTER.z), Quaternion.Euler(0f, 0f, 0f));
        }

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
        blocking.SetActive(false);

        GameObject[] textlabels = GameObject.FindGameObjectsWithTag("cityname");

        foreach (GameObject label in textlabels) {
            label.SetActive(false);
        }
    }

    public static bool IsBlocked() {
        return blocking.activeInHierarchy;
    }
}
