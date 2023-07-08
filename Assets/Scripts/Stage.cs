using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Stage : MonoBehaviour {
    public UnityEvent onAdvance;
    public UnityEvent beforeAdvance;

    private Dictionary<Vector3Int, GridObject> gridObjects;
    private Dictionary<Vector3Int, Trigger> triggerObjects;

    private Dictionary<string, string> variables;
    private Dictionary<string, UnityEvent<string>> onVariableSet;

    Grid grid;

    StageTimeline timeline;
    StageAudience audience;
    private void Awake() {
        grid = GetComponent<Grid>();
        gridObjects = new Dictionary<Vector3Int, GridObject>();
        triggerObjects = new Dictionary<Vector3Int, Trigger>();
        variables = new Dictionary<string, string>();
        onVariableSet = new Dictionary<string, UnityEvent<string>>();
        timeline = GetComponent<StageTimeline>();
        audience = GetComponent<StageAudience>();
    }

    private delegate bool GridObjectOperation(Vector3Int pos, GridObject gridObject, params object[] args);

    private bool ModifyGridObject(Vector3Int origin, GridObject gridObject, GridObjectOperation operation, params object[] args) {
        for (int i = 0; i < gridObject.Scale.x; i++) {
            for (int j = 0; j < gridObject.Scale.y; j++) {
                var successful = operation(origin - new Vector3Int(i, j), gridObject, args);
                if (!successful) {
                    return successful;
                }
            }
        }
        return true;
    }

    private bool DeleteGridObject(Vector3Int pos, GridObject gridObject, params object[] args) {
        gridObjects.Remove(pos);
        return true;
    }

    private bool AddGridObject(Vector3Int pos, GridObject gridObject, params object[] args) {
        gridObjects.Add(pos, gridObject);
        return true;
    }

    private bool PushAdjacentObjects(Vector3Int pos, GridObject gridObject, params object[] args) {

        if (gridObjects.TryGetValue(pos, out GridObject newGridObject) && newGridObject != gridObject) {
            return newGridObject.Move((Vector3Int)args[0]);
        }
        if (triggerObjects.TryGetValue(pos, out Trigger triggerVal)) {
            triggerVal.onTrigger.Invoke(gridObject);
        }
        return true;
    }

    private Vector3 GetOffsetFromObject(GridObject gridObject) {
        return new Vector3(gridObject.GridAnchor.x * grid.transform.localScale.x, gridObject.GridAnchor.y * grid.transform.localScale.y, gridObject.GridAnchor.z * grid.transform.localScale.z);
    }

    public void RegisterObject(GridObject gridObject) {
        // Snap to the grid:
        var cellPos = grid.WorldToCell(gridObject.transform.position);
        Vector3 offset = GetOffsetFromObject(gridObject);
        gridObject.transform.position = grid.CellToWorld(cellPos) + offset;

        ModifyGridObject(cellPos, gridObject, AddGridObject);
    }

    public void DeregisterObject(GridObject gridObject) {
        Vector3 offset = GetOffsetFromObject(gridObject);
        var cellPos = grid.WorldToCell(gridObject.transform.position - offset);

        ModifyGridObject(cellPos, gridObject, DeleteGridObject);
    }

    public void RegisterTrigger(Trigger triggerObject) {
        var cellPos = grid.WorldToCell(triggerObject.transform.position);
        triggerObjects.Add(cellPos, triggerObject);
    }


    public bool MoveObject(GridObject gridObject, Vector3Int direction) {
        Vector3 offset = GetOffsetFromObject(gridObject);
        Vector3Int gridPos = grid.WorldToCell(gridObject.transform.position - offset);

        Vector3Int newPos = gridPos + direction;

        if (ModifyGridObject(newPos, gridObject, PushAdjacentObjects, direction)) {
            // TODO: Slide transition (call from GridObject itself)

            ModifyGridObject(gridPos, gridObject, DeleteGridObject);
            ModifyGridObject(newPos, gridObject, AddGridObject);
            gridObject.transform.position = grid.CellToWorld(newPos) + offset;
            return true;
        } else {
            return false;
        }
    }

    public bool TryGetAdjacent(GridObject gridObject, Vector3Int direction, out GridObject result) {
        Vector3 offset = GetOffsetFromObject(gridObject);

        Vector3Int directionScaled = new Vector3Int(direction.x * gridObject.Scale.x, direction.y * gridObject.Scale.y, direction.z * gridObject.Scale.z);
        return gridObjects.TryGetValue(grid.WorldToCell(gridObject.transform.position - offset) + directionScaled, out result);
    }

    public void AdvanceTime() {
        beforeAdvance.Invoke();
        timeline.Advance();
        onAdvance.Invoke();
    }

    public void Excite(float amount) {
        audience.AddExcitement(amount);
    }

    public string GetVariable(string name) {
        return variables[name];
    }

    public void AddVariableSetListener(string name, UnityAction<string> function) {
        if (onVariableSet.ContainsKey(name)) {
            onVariableSet[name].AddListener(function);
        } else {
            var unityEvent = new UnityEvent<string>();
            unityEvent.AddListener(function);
            onVariableSet.Add(name, unityEvent);
        }
    }

    public void SetVariable(string name, string value) {
        if (variables.ContainsKey(name)) {
            variables[name] = value;
        } else {
            variables.Add(name, value);
        }
        if (onVariableSet.ContainsKey(name)) {
            onVariableSet[name].Invoke(value);
        }
    }

    public void AddOnUniqueActionListener(string name, UnityAction action) {
        timeline.AddOnUniqueAction(name, action);
    }
}
