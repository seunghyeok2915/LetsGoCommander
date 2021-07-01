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

    public static void LoadScene(string sceneName)
    {
        DG.Tweening.DOTween.CompleteAll();

        nextScene = sceneName;

        SceneManager.LoadScene("LoadingScene");
    }

    public IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        float timer = 0.0f;
        Debug.Log("1");
        while (!op.isDone)
        {
            Debug.Log("2");
            yield return null;

            timer += Time.deltaTime;

            if (op.progress >= 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                Debug.Log("3");
                if (progressBar.fillAmount > 0.9f)
                {
                    Debug.Log("4");
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
            else
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1, timer);
                Debug.Log("5");
                if (progressBar.fillAmount >= op.progress)
                {
                    Debug.Log("6");
                    timer = 0f;
                }
            }

        }
        progressBar.fillAmount = 1f;

    }
}