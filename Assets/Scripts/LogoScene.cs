using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LogoScene : MonoBehaviour
{
    public Image logoImg;

    public Sequence sequence;
    private void Start()
    {
        sequence.Kill();
        sequence = DOTween.Sequence();

        sequence.Append(logoImg.DOFade(1f, 1f))
        .Join(logoImg.gameObject.transform.DOScale(1.1f, 3f))
        .Insert(1.5f, logoImg.DOFade(0f, 1f))
        .OnComplete(() => LoadingSceneManager.LoadScene("MainScene"));
    }
}
