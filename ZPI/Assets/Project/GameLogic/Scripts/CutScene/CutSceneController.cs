using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    public float cutsceneDuration = 10f;

    void Start()
    {
        Invoke("EndCutscene", cutsceneDuration);
    }

    void EndCutscene()
    {
        SceneManager.LoadScene("Final Scene");
    }
}
