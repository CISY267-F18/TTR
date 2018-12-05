using UnityEngine;
using UnityEngine.UI;

public class TTRUIStatusText : MonoBehaviour {
    public static GameObject Create(string message) {
        // Positioning ui elements remains my least favorite part about Unity and whoever
        // devised it should spend the rest of their life cleaning toilets in the subway station.
        GameObject nova = Instantiate(Resources.Load<GameObject>("PFStatusText"), Vector3.zero, Quaternion.identity);
        nova.transform.SetParent(GameObject.FindGameObjectWithTag("ui/statustext").transform, false);
        nova.GetComponent<Text>().text = message;

        return nova;
    }

    private Text uiText;
    private float t0;

    private const float tfade = 1f;
    private const float tduration = 10f;

    private void Awake() {
        uiText = GetComponent<Text>();
        t0 = Time.time;
    }

    private void Update() {
        float t = Time.time - t0;
        
        Color tc = uiText.color;
        Color oc = GetComponent<Outline>().effectColor;
        
        if (t < tfade) {
            tc.a = t / tfade;
            oc.a = t / tfade;
        } else if (t < (tduration + tfade)) {
            tc.a = 1.0f;
            oc.a = 1.0f;
        } else if (t < (tduration + 2 * tfade)) {
            t = t - (tduration + tfade);
            tc.a = 1.0f - t / tfade;
            oc.a = 1.0f - t / tfade;
        } else {
            Destroy(gameObject);
        }

        uiText.color = tc;
        GetComponent<Outline>().effectColor = oc;
    }
}
