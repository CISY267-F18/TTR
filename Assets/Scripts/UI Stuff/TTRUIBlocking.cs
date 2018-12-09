using UnityEngine;
using UnityEngine.UI;

public class TTRUIBlocking : MonoBehaviour {
    private static GameObject blocking;

    void Awake() {
        blocking = GameObject.FindGameObjectWithTag("ui/semidark");
        blocking.SetActive(false);
    }

    public static void Block(string message) {
        // sigh
        blocking.SetActive(true);

        GameObject.FindGameObjectWithTag("ui/semidark/text").GetComponent<Text>().text=message;
    }

    public static void Unblock() {
        blocking.SetActive(false);
    }

    public static bool IsBlocked() {
        return blocking.activeInHierarchy;
    }
}
