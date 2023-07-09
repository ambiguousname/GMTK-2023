using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighter : GridObject
{
    public override bool Move(Vector3Int direction) {
        if (stageObject.TryGetAdjacent(this, direction, out GridObject result)) {
            if (result.canMove) {
                result.ActionAt(Actions.FIRE, direction);
                DestroyThisObject();
                return true;
            }
        }
        return base.Move(direction);
    }
}
