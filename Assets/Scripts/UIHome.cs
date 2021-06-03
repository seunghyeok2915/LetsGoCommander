using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHome : MonoBehaviour
{
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

    public void Init(PlayerData playerData)
    {
        squadBtn.onClick.AddListener(() => playerData.UpgradeLevel(UpgradeEnum.SQUAD));
        damageBtn.onClick.AddListener(() => playerData.UpgradeLevel(UpgradeEnum.DAMAGE));
        healthBtn.onClick.AddListener(() => playerData.UpgradeLevel(UpgradeEnum.HEALTH));
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
