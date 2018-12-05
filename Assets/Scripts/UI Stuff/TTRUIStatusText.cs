using UnityEngine;
using UnityEngine.UI;

public class TTRUIStatusText : MonoBehaviour {
    public static GameObject Create(string message) {
        GameObject master = GameObject.FindGameObjectWithTag("ui/statustext");
        // could position messages underneath existing messages but just clearing them is easier
        foreach (Transform child in master.transform) {
            Destroy(child.gameObject);
        }

        // Positioning ui elements remains my least favorite part about Unity and whoever
        // devised it should spend the rest of their life cleaning toilets in the subway station.
        GameObject nova = Instantiate(Resources.Load<GameObject>("PFStatusText"), new Vector3(320f, 0f, 0f)/*Vector3.zero*/, Quaternion.identity);
        nova.transform.SetParent(master.transform, false);
        nova.GetComponent<Text>().text = message;

        return nova;
    }

    private Text uiText;
    private float t0;

    private const float tfade = 1f;
    private const float tduration = 5f;
    private const float maxAlpha =1f;

    private void Awake() {
        uiText = GetComponent<Text>();
        t0 = Time.time;
    }

    private void Update() {
        float t = Time.time - t0;
        
        Color tc = uiText.color;
        Color oc = GetComponent<Outline>().effectColor;
        
        if (t < tfade) {
            tc.a = t / tfade * maxAlpha;
            oc.a = t / tfade * maxAlpha;
        } else if (t < (tduration + tfade)) {
            tc.a = maxAlpha;
            oc.a = maxAlpha;
        } else if (t < (tduration + 2 * tfade)) {
            t = t - (tduration + tfade);
            tc.a = 1.0f - t / tfade * maxAlpha;
            oc.a = 1.0f - t / tfade * maxAlpha;
        } else {
            Destroy(gameObject);
        }

        uiText.color = tc;
        GetComponent<Outline>().effectColor = oc;
    }
}
