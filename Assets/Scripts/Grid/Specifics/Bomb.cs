using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : GridObject
{
    public int radius = 1;
    public override void ActionAt(Actions a, Vector3Int direction) {
        stageObject.Excite(0.1f * excitementMultiplier);
        if (a == Actions.FIRE) {
            for (int i = -radius; i <= radius; i++) {
                for (int j = -radius; j <= radius; j++) {
                    if (stageObject.TryGetAdjacent(this, new Vector3Int(i, j), out GridObject result) && result.canMove && result != this) {
                        if (displayName.Contains("Knife")) {
                            result.excitementMultiplier += 1;
                            result.ActionAt(Actions.SHRAPNEL, new Vector3Int(i, j));
                        } else {
                            result.ActionAt(a, new Vector3Int(i, j));
                        }
                    }
                }
            }
            DestroyThisObject();
        }
    }
}
