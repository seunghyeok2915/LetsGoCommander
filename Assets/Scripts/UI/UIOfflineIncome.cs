using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIOfflineIncome : MonoBehaviour
{
    public Image backPanel;
    public GameObject popUpPanel;

    public Text adClaimTxt;
    public Text claimTxt;

    public Button adClaimBtn;
    public Button claimBtn;

    public int gold;

    private Sequence seq1;

    public void Popup(int time)
    {
        gameObject.SetActive(true);

        backPanel.DOFade(0, 0f);
        backPanel.DOFade(0.5f, 1f);

        seq1.Kill();
        seq1 = DOTween.Sequence();

        seq1.Append(popUpPanel.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0));

        seq1.Append(popUpPanel.transform.DOScale(new Vector3(1, 1, 1), 1).SetEase(Ease.OutElastic));

        gold = (int)(time * 0.00014);

        claimTxt.text = string.Format($"{gold}");
        adClaimTxt.text = string.Format($"{gold * 2}");

        adClaimBtn.onClick.AddListener(() => OnClickClaimBtn(2));
        claimBtn.onClick.AddListener(() => OnClickClaimBtn(1));
    }

    public void OnClickClaimBtn(int multiply)
    {
        GameManager.instance.GetGold(gold * multiply);
        popUpPanel.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 0.2f).OnComplete(() => gameObject.SetActive(false));
        backPanel.DOFade(0, 0.2f);

    }
}
