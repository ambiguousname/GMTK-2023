using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : GridObject
{
    private void Start() {
        Initialize();
        canMove = false;
    }

    public override void ResetObject() {
        return;
    }

    public override bool Move(Vector3Int direction) {
        return false;
    }
}
