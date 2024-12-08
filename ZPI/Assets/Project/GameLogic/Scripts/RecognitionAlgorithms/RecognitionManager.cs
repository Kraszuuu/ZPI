using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecognitionManager : MonoBehaviour
{
    private GestureTemplates _templates => GestureTemplates.Get();
    private static readonly DollarOneRecognizer _dollarOneRecognizer = new();
    private static readonly DollarPRecognizer _dollarPRecognizer = new();
    private IRecognizer _currentRecognizer = _dollarPRecognizer;
    private RecognizerState _state = RecognizerState.RECOGNITION;

    public enum RecognizerState
    {
        TEMPLATE,
        RECOGNITION,
        TEMPLATE_REVIEW
    }

    [Serializable]
    public struct GestureTemplate
    {
        public string Name;
        public DollarPoint[] Points;

        public GestureTemplate(string templateName, DollarPoint[] preparePoints)
        {
            Name = templateName;
            Points = preparePoints;
        }
    }

    private void Start()
    {
        SetupState(_state);
    }

    private void SetupState(RecognizerState state)
    {
        _state = state;
    }

    public (string, float) OnDrawFinished(DollarPoint[] points)
    {
        (string, float) result = _currentRecognizer.DoRecognition(points, 64,
                _templates.RawTemplates);
        return result;

    }

    private void OnApplicationQuit()
    {
        _templates.Save();
    }
}
