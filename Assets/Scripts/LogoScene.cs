using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LogoScene : MonoBehaviour
{
    public Image logoImg;

    public Sequence sequence;

    public GooglePlayLogin googlePlayLogin;

    public bool logoStatus;

    private void Start()
    {
        sequence.Kill();
        sequence = DOTween.Sequence();

        sequence.Append(logoImg.DOFade(1f, 1f))
        .Join(logoImg.gameObject.transform.DOScale(1.1f, 3f))
        .Insert(1.5f, logoImg.DOFade(0f, 1f))
        .OnComplete(() => logoStatus = true);
    }

    private void Update()
    {
        if (googlePlayLogin.loginStaus && logoStatus)
        {
            LoadingSceneManager.LoadScene("MainScene");
        }

#if UNITY_EDITOR
        if (logoStatus)
        {
            LoadingSceneManager.LoadScene("MainScene");
        }
#endif
    }


}
