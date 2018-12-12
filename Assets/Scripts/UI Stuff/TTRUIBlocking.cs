using UnityEngine;
using UnityEngine.UI;

public class TTRUIBlocking : MonoBehaviour {
    private static GameObject buttonTicketClaim;

    private static GameObject tint;

    private static Transform CENTER;
    private static TTRCardTravel[] focused;

    private static GameObject blockingText;

    private static GameObject[] ticketClaimStuff;

    void Awake() {
        CENTER = GameObject.FindGameObjectWithTag("center").transform;

        /*
         * ticket claim stuff
         */

        buttonTicketClaim = GameObject.FindGameObjectWithTag("ui/ticketclaim/button");
        ticketClaimStuff = GameObject.FindGameObjectsWithTag("ui/ticketclaim");

        SetActiveEach(ticketClaimStuff, true);

        /*
         * player select stuff
         */


        blockingText = GameObject.FindGameObjectWithTag("ui/text");
        blockingText.SetActive(false);
        // this has to go at the end because you can't find deactivate game objects
        tint = GameObject.FindGameObjectWithTag("ui/semidark");
    }

    // because Unity apparently can't call static methods from the button thing
    public void CancelBlockTicketClaimInstance() {
        CancelBlockTicketClaim();
    }

    public static void CancelBlockTicketClaim() {
        Unblock();

        GameObject[] textlabels = GameObject.FindGameObjectsWithTag("cityname");

        SetActiveEach(textlabels, true);

        foreach (TTRCardTravel card in focused) {
            if (card != null && card.Owner == null) {
                card.Pending = false;
                card.Discard();
                card.MoveTo(TTRBoard.me.pdecks.travel);
            }
        }

        blockingText.SetActive(false);
        SetActiveEach(ticketClaimStuff, false);

        TTRBoard.me.Active.EvaluatedTravelCards = true;
        TTRBoard.me.Active.PositionMyCards(true);
    }

    public static void BlockTicketClaim(string message, TTRCardTravel[] tc, bool showCancel = false) {
        // Pretty sure this can never happen, since you can't do this if there are no cards to
        // be clicked on, but just in case
        if (tc.Length == 0) {
            TTRUIStatusText.Create("No travel cards to set!");
            return;
        }

        SetActiveEach(ticketClaimStuff, true);

        Tint();

        blockingText.GetComponent<Text>().text = message;
        blockingText.SetActive(false);

        float half = tc.Length / 2f;
        float separation=12f;

        for (int i = 0; i < tc.Length; i++) {
            TTRCardTravel card = tc[i];

            card.Revealed = true;
            card.Pending = true;
            card.MoveTo(new Vector3(CENTER.position.x - separation * (i - half + 0.5f), CENTER.position.y, CENTER.position.z), CENTER.rotation, CENTER.localScale);
        }

        focused = tc;

        // normally once you decide to draw a ticket card you have to commit to it, but at the
        // beginning of the game things are slightly different
        buttonTicketClaim.SetActive(showCancel);

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
        // don't do anything specific in here, just remove the screen tint and
        // replace the city name labels
        Untint();

        GameObject[] textlabels = GameObject.FindGameObjectsWithTag("cityname");

        SetActiveEach(textlabels, true);
    }

    public static bool IsBlocked() {
        return tint.activeInHierarchy;
    }

    public static void Tint() {
        tint.SetActive(true);
    }

    public static void Untint() {
        tint.SetActive(false);
    }

    public static void SetActiveEach(GameObject[] objects, bool active) {
        foreach (GameObject thing in objects) {
            thing.SetActive(active);
        }
    }
}