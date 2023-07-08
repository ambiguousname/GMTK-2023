using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    public float writeSpeed = 0.01f;

    TextMeshPro text;
    Transform background;

    private Vector3 baseScale;
    // Start is called before the first frame update
    void OnEnable()
    {
        text = GetComponentInChildren<TextMeshPro>();
        background = GetComponentInChildren<SpriteRenderer>().transform;
        baseScale = background.localScale;   
    }

    IEnumerator Display(string textToWrite) {
        foreach (char c in textToWrite) {
            text.text += c;
            /*var x = baseScale.x;
            Vector2 scale = text.GetPreferredValues();
            if (scale.x > background.localScale.x) {
                x = scale.x;
            }

            var y = baseScale.y;
            if (scale.y > background.localScale.y * 2) {
                y = scale.y * 2;
            }
            background.localScale = new Vector3(x, y, baseScale.z);*/
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
