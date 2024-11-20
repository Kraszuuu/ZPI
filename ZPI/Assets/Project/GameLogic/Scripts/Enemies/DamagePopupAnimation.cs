using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopupAnimation : MonoBehaviour
{
    public AnimationCurve OpacityCurve;
    public AnimationCurve ScaleCurve;
    public AnimationCurve HeightCurve;

    private TextMeshProUGUI _tmp;
    private float _time = 0;
    private Vector3 _origin;

    private void Awake()
    {
        _tmp = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _origin = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        _tmp.color = new Color(1, 1, 1, OpacityCurve.Evaluate(_time));
        transform.localScale = Vector3.one * ScaleCurve.Evaluate(_time);    
        transform.position = _origin + new Vector3(0, 1 + HeightCurve.Evaluate(_time), 0);
        _time += Time.deltaTime;
    }
}
