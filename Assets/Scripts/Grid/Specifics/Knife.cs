using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : GridObject
{
    protected override void Initialize() {
        base.Initialize();
        if (stageObject != null) {

        }
    }

    Actor actorToStab;
    Vector3Int directionToStab;

    private void StabOnce() {
        actorToStab.Stab(directionToStab);
        stageObject.Excite(0.1f);
        stageObject.onAdvance.RemoveListener(StabOnce);
    }

    public override bool Move(Vector3Int direction) {
        if (stageObject.TryGetAdjacent(this, direction, out GridObject result)) {
            if (result.GetType().IsAssignableFrom(typeof(Actor))) {
                actorToStab = (Actor)result;
                directionToStab = -direction; 
                stageObject.onAdvance.AddListener(StabOnce);
                Destroy();
                return true;
            }
        }
        return base.Move(direction);
    }
}
