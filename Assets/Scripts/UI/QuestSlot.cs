using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestSlot : MonoBehaviour
{
    public int variableIndex;

    public string questName;
    public string questDefinition;

    public int status;
    public int maxStatus;

    public int rewardGold;

    public Text nameTxt;
    public Text definitionTxt;

    public Text rewardTxt;

    public Text statusTxt;
    public Image fillImage;

    public Image getImage;

    public Button getRewardBtn;

    public bool hasGetReward;

    public void SetQusetSlot(Quest quest)
    {
        this.variableIndex = quest.variableIndex;

        this.questName = quest.name;
        this.questDefinition = quest.description;

        this.maxStatus = quest.count;

        this.rewardGold = quest.rewardGold;

        getRewardBtn.onClick.AddListener(OnClickGetBtn);

        nameTxt.text = questName;
        definitionTxt.text = questDefinition;

        rewardTxt.text = rewardGold.ToString();
    }

    private void Update()
    {
        hasGetReward = GameManager.instance.hasGetQuestReward[variableIndex - 1];

        switch (variableIndex)
        {
            case 1:
                status = GameManager.instance.killCount;
                break;
            case 2:
                status = GameManager.instance.getGoldCount;
                break;
            case 3:
                status = GameManager.instance.stageClearCount;
                break;
            default:
                Debug.LogError("Unknown Type");
                break;
        }

        if (gameObject.activeSelf == true)
        {
            fillImage.fillAmount = Mathf.Clamp01((float)status / (float)maxStatus);
            statusTxt.text = string.Format($"{status} / {maxStatus}");
        }

        if (status >= maxStatus && !hasGetReward)
        {
            getRewardBtn.interactable = true;
        }
        else
        {
            getRewardBtn.interactable = false;
        }

        if (hasGetReward)
        {
            getImage.gameObject.SetActive(true);
        }
        else
            getImage.gameObject.SetActive(false);
    }

    private void OnClickGetBtn()
    {
        GameManager.instance.GetGold(rewardGold);

        GameManager.instance.hasGetQuestReward[variableIndex - 1] = true;

        DataManager.SaveData();
    }
}
