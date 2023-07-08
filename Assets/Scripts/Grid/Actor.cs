using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : GridObject
{
    public DialogueBox dialogueBox;
    private void Start() {
        Initialize();

        if (stageObject != null) {
            stageObject.onAdvance.AddListener(HideDialogue);
        }
    }

    private void HideDialogue() {
        dialogueBox.gameObject.SetActive(false);
    }

    public void Talk(string direction, string dialogue) {
        dialogueBox.gameObject.SetActive(true);
        dialogueBox.ShowText(dialogue);
    }
}
