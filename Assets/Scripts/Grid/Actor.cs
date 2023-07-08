using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class Actor : GridObject
{
    public DialogueBox dialogueBox;
    public GameObject talkToIndicator;

    protected override void Initialize() {
        base.Initialize();
        if (stageObject != null) {
            stageObject.beforeAdvance.AddListener(HideDialogue);
        }
    }

    protected void HideDialogue() {
        dialogueBox.gameObject.SetActive(false);
        talkToIndicator.SetActive(false);
    }
    
    protected readonly string variableMatch = @"\$\w+";

    protected string GetVariablesFromDialogue(string dialogue) {
        return Regex.Replace(dialogue, variableMatch, delegate (Match m) {
            var variableName = m.Value.Replace("$", "");
            return stageObject.GetVariable(variableName);
        });
    }

    public void Talk(string direction, string dialogue) {
        Vector3Int dir = GetDirectionFromString(direction);
        Talk(dir, dialogue);
    }

    public void Talk(Vector3Int direction, string dialogue) {
        dialogueBox.gameObject.SetActive(true);
        talkToIndicator.SetActive(true);
        talkToIndicator.transform.localPosition = new Vector3(0, 0.5f) + direction;

        var variableReplaced = GetVariablesFromDialogue(dialogue);
        dialogueBox.ShowText(variableReplaced);

        if (stageObject.TryGetAdjacent(this, direction, out GridObject result)) {
            result.ActionAt(Actions.TALK);
        }
    }

    public override void ActionAt(Actions a) {
        switch (a) {
            case Actions.TALK:
                stageObject.Excite(0.08f);
                break;
        }
    }

    public void Stab(Vector3Int direction) {
        Talk(direction, "You stabbed me!");
    }
}
