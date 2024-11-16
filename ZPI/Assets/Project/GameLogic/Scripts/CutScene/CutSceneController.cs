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
        // Na pocz�tku aktywna jest tylko kamera cutscenki
        cutsceneCamera.SetActive(true);
        player.SetActive(false);

        // Rozpocznij cutscenk� i zaplanuj jej zako�czenie
        Invoke("EndCutscene", cutsceneDuration);
    }

    void EndCutscene()
    {
        // Prze��cz kamery
        cutsceneCamera.SetActive(false);
        player.SetActive(true);

        // Usu� posta� z cutscenki
        cutsceneCharacter.SetActive(false);
    }
}
