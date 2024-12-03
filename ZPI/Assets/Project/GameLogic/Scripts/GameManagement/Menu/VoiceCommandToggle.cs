using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour
{
    private Toggle _toggle;

    void Awake()
    {
        _toggle = GetComponent<Toggle>();

        if (_toggle != null)
        {
            _toggle.isOn = GameState.Instance.IsSpeechRecognitionEnabled;
            _toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
    }

    private void OnToggleValueChanged(bool isOn)
    {
        GameState.Instance.IsSpeechRecognitionEnabled = isOn;
    }

    void OnDestroy()
    {
        if (_toggle != null)
        {
            _toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }
    }
}
