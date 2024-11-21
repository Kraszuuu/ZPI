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
        // Rozpocznij ³adowanie sceny w tle.
        sceneLoadOperation = SceneManager.LoadSceneAsync("Final Scene");
        sceneLoadOperation.allowSceneActivation = false; // Wstrzymaj aktywacjê sceny.

        // Zaplanuj zakoñczenie cutscenki.
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
