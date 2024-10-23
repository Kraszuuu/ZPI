using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecognitionManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _recognitionResult;
    [SerializeField] private TMP_InputField _templateName;

    private GestureTemplates _templates => GestureTemplates.Get();
    private static readonly DollarOneRecognizer _dollarOneRecognizer = new DollarOneRecognizer();
    private static readonly DollarPRecognizer _dollarPRecognizer = new DollarPRecognizer();
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

    private string TemplateName => _templateName.text;


    private void Start()
    {
        SetupState(_state);
    }

    private void SetupState(RecognizerState state)
    {
        _state = state;
        _templateName.gameObject.SetActive(_state == RecognizerState.TEMPLATE);
        _recognitionResult.gameObject.SetActive(_state == RecognizerState.RECOGNITION);


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
