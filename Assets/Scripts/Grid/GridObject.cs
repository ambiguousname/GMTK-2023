using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    public Vector3 GridAnchor = new Vector2(0.5f, 0.5f);

    public Vector3Int Scale = new Vector3Int(1, 1);

    public bool canMove = true;

    protected Stage stageObject;


    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    protected void Initialize() {
        stageObject = GetComponentInParent<Stage>();
        if (stageObject == null) {
            Debug.LogError("Grid Object could not find Stage script in a parent.");
        } else {
            stageObject.RegisterObject(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move(string direction) {
        switch (direction.ToLower()) {
            case "n":
                Move(new Vector3Int(0, 1));
                break;
            case "s":
                Move(new Vector3Int(0, -1));
                break;
            case "e":
                Move(new Vector3Int(1, 0));
                break;
            case "w":
                Move(new Vector3Int(-1, 0));
                break;
        }
    }

    public void Move(Vector3Int direction) {
        stageObject.MoveObject(this, direction);
    }
}
