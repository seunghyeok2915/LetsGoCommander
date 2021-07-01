using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIResetPage : MonoBehaviour
{
    public Button yesBtn;
    public Button noBtn;

    private Sequence seq1;

    private void Start()
    {
        yesBtn.onClick.AddListener(OnClickYesBtn);
        noBtn.onClick.AddListener(OnClickNoBtn);
    }

    public void Popup()
    {
        gameObject.SetActive(true);

        seq1.Kill();
        seq1 = DOTween.Sequence();

        seq1.Append(gameObject.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0));

        seq1.Append(gameObject.transform.DOScale(new Vector3(1, 1, 1), 1).SetEase(Ease.OutElastic));
    }

    private void OnClickYesBtn()
    {
        SoundManager.instance.PlaySound(6);
        GameManager.instance.Vibrate();
        DataManager.ResetData();
    }

    private void OnClickNoBtn()
    {
        SoundManager.instance.PlaySound(6);
        GameManager.instance.Vibrate();
        gameObject.SetActive(false);
    }
}
