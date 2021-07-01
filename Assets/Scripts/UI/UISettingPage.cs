using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UISettingPage : MonoBehaviour
{
    public Image settingPanel;
    public GameObject popupPanel;

    public Button soundBtn;
    public Button hapticBtn;

    public Text soundTxt;
    public Text hapticTxt;

    public Text appVersionTxt;

    public Button closeBtn;

    private Sequence seq1;

    private void Start()
    {
        soundBtn.onClick.AddListener(OnClickSoundBtn);
        hapticBtn.onClick.AddListener(OnClickHapticBtn);

        closeBtn.onClick.AddListener(CallCloseFunc);
    }

    private void OnEnable()
    {
        settingPanel.DOFade(0, 0f);
        settingPanel.DOFade(1, 1f);

        seq1.Kill();
        seq1 = DOTween.Sequence();

        seq1.Append(popupPanel.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0));

        seq1.Append(popupPanel.transform.DOScale(new Vector3(1, 1, 1), 1).SetEase(Ease.OutElastic));

        soundTxt.text = GameManager.instance.sound ? "ON" : "OFF";
        hapticTxt.text = GameManager.instance.haptic ? "ON" : "OFF";

        appVersionTxt.text = Application.version;
    }

    private void OnClickSoundBtn()
    {
        SoundManager.instance.PlaySound(6);
        GameManager.instance.Vibrate();

        GameManager.instance.sound = !GameManager.instance.sound;
        soundTxt.text = GameManager.instance.sound ? "ON" : "OFF";

        SoundManager.instance.OnChangeVolume();

        DataManager.SaveData();
    }

    private void OnClickHapticBtn()
    {
        SoundManager.instance.PlaySound(6);
        GameManager.instance.Vibrate();

        GameManager.instance.haptic = !GameManager.instance.haptic;
        hapticTxt.text = GameManager.instance.haptic ? "ON" : "OFF";

        DataManager.SaveData();
    }

    private void CallCloseFunc()
    {
        SoundManager.instance.PlaySound(6);
        GameManager.instance.Vibrate();
        gameObject.SetActive(false);
    }

}
