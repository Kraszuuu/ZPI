using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public Slider volumeSlider;

    private void Start()
    {
        volumeSlider.onValueChanged.AddListener(SetVolume);
        volumeSlider.value = AudioManager.instance.backgroundMusicSource.volume;
    }

    private void SetVolume(float volume)
    {
        AudioManager.instance.SetVolume(volume);
    }
}
