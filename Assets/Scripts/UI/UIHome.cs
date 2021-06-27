using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
        });
        damageBtn.onClick.AddListener(() =>
        {
            gameManager.UpgradeLevel(UpgradeEnum.DAMAGE);
            GameManager.instance.Vibrate();
        });
        healthBtn.onClick.AddListener(() =>
        {
            gameManager.UpgradeLevel(UpgradeEnum.HEALTH);
            GameManager.instance.Vibrate();
        });

        playerBtn.onClick.AddListener(() =>
        {
            gameManager.PlayGame();
            gameObject.SetActive(false);
            GameManager.instance.Vibrate();
        });

        settingBtn.onClick.AddListener(() =>
        {
            settingPage.gameObject.SetActive(true);
            GameManager.instance.Vibrate();
        });

        questBtn.onClick.AddListener(() =>
        {
            questPage.Popup();
            GameManager.instance.Vibrate();
        });

        resetBtn.onClick.AddListener(() =>
        {
            resetPage.Popup();
            GameManager.instance.Vibrate();
        });
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
