using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIBuyNewQuestsPage : MonoBehaviour
{
    public QuestManager questManager;

    public int buyNewQuestRubyCost;
    public Text rubyCostTxt;

    public Button buyBtn;
    public Button buyNoBtn;

    private Sequence seq1;

    private void Start()
    {
        buyBtn.onClick.AddListener(OnClickBuyBtn);
        buyNoBtn.onClick.AddListener(OnClickNoBuyBtn);
    }

    public void Popup()
    {
        gameObject.SetActive(true);

        seq1.Kill();
        seq1 = DOTween.Sequence();

        seq1.Append(gameObject.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0));

        seq1.Append(gameObject.transform.DOScale(new Vector3(1, 1, 1), 1).SetEase(Ease.OutElastic));

        rubyCostTxt.text = buyNewQuestRubyCost.ToString();
    }

    private void OnClickBuyBtn()
    {
        if (GameManager.instance.TryUseRuby(buyNewQuestRubyCost))
        {
            questManager.PickRandomQuests(false);
            gameObject.SetActive(false);
        }
    }

    private void OnClickNoBuyBtn()
    {
        gameObject.SetActive(false);
    }
}
