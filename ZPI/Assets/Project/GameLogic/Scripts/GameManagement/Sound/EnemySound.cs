using UnityEngine;

public class EnemySound : MonoBehaviour
{
    public void PlayAttackSound()
    {
        AudioManager.instance.PlayButtonClickSound();
    }
}
