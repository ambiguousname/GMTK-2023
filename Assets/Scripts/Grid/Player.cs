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
    private void OnMurderWeaponSet(string value) {
        if (value == "Player") {
            stageObject.AddOnUniqueActionListener("MurderWeaponTalk", delegate () {
                var wife = GameObject.Find("Wife").GetComponent<Actor>();
                wife.Talk("S", "It seems that this... Player. Is. the. murder weapon.");
            });
        }
    }


    private bool isInNight = false;
    public void NextNight() {
        isInNight = true;
        nextNight.SetActive(true);
        night++;
        text.text = "Night " + night + " \n Press to continue.";
    }

    protected override void Initialize() {
        base.Initialize();
        if (stageObject != null) {
            stageObject.AddVariableSetListener("MurderWeapon", OnMurderWeaponSet);
        }
        text = nextNight.GetComponentInChildren<TextMeshProUGUI>();
    }

    void OnMovement(InputValue value) {
        var move = value.Get<Vector2>();

        if (isInNight) {
            if (move.x > 0 || move.y > 0) {
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
