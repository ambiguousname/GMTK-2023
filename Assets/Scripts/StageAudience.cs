using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageAudience : MonoBehaviour
{
    Stage stage;
    public float audienceExcitement { get { return _audienceExcitement; } }
    private float _audienceExcitement = 0.0f;

    private void Start() {
        stage = GetComponent<Stage>();
        stage.onAdvance.AddListener(ModifyAudienceExcitement);
    }

    // TODO: Add engagement UI modifications here.
    public void AddExcitement(float change) {
        _audienceExcitement += change;
    }

    private void ModifyAudienceExcitement() {
        _audienceExcitement -= 0.1f;
    }
}
