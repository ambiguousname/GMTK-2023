using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    public UnityEvent<GridObject> onTrigger;


    protected Stage stageObject;

    private void Start() {
        stageObject = GetComponentInParent<Stage>();
        if (stageObject == null) {
            Debug.LogError("Grid Object could not find Stage script in a parent.");
        } else {
            stageObject.RegisterTrigger(this);
        }
    }

    public void ResetObject() {
        stageObject.DeregisterTrigger(this);
        stageObject.RegisterTrigger(this);
    }
}
