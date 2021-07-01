using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIExitPopupPage : MonoBehaviour
{
    public GameObject exitPanel;

    public Button yesBtn;
    public Button[] noBtns;

    private void Start()
    {
        yesBtn.onClick.AddListener(Application.Quit);
        for (int i = 0; i < noBtns.Length; i++)
        {
            noBtns[i].onClick.AddListener(() => gameObject.SetActive(false));
        }
    }

    public void Popup()
    {
        gameObject.SetActive(true);

        exitPanel.transform.localScale = Vector3.zero;
        exitPanel.transform.DOScale(new Vector3(1, 1, 1), 1f).SetEase(Ease.OutElastic);
    }
}
