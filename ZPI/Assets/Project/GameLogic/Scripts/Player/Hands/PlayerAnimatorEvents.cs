using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorEvents : MonoBehaviour
{
    public GameObject WandObject;
    public SmoothFollowPoint SmoothFollowPoint;
    public SpellCasting SpellCasting;

    public void OnWandWithdrawal()
    {
        if (WandObject != null)
            WandObject.SetActive(true);
    }

    public void OnWandSheath()
    {
        if (WandObject != null)
            WandObject.SetActive(false);
    }

    public void UnlockToggleWand() => SmoothFollowPoint.UnlockToggleWand();

    public void CastFireball() => SpellCasting.CastFireball();

    public void CastMeteor() => SpellCasting.CastMeteorRain();

    public void CastShield() => SpellCasting.CastShield();

    public void CastLightning() => SpellCasting.CastLightning();

    public void EnablePrimaryAttack() => SpellCasting.EnablePrimaryAttack();
}
