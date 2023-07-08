using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Actor
{

    // Update is called once per frame
    void Update()
    {
        
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
