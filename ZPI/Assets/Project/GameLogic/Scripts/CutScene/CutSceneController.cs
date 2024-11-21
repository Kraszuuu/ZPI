using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    public float cutsceneDuration = 10f;
    public Animator cutsceneAnimator;
    private AsyncOperation sceneLoadOperation;

    void Start()
    {
        // Rozpocznij ładowanie sceny w tle.
        sceneLoadOperation = SceneManager.LoadSceneAsync("Final Scene");
        sceneLoadOperation.allowSceneActivation = false; // Wstrzymaj aktywację sceny.

        // Zaplanuj zakończenie cutscenki.
        Invoke("EndCutscene", cutsceneDuration);
    }

    void EndCutscene()
    {
        StartCoroutine(MakeTransition());
    }

    IEnumerator MakeTransition()
    {
        cutsceneAnimator.SetBool("PlayAnimation", true);
        yield return new WaitForSeconds(1.5f);
        sceneLoadOperation.allowSceneActivation = true;
    }
}
