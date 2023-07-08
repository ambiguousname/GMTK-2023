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

    Grid grid;

    StageTimeline timeline;
    private void Awake() {
        grid = GetComponent<Grid>();
        gridObjects = new Dictionary<Vector3Int, GridObject>();
        timeline = GetComponent<StageTimeline>();
    }

    private delegate void GridObjectOperation(Vector3Int pos, GridObject gridObject, params object[] args);

    private void ModifyGridObject(Vector3Int origin, GridObject gridObject, GridObjectOperation operation, params object[] args) { 
        for (int i = 0; i < gridObject.Scale.x; i++) {
            for (int j = 0; j < gridObject.Scale.y; j++) {
                operation(origin + new Vector3Int(i, j), gridObject, args);
            }
        }
    }

    private void DeleteGridObject(Vector3Int pos, GridObject gridObject, params object[] args) {
        gridObjects.Remove(pos);
    }

    private void AddGridObject(Vector3Int pos, GridObject gridObject, params object[] args) {
        gridObjects.Add(pos, gridObject);
    }

    private void PushAdjacentObjects(Vector3Int pos, GridObject gridObject, params object[] args) {
        if (gridObjects.TryGetValue(pos, out GridObject newGridObject)) {
            newGridObject.Move((Vector3Int) args[0]);
        }
    }

    public void RegisterObject(GridObject gridObject) {
        // Snap to the grid:
        var cellPos = grid.WorldToCell(gridObject.transform.position);
        gridObject.transform.position = grid.CellToWorld(cellPos) + gridObject.GridAnchor;

        ModifyGridObject(cellPos, gridObject, AddGridObject);
    }


    public void MoveObject(GridObject gridObject, Vector3Int direction) {
        Vector3Int gridPos = grid.WorldToCell(gridObject.transform.position - gridObject.GridAnchor);

        // TODO, some logic about hitting walls.
        Vector3Int newPos = gridPos + direction;

        // TODO: Slide transition (call from GridObject itself)
        gridObject.transform.position = grid.CellToWorld(newPos) + gridObject.GridAnchor;

        ModifyGridObject(gridPos, gridObject, DeleteGridObject);

        ModifyGridObject(newPos, gridObject, PushAdjacentObjects, direction);

        ModifyGridObject(newPos, gridObject, AddGridObject);
    }

    public void AdvanceTime() {
        onAdvance.Invoke();
        timeline.Advance();
    }
}
