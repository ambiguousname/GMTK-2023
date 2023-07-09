using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class Actor : GridObject
{
    public DialogueBox dialogueBox;
    public GameObject talkToIndicator;
    public GameObject fireIndicator;

    Vector3 initialTalkPos;

    private void Start() {
        Initialize();
        initialTalkPos = talkToIndicator.transform.localPosition;
    }

    protected override void Initialize() {
        base.Initialize();
        if (stageObject != null) {
            stageObject.beforeAdvance.AddListener(HideDialogue);
            stageObject.onAdvance.AddListener(TurnUpdate);
        }
    }

    protected int fireTimer = 0;
    protected bool onFire = false;
    protected virtual void TurnUpdate() {
        if (fireTimer > 0) {
            fireTimer--;
            GameObject.Find("FireSound").GetComponent<AudioSource>().Play();
            if (fireTimer <= 0) {
                excitementMultiplier = 1;
                onFire = false;
                fireIndicator.SetActive(false);
            } else {
                Talk(Vector3Int.zero, "AAAAAAAAA!");
            }
        }
    }

    public override void ResetObject() {
        stageObject.beforeAdvance.RemoveListener(HideDialogue);
        stageObject.onAdvance.RemoveListener(TurnUpdate);
        base.ResetObject();
        fireTimer = 0;
        onFire = false;
        fireIndicator.SetActive(false);
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

    public void TalkSelf(string dialogue) {
        Talk(Vector3Int.zero, dialogue);
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
        stageObject.Excite(0.05f * excitementMultiplier);
        Burn(burnDirection);
        stageObject.onAdvance.RemoveListener(BurnOnce);
        GameObject.Find("FireSound").GetComponent<AudioSource>().Play();

        onFire = true;
        fireTimer = 5;
        fireIndicator.SetActive(true);
    }

    Vector3Int shrapnelDirection;
    protected void ShrapnelOnce() {
        stageObject.Excite(0.2f * excitementMultiplier);
        Shrapnel(shrapnelDirection);
        stageObject.onAdvance.RemoveListener(ShrapnelOnce);

        onFire = true;
        fireTimer = 5;
    }

    public override void ActionAt(Actions a, Vector3Int direction) {
        switch (a) {
            case Actions.TALK:
                stageObject.Excite(0.05f * excitementMultiplier);
                break;
            case Actions.FIRE:
                if (!onFire) {
                    excitementMultiplier += 0.05f;
                    burnDirection = direction;
                    stageObject.onAdvance.AddListener(BurnOnce);
                }
                break;
            case Actions.SHRAPNEL:
                if (!onFire) {
                    excitementMultiplier += 0.3f;
                    shrapnelDirection = direction;
                    stageObject.onAdvance.AddListener(ShrapnelOnce);
                }
                break;
        }
    }

    public void Burn(Vector3Int direction) {
        Talk(-direction, "Oh my god, I'm on fire!");
    }

    public void Shrapnel(Vector3Int direction) {

        var lines = new string[]{ "Is that live shrapnel?", "THIS IS MY SUPERHERO ORIGIN STORY!", "I am bleeding to death!"};
        Talk(-direction, lines[Random.Range(0, lines.Length)]);
    }

    public void Stab(Vector3Int direction) {
        Debug.Log(name);
        Talk(direction, "You stabbed me!");
    }
}
