using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : GridObject
{
    public void Talk(string direction, string dialogue) {
        Debug.Log(dialogue);
    }
}
