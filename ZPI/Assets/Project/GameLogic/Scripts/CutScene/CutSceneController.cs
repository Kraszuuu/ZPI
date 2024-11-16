using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    public GameObject cutsceneCamera;
    public GameObject player;
    public GameObject cutsceneCharacter;
    public GameObject UI;
    public float cutsceneDuration = 10f;

    void Start()
    {
        cutsceneCamera.SetActive(true);
        player.SetActive(false);

        Invoke("EndCutscene", cutsceneDuration);
    }

    void EndCutscene()
    {
        cutsceneCamera.SetActive(false);
        player.SetActive(true);
        UI.SetActive(true);

        cutsceneCharacter.SetActive(false);
    }
}
