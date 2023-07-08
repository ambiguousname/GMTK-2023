using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireplace : Trigger
{
    protected override void Initialize() {
        base.Initialize();
        onTrigger.AddListener(Burn);
    }

    public void Burn(GridObject target, Vector3Int direction) {
        // Zero direction because this is a trigger.
        target.ActionAt(GridObject.Actions.FIRE, Vector3Int.zero);
    }
}
