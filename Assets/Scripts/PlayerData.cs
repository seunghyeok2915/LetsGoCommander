using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeEnum
{
    SQUAD,
    DAMAGE,
    HEALTH
}

public class PlayerData : MonoBehaviour
{
    public UIHome uiHome;

    public int gold;
    public int ruby;

    public int squadLevel;
    public int damageLevel;
    public int healthLevel;

    private int squadUpgradeCost;
    private int damageUpgradeCost;
    private int healthUpgradeCost;

    private void Start()
    {
        uiHome.Init(this);

        uiHome.SetGold(gold, ruby);

        uiHome.SetUpgradeBtn(squadLevel, GetUpgradeCost(UpgradeEnum.SQUAD), UpgradeEnum.SQUAD);
        uiHome.SetUpgradeBtn(damageLevel, GetUpgradeCost(UpgradeEnum.DAMAGE), UpgradeEnum.DAMAGE);
        uiHome.SetUpgradeBtn(healthLevel, GetUpgradeCost(UpgradeEnum.HEALTH), UpgradeEnum.HEALTH);
    }

    public bool UpgradeLevel(UpgradeEnum upgradeEnum)
    {
        int costGold = 0;
        costGold = GetUpgradeCost(upgradeEnum);
        if (gold >= costGold)
        {

            gold -= costGold;
            uiHome.SetGold(gold, ruby);

            switch (upgradeEnum)
            {
                case UpgradeEnum.SQUAD:
                    squadLevel++;
                    uiHome.SetUpgradeBtn(squadLevel, GetUpgradeCost(UpgradeEnum.SQUAD), UpgradeEnum.SQUAD);
                    break;
                case UpgradeEnum.DAMAGE:
                    damageLevel++;
                    uiHome.SetUpgradeBtn(damageLevel, GetUpgradeCost(UpgradeEnum.DAMAGE), UpgradeEnum.DAMAGE);
                    break;
                case UpgradeEnum.HEALTH:
                    healthLevel++;
                    uiHome.SetUpgradeBtn(healthLevel, GetUpgradeCost(UpgradeEnum.HEALTH), UpgradeEnum.HEALTH);
                    break;
                default:
                    Debug.LogError("Unknown Type");
                    break;
            }
            return true;
        }
        else return false;


    }

    public int GetUpgradeCost(UpgradeEnum upgradeEnum)
    {
        int cost = 0;
        switch (upgradeEnum)
        {
            case UpgradeEnum.SQUAD:
                cost = squadLevel * 2;
                break;
            case UpgradeEnum.DAMAGE:
                cost = damageLevel * 2;
                break;
            case UpgradeEnum.HEALTH:
                cost = healthLevel * 2;
                break;
            default:
                Debug.LogError("Unknown Type");
                break;
        }
        return cost;
    }
}
