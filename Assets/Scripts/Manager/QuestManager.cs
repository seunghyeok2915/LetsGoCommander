using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Quest
{
    public int index;
    public int variableIndex;

    public string name;
    public string description;

    public int count;
    public int rewardGold;
}

public class QuestManager : MonoBehaviour
{
    public List<Quest> quests;
    public Quest[] nowQuests = new Quest[3];

    public TextAsset questText;

    public int leftTime;

    public int hour, minute;

    int lineSize, rowSize;

    private void Start()
    {
        string currentText = questText.text.Substring(0, questText.text.Length - 1);
        string[] line = currentText.Split('\n');

        lineSize = line.Length;
        rowSize = line[0].Split('\t').Length;

        for (int i = 0; i < lineSize; i++)
        {
            Quest quest = new Quest();

            string[] row = line[i].Split('\t');

            quest.index = int.Parse(row[0]);
            quest.variableIndex = int.Parse(row[1]);
            quest.name = row[2];
            quest.description = row[3];
            quest.count = int.Parse(row[4]);
            quest.rewardGold = int.Parse(row[5]);

            quests.Add(quest);
        }
        //PickRandomQuests();
        Invoke("LoadQuests", 0.1f);
    }

    public void LoadQuests()
    {
        for (int i = 0; i < 3; i++) //불러오기
        {
            var index = GameManager.instance.questIndex[i] - 1;
            nowQuests[i] = quests[index];
        }
    }

    private void Update()
    {
        leftTime = 86400 - Mathf.Abs(GameManager.instance.questSetTime - Utils.GetUnixTime());
        if (leftTime < 0)
        {
            PickRandomQuests();
        }

        hour = leftTime / 3600;
        minute = leftTime % 1440 / 60;
    }

    public void PickRandomQuests(bool changeSetTime = true)
    {
        for (int i = 0; i < 3; i++)
        {
            var index = i + Random.Range(0, 3) * 3;
            nowQuests[i] = quests[index];

            GameManager.instance.questIndex[i] = quests[index].index;
            GameManager.instance.hasGetQuestReward[i] = false;
        }

        GameManager.instance.killCount = 0;
        GameManager.instance.getGoldCount = 0;
        GameManager.instance.stageClearCount = 0;

        if (changeSetTime)
            GameManager.instance.questSetTime = Utils.GetUnixTime();

        DataManager.SaveData();

        LoadQuests();
    }
}