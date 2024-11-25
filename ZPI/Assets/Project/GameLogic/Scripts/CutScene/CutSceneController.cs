using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    public float cutsceneDuration = 10f;
    public Animator cutsceneAnimator;
    public TextMeshProUGUI cutsceneText;
    private AsyncOperation _sceneLoadOperation;
    private bool _isSkipping = false;
    private bool _canSkip = false;

    void Start()
    {

        // Ustawienie koloru tekstu na bia³y
        //cutsceneText.color = Color.white;

        // W³¹czenie i ustawienie obramowania
        cutsceneText.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.2f); // Gruboœæ obramowania
        cutsceneText.fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor, Color.black); // Kolor obramowania
        _sceneLoadOperation = SceneManager.LoadSceneAsync("Final Scene");
        _sceneLoadOperation.allowSceneActivation = false;

        Invoke("EndCutscene", cutsceneDuration);

        StartCoroutine(ShowCutsceneText());
    }

    void Update()
    {
        if (_canSkip)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SkipCutscene();
            }
        }
    }

    void SkipCutscene()
    {
        if (_isSkipping) return;
        _isSkipping = true;

        CancelInvoke("EndCutscene");

        EndCutscene();
    }

    void EndCutscene()
    {
        StartCoroutine(MakeTransition());
    }

    IEnumerator MakeTransition()
    {
        cutsceneAnimator.SetBool("PlayAnimation", true);
        yield return new WaitForSeconds(2f);
        _sceneLoadOperation.allowSceneActivation = true;
    }

    IEnumerator ShowCutsceneText()
    {
        yield return new WaitForSeconds(1f);
        cutsceneText.gameObject.SetActive(true);
        _canSkip = true;
        yield return new WaitForSeconds(3f);
        cutsceneText.gameObject.SetActive(false);
    }
}
