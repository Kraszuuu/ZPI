using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Transform descriptionPanel;

    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionPanel.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionPanel.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        descriptionPanel.gameObject.SetActive(false);
    }
}
