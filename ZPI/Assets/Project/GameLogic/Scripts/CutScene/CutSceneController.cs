using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    public GameObject cutsceneCamera;
    public GameObject player;
    public GameObject cutsceneCharacter;
    public float cutsceneDuration = 10f;

    void Start()
    {
        // Na pocz¹tku aktywna jest tylko kamera cutscenki
        cutsceneCamera.SetActive(true);
        player.SetActive(false);

        // Rozpocznij cutscenkê i zaplanuj jej zakoñczenie
        Invoke("EndCutscene", cutsceneDuration);
    }

    void EndCutscene()
    {
        // Prze³¹cz kamery
        cutsceneCamera.SetActive(false);
        player.SetActive(true);

        // Usuñ postaæ z cutscenki
        cutsceneCharacter.SetActive(false);
    }
}
