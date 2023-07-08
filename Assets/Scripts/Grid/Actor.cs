using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class Actor : GridObject
{
    public DialogueBox dialogueBox;
    public GameObject talkToIndicator;

    Vector3 initialTalkPos;

    protected override void Initialize() {
        base.Initialize();
        if (stageObject != null) {
            stageObject.beforeAdvance.AddListener(HideDialogue);
            stageObject.onAdvance.AddListener(TurnUpdate);
        }
        initialTalkPos = talkToIndicator.transform.localPosition;
    }

    protected int fireTimer = 0;
    protected bool onFire = false;
    protected virtual void TurnUpdate() {
        if (fireTimer > 0) {
            fireTimer -= 1;
            if (fireTimer <= 0) {
                onFire = false;
            } else {
                Talk(Vector3Int.zero, "AAAAAAAAA!");
            }
        }
    }

    public override void ResetObject() {
        base.ResetObject();
        fireTimer = 0;
        dialogueBox.gameObject.SetActive(false);
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
        talkToIndicator.transform.localPosition = initialTalkPos + direction;

        var variableReplaced = GetVariablesFromDialogue(dialogue);
        dialogueBox.ShowText(variableReplaced);

        if (stageObject.TryGetAdjacent(this, direction, out GridObject result) && result != this) {
            result.ActionAt(Actions.TALK, direction);
        }
        if (stageObject.TryGetAdjacentTrigger(this, direction, out Trigger resultTrigger)) {
            resultTrigger.ActionAt(Actions.TALK, direction);
        }
    }

    Vector3Int burnDirection;
    protected void BurnOnce() {
        stageObject.Excite(0.1f);
        Burn(burnDirection);
        stageObject.onAdvance.RemoveListener(BurnOnce);

        onFire = true;
        fireTimer = 5;
    }

    public override void ActionAt(Actions a, Vector3Int direction) {
        switch (a) {
            case Actions.TALK:
                stageObject.Excite(0.04f);
                break;
            case Actions.FIRE:
                if (!onFire) {
                    burnDirection = direction;
                    stageObject.onAdvance.AddListener(BurnOnce);
                }
                break;
        }
    }

    public void Burn(Vector3Int direction) {
        Talk(-direction, "Oh my god, I'm on fire!");
    }

    public void Stab(Vector3Int direction) {
        Talk(direction, "You stabbed me!");
    }
}
