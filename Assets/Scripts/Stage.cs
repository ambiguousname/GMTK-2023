using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Stage : MonoBehaviour
{
    public UnityEvent onAdvance;

    private Dictionary<Vector3Int, GridObject> gridObjects;
    private Dictionary<Vector3Int, Trigger> triggerObjects;

    Grid grid;

    StageTimeline timeline;
    private void Awake() {
        grid = GetComponent<Grid>();
        gridObjects = new Dictionary<Vector3Int, GridObject>();
        triggerObjects = new Dictionary<Vector3Int, Trigger>();
        timeline = GetComponent<StageTimeline>();
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
            if (!newGridObject.canMove) {
                return false;
            }
            return MoveObject(newGridObject, (Vector3Int) args[0]);
        }
        if (triggerObjects.TryGetValue(pos, out Trigger triggerVal)) {
            triggerVal.onTrigger.Invoke(gridObject);
        }
        return true;
    }

    public void RegisterObject(GridObject gridObject) {
        // Snap to the grid:
        var cellPos = grid.WorldToCell(gridObject.transform.position);
        Vector3 offset = new Vector3(gridObject.GridAnchor.x * grid.cellSize.x, gridObject.GridAnchor.y * grid.cellSize.y, gridObject.GridAnchor.z * grid.cellSize.z);
        gridObject.transform.position = grid.CellToWorld(cellPos) + offset;

        ModifyGridObject(cellPos, gridObject, AddGridObject);
    }

    public void RegisterTrigger(Trigger triggerObject) {
        var cellPos = grid.WorldToCell(triggerObject.transform.position);
        triggerObjects.Add(cellPos, triggerObject);
    }


    public bool MoveObject(GridObject gridObject, Vector3Int direction) {
        Vector3 offset = new Vector3(gridObject.GridAnchor.x * grid.cellSize.x, gridObject.GridAnchor.y * grid.cellSize.y, gridObject.GridAnchor.z * grid.cellSize.z);
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

    public void AdvanceTime() {
        onAdvance.Invoke();
        timeline.Advance();
    }
}
