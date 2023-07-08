using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class Actor : GridObject
{
    public DialogueBox dialogueBox;

    protected override void Initialize() {
        base.Initialize();
        if (stageObject != null) {
            stageObject.onAdvance.AddListener(HideDialogue);
        }
    }

    protected void HideDialogue() {
        dialogueBox.gameObject.SetActive(false);
    }
    
    protected readonly string variableMatch = @"\$\w+";

    protected string GetVariablesFromDialogue(string dialogue) {
        return Regex.Replace(dialogue, variableMatch, delegate (Match m) {
            var variableName = m.Value.Replace("$", "");
            return stageObject.GetVariable(variableName);
        });
    }

    public void Talk(string direction, string dialogue) {
        dialogueBox.gameObject.SetActive(true);

        var variableReplaced = GetVariablesFromDialogue(dialogue);
        dialogueBox.ShowText(variableReplaced);

        Vector3Int dir = GetDirectionFromString(direction);
        if (stageObject.TryGetAdjacent(this, dir, out GridObject result)) {

        }
    }
}
