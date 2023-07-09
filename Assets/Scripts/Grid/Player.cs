using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Actor
{
    public GameObject nextNight;
    TextMeshProUGUI text;
    int night = 1;

    string playerName;

    private void OnMurderWeaponSet(string value) {
        if (value == "Player") {
            stageObject.AddOnUniqueActionListener("MurderWeaponTalk", delegate () {
                var wife = GameObject.Find("Wife").GetComponent<Actor>();
                wife.Talk(-GetDirectionToPos(wife.transform.position), "It seems like... " + playerName + ". is the Murder. Weapon.");
            });
        }
    }

    private void Start() {
        var names = new string[] { "Scarlet Vanderbrough", "Johannas Joplin", "Layton Kipling", "Katarina Fencepost"};
        playerName = names[Random.Range(0, names.Length)];
        Initialize();
    }

    protected override void TurnUpdate() {
        if (fireTimer == 1) {
            fireTimer = 0;
            onFire = false;
            fireIndicator.SetActive(false);
            NextNight();
            text.text = "You burned yourself horribly.\n" + text.text;
            return;
        }
        base.TurnUpdate();
    }

    public override void ActionAt(Actions a, Vector3Int direction) {
        base.ActionAt(a, direction);
    }


    private bool isInNight = false;
    public void NextNight() {
        isInNight = true;
        nextNight.SetActive(true);
        night++;
        text.text = "Night " + night + " \n Press to continue.";
        stageObject.ResetStage();
    }

    protected override void Initialize() {
        base.Initialize();
        if (stageObject != null) {
            stageObject.AddVariableSetListener("MurderWeapon", OnMurderWeaponSet);
            stageObject.SetVariable("PlayerName", playerName);
        }
        text = nextNight.GetComponentInChildren<TextMeshProUGUI>();
    }

    void OnMovement(InputValue value) {
        var move = value.Get<Vector2>();

        if (isInNight) {
            if (move.x != 0 || move.y != 0) {
                nextNight.SetActive(false);
                isInNight = false;
            }
            return;
        }

        // So we can only move in one direction:
        if (move.x != 0) {
            Move(new Vector3Int((int)move.x, 0, 0));
            stageObject.AdvanceTime();
        } else if (move.y != 0) {
            Move(new Vector3Int(0, (int)move.y, 0));
            stageObject.AdvanceTime();
        }
    }
}
