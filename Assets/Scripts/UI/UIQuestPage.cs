using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIQuestPage : MonoBehaviour
{
    public QuestManager questManager;
    public QuestSlot[] questSlots;

    public GameObject notifIcon;

    public Image backPanel;
    public GameObject popUpPanel;

    public Text timeLeft;

    public Button closeBtn;
    public Button newQuestsBtn;

    public UIBuyNewQuestsPage uIBuyNewQuestsPage;

    private Sequence seq1;

    private void Start()
    {
        questSlots = GetComponentsInChildren<QuestSlot>();
        closeBtn.onClick.AddListener(OnClickCloseBtn);

        newQuestsBtn.onClick.AddListener(OnClickNewQuestBtn);
    }

    public void Popup()
    {
        gameObject.SetActive(true);

        backPanel.DOFade(0, 0f);
        backPanel.DOFade(0.5f, 1f);

        seq1.Kill();
        seq1 = DOTween.Sequence();

        seq1.Append(popUpPanel.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0));

        seq1.Append(popUpPanel.transform.DOScale(new Vector3(1, 1, 1), 1).SetEase(Ease.OutElastic));

        SetQusetSlot();
    }

    private void Update()
    {
        timeLeft.text = string.Format($"NEXT IN {questManager.hour}H {questManager.minute}M");
    }

    public void OnClickCloseBtn()
    {
        SoundManager.instance.PlaySound(6);
        GameManager.instance.Vibrate();
        popUpPanel.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 0.2f).OnComplete(() => gameObject.SetActive(false));
        backPanel.DOFade(0, 0.2f);
        gameObject.SetActive(false);
    }

    public void OnClickNewQuestBtn()
    {
        SoundManager.instance.PlaySound(6);
        GameManager.instance.Vibrate();
        uIBuyNewQuestsPage.Popup();
        uIBuyNewQuestsPage.buyBtn.onClick.AddListener(SetQusetSlot);
    }

    public void SetQusetSlot()
    {
        for (int i = 0; i < 3; i++)
        {
            questSlots[i].SetQusetSlot(questManager.nowQuests[i]);
        }
    }

    public void CheckNotifi()
    {
        SetQusetSlot();

        for (int i = 0; i < 3; i++)
        {
            if (questSlots[i].canGetReward)
            {
                notifIcon.SetActive(true);
                return;
            }
        }
        notifIcon.SetActive(false);
    }
}
