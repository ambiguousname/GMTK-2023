using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : GridObject
{
    public new readonly bool canMove = false;
    private void Start() {
        Initialize();
    }
    public new void Move(Vector3Int direction) {
        return;
    }
}
