using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    public Vector3Int Scale = new Vector3Int(1, 1);
    public UnityEvent<GridObject, Vector3Int> onTrigger;


    protected Stage stageObject;

    private void Start() {
        Initialize();
    }

    protected virtual void Initialize() {
        stageObject = GetComponentInParent<Stage>();
        if (stageObject == null) {
            Debug.LogError("Grid Object could not find Stage script in a parent.");
        } else {
            stageObject.RegisterTrigger(this);
        }
    }

    public virtual void ActionAt(GridObject.Actions a, Vector3Int direction) {
        return;
    }

    public void ResetObject() {
        stageObject.DeregisterTrigger(this);
        stageObject.RegisterTrigger(this);
    }
}
