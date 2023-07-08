using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    public Vector3 GridAnchor = new Vector2(0.5f, 0.5f);

    public Vector3Int Scale = new Vector3Int(1, 1);

    public bool canMove = true;

    protected Stage stageObject;

    protected Vector3 initialPosition;


    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    protected virtual void Initialize() {
        stageObject = GetComponentInParent<Stage>();
        if (stageObject == null) {
            Debug.LogError("Grid Object could not find Stage script in a parent.");
        } else {
            stageObject.RegisterObject(this);
        }
        initialPosition = this.transform.position;
    }

    public virtual void ResetObject() {
        stageObject.DeregisterObject(this);
        this.transform.position = initialPosition;
        Initialize();
    }

    protected virtual void Destroy() {
        if (stageObject != null) {
            stageObject.DeregisterObject(this);
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected Vector3Int GetDirectionFromString(string direction) {
        switch (direction.ToLower()) {
            case "n":
                return new Vector3Int(0, 1);
            case "s":
                return new Vector3Int(0, -1);
            case "e":
                return new Vector3Int(1, 0);
            case "w":
                return new Vector3Int(-1, 0);
            default:
                return Vector3Int.zero;
        }
    }

    public void Move(string direction) {
        Vector3Int dir = GetDirectionFromString(direction);
        Move(dir);
    }

    public void Set(string direction, string variableName) {
        Vector3Int dir = GetDirectionFromString(direction);
        if (stageObject.TryGetAdjacent(this, dir, out GridObject result)) {
            stageObject.SetVariable(variableName, result.name);
        }
    }

    public void SetDefault(string variableName, string value) {
        stageObject.SetVariable(variableName, value);
    }

    public virtual bool Move(Vector3Int direction) {
        return stageObject.MoveObject(this, direction);
    }
}
