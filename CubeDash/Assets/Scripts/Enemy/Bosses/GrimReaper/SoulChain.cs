using System.Collections;
using UnityEngine;

public class SoulChain : MonoBehaviour
{
    public Transform maskController;  // De mask controller, moet niet bewegen
    public float moveDuration = 0.5f; // Duur van de beweging (snappier)
    public float endPositionY = -11f; // De Y-positie waar de ketting naar toe moet bewegen
    public float returnPositionY = 18f; // De Y-positie waar de ketting weer terug naar moet bewegen

    private bool isMoving = false; // Om te controleren of de ketting aan het bewegen is
    private bool isWaitingForReturn = false;  // Een vlag om te wachten voor de terugbeweging

    // Deze functie wordt door de aanval aangeroepen om de ketting te laten bewegen
    public void MoveChain()
    {
        if (isMoving) return; // Als de ketting al beweegt, doe niets

        isMoving = true;

        // Beginpositie en eindpositie van de ketting in lokale coördinaten
        Vector3 startPosition = transform.localPosition;

        // Beweeg de ketting naar beneden
        LeanTween.moveLocalY(gameObject, endPositionY, moveDuration)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(OnMoveToLowComplete);  // Zodra de beweging klaar is, start de return beweging
    }

    private void OnMoveToLowComplete()
    {
        if (isWaitingForReturn)
        {
            // Als we wachten voor de terugbeweging, start deze dan pas na een vertraging
            StartCoroutine(WaitForReturn());
        }
        else
        {
            // Start onmiddellijk de terugbeweging
            StartReturn();
        }
    }

    private IEnumerator WaitForReturn()
    {
        // Wacht een beetje voordat we terug omhoog gaan
        yield return new WaitForSeconds(5f);  // Deze waarde kun je aanpassen om de tijd te regelen

        // Start de terugbeweging
        StartReturn();
    }

    private void StartReturn()
    {
        // Na het naar beneden bewegen, beweeg de ketting terug omhoog
        LeanTween.moveLocalY(gameObject, returnPositionY, moveDuration)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(OnReturnComplete);  // Roep de nieuwe functie aan om te vernietigen
    }

    // Nieuwe functie om de ketting te vernietigen na de terugbeweging
    private void OnReturnComplete()
    {
        // Vernietig de ketting na de terugbeweging
        Destroy(gameObject);
    }

    // Start de ketting met een wachttijd voor de terugbeweging
    public void SetWaitingForReturn(bool wait)
    {
        isWaitingForReturn = wait;
    }
}
