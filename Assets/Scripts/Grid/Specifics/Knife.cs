using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : GridObject
{
    public AudioSource audio;
    protected override void Initialize() {
        base.Initialize();
        if (stageObject != null) {

        }
    }

    GridObject thingToStab;
    Vector3Int directionToStab;

    private void StabOnce() {
        thingToStab.displayName += ", Stabbed By A Knife";
        if ((thingToStab.GetType()).Equals(typeof(Actor))) {
            ((Actor)thingToStab).Stab(directionToStab);
        }
        stageObject.SetVariable("StabbedObject", thingToStab.name);
        thingToStab.excitementMultiplier += 0.3f;
        stageObject.Excite(0.2f * excitementMultiplier);
        stageObject.onAdvance.RemoveListener(StabOnce);
        audio.Play();
    }

    public override bool Move(Vector3Int direction) {
        if (stageObject.TryGetAdjacent(this, direction, out GridObject result)) {
            if (result.canMove) {
                thingToStab = result;
                directionToStab = -direction;
                stageObject.onAdvance.AddListener(StabOnce);
                DestroyThisObject();
                return true;
            }
        }
        return base.Move(direction);
    }
}
