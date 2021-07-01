using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using UnityEngine.Purchasing;

public class UIHome : MonoBehaviour
{
    public Image stageImg;
    public Text stageText;

    public Button playerBtn;

    public Text goldTxt;
    public Text rubyTxt;

    public Button squadBtn;
    public Button damageBtn;
    public Button healthBtn;

    public Text squadLevelTxt;
    public Text damageLevelTxt;
    public Text healthLevelTxt;

    public Text squadCostTxt;
    public Text damageCostTxt;
    public Text healthCostTxt;

    public Button settingBtn;
    public Button questBtn;
    public Button resetBtn;
    public Button leaderBoardBtn;
    public IAPButton adRemovalIAPBtn;
    public Button adRemovalBtn;

    public UISettingPage settingPage;
    public UIQuestPage questPage;
    public UIResetPage resetPage;

    public void Init(GameManager gameManager)
    {
        stageImg.fillAmount = Mathf.Clamp01((float)gameManager.currentStage / 10);
        stageText.text = string.Format($"Stage 1 - {gameManager.currentStage}");

        squadBtn.onClick.AddListener(() =>
        {
            gameManager.UpgradeLevel(UpgradeEnum.SQUAD);
            GameManager.instance.Vibrate();
            SoundManager.instance.PlaySound(6);
        });
        damageBtn.onClick.AddListener(() =>
        {
            gameManager.UpgradeLevel(UpgradeEnum.DAMAGE);
            GameManager.instance.Vibrate();
            SoundManager.instance.PlaySound(6);
        });
        healthBtn.onClick.AddListener(() =>
        {
            gameManager.UpgradeLevel(UpgradeEnum.HEALTH);
            GameManager.instance.Vibrate();
            SoundManager.instance.PlaySound(6);
        });

        playerBtn.onClick.AddListener(() =>
        {
            gameManager.PlayGame();
            gameObject.SetActive(false);
            GameManager.instance.Vibrate();
            SoundManager.instance.PlaySound(6);
        });

        settingBtn.onClick.AddListener(() =>
        {
            settingPage.gameObject.SetActive(true);
            GameManager.instance.Vibrate();
            SoundManager.instance.PlaySound(6);
        });

        questBtn.onClick.AddListener(() =>
        {
            questPage.Popup();
            GameManager.instance.Vibrate();
            SoundManager.instance.PlaySound(6);
        });

        resetBtn.onClick.AddListener(() =>
        {
            resetPage.Popup();
            GameManager.instance.Vibrate();
            SoundManager.instance.PlaySound(6);
        });

        leaderBoardBtn.onClick.AddListener(() =>
        {
            Social.ShowLeaderboardUI();
            GameManager.instance.Vibrate();
            SoundManager.instance.PlaySound(6);
        });

        if (gameManager.adRemoval)
        {
            adRemovalBtn.interactable = false;
        }

        StartCoroutine(CheckQuestNoti());
    }

    private IEnumerator CheckQuestNoti()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.1f);
            questPage.CheckNotifi();
        }
    }

    public void BuyAdRemoval()
    {
        GameManager.instance.adRemoval = true;
        GameManager.instance.Vibrate();
        SoundManager.instance.PlaySound(6);
        DataManager.SaveData();
        adRemovalBtn.interactable = false;
    }



    public void SetGold(int gold, int ruby)
    {
        goldTxt.text = string.Format($"{gold}");
        rubyTxt.text = string.Format($"{ruby}");
    }

    public void SetUpgradeBtn(int level, int cost, UpgradeEnum upgradeEnum)
    {
        switch (upgradeEnum)
        {
            case UpgradeEnum.SQUAD:
                squadLevelTxt.text = string.Format($"LV.{level}");
                squadCostTxt.text = string.Format($"{cost}");
                break;
            case UpgradeEnum.DAMAGE:
                damageLevelTxt.text = string.Format($"LV.{level}");
                damageCostTxt.text = string.Format($"{cost}");
                break;
            case UpgradeEnum.HEALTH:
                healthLevelTxt.text = string.Format($"LV.{level}");
                healthCostTxt.text = string.Format($"{cost}");
                break;
            default:
                Debug.LogError("Unknown Type");
                break;
        }
    }
}
