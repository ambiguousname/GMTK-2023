using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasticBaby : GridObject
{
    public override void ActionAt(Actions a, Vector3Int direction) {
        stageObject.Excite(0.1f * excitementMultiplier);
        if (a == Actions.FIRE) {
            GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f);
            excitementMultiplier += 0.5f;
            displayName = "Melted " + displayName;
        }
    }

    public override void ResetObject() {
        base.ResetObject();
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}
