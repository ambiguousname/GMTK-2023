using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageAudience : MonoBehaviour
{
    Stage stage;
    Image stageUI;
    public float audienceExcitement { get { return _audienceExcitement; } }
    private float _audienceExcitement = 0.3f;

    private void Start() {
        stage = GetComponent<Stage>();
        stage.onAdvance.AddListener(ModifyAudienceExcitement);
        stageUI = GameObject.Find("AudienceExcitement").GetComponent<Image>();
    }

    private void Update() {
        stageUI.fillAmount = Mathf.Lerp(stageUI.fillAmount, _audienceExcitement, Time.deltaTime);
        if (_audienceExcitement >= 0.95f) {
            SceneManager.LoadScene("WinScreen");
        }
    }

    public void ResetExcitement() {
        _audienceExcitement = 0.3f;
    }

    public void AddExcitement(float change) {
        _audienceExcitement += change;
    }

    private void ModifyAudienceExcitement() {
        _audienceExcitement -= 0.004f;
        if (_audienceExcitement <= 0) {
            _audienceExcitement = 0.01f;
        }
    }
}
