using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    public float writeSpeed = 0.01f;

    TextMeshPro text;
    // Start is called before the first frame update
    void OnEnable()
    {
        text = GetComponentInChildren<TextMeshPro>();
    }

    IEnumerator Display(string textToWrite) {
        foreach (char c in textToWrite) {
            text.text += c;
            yield return new WaitForSeconds(writeSpeed);
        }
    }

    IEnumerator runningDisplay;

    public void ShowText(string textToShow) {
        if (runningDisplay != null) {
            StopCoroutine(runningDisplay);
        }
        text.text = "";
        runningDisplay = Display(textToShow);
        StartCoroutine(runningDisplay);
    }
}
