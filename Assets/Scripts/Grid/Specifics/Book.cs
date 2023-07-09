using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : GridObject
{
    public override void ActionAt(Actions a, Vector3Int direction) {
        stageObject.Excite(0.1f * excitementMultiplier);
        if (a == Actions.FIRE) {
            GetComponent<SpriteRenderer>().color = new Color(0.9f, 0.9f, 0.9f);
            excitementMultiplier += 0.5f;
            displayName = "Burned " + displayName;
        }
    }
}
