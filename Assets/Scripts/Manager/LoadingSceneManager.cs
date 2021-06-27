using System;
using System.Collections;

using UnityEngine;

using UnityEngine.UI;

using UnityEngine.SceneManagement;



public class LoadingSceneManager : MonoBehaviour

{
    private static string nextScene;


    [SerializeField]
    Image progressBar;



    private void Start()
    {
        StartCoroutine(LoadScene());
    }



    public static void LoadScene(string sceneName)

    {
        DG.Tweening.DOTween.CompleteAll();

        nextScene = sceneName;

        SceneManager.LoadScene("LoadingScene");
    }



    IEnumerator LoadScene()

    {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        float timer = 0.0f;
        while (!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;
            if (op.progress < 0.9f) { progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer); if (progressBar.fillAmount >= op.progress) { timer = 0f; } }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                if (Math.Abs(progressBar.fillAmount - 1.0f) < 0.1f) { op.allowSceneActivation = true; yield break; }
            }
        }
    }

}