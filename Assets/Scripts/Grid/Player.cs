using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Actor
{
    private void OnMurderWeaponSet(string value) {
        if (value == "Player") {
            stageObject.AddOnUniqueActionListener("MurderWeaponTalk", delegate () {
                var wife = GameObject.Find("Wife").GetComponent<Actor>();
                wife.Talk("S", "Test!");
            });
        }
    }

    protected override void Initialize() {
        base.Initialize();
        if (stageObject != null) {
            stageObject.AddVariableSetListener("MurderWeapon", OnMurderWeaponSet);
        }
    }

    void OnMovement(InputValue value) {
        var move = value.Get<Vector2>();
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
