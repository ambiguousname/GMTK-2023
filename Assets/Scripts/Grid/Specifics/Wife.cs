using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wife : Actor
{
    public TextAsset wifeBurnedAsset;
    private void OnStabSet(string value) {
        if (value == "Wife") {
            stageObject.ClearObjectTimeline("Wife");
            stageObject.AppendTimeline(wifeBurnedAsset.text);
        } else if (value == "Player") {
            GameObject.Find("Player").GetComponent<Player>().Stab(Vector3Int.zero);
        }
    }

    protected override void Initialize() {
        base.Initialize();
        stageObject.AddVariableSetListener("StabbedObject", OnStabSet);
    }
}
