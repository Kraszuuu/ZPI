using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandWithdrawalEvent : MonoBehaviour
{
    public GameObject WandObject;
    public SmoothFollowPoint smoothFollowPoint;

    // Function that will be called when the wand is withdrawn
    public void OnWandWithdrawal()
    {
        if (WandObject != null)
            WandObject.SetActive(true);
    }

    // Function that will be called when the wand is sheathed
    public void OnWandSheath()
    {
        if (WandObject != null)
            WandObject.SetActive(false);
    }

    public void UnlockToggleWand() => smoothFollowPoint.UnlockToggleWand();
}
