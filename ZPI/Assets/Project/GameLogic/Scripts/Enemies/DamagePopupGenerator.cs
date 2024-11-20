using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UIElements;

public class DamagePopupGenerator : MonoBehaviour
{
    public DamagePopupGenerator Current;
    public GameObject prefab;
    private void Update()
    {
        
    }

    private void Awake()
    {
        Current = this;
    }

    public void CreatePopup(Vector3 position, string text, Color color)
    {
        if (prefab == null) { Debug.LogError("Gowno"); }
        var popup = Instantiate(prefab, position, Quaternion.identity);
        var temp = popup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        temp.text = text;
        temp.faceColor = color;

        Destroy(popup, 1f);
    }
}
