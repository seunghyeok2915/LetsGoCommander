using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class PlayerData
{
    public int currentStage;

    public int gold;
    public int ruby;

    public int squadLevel;
    public int damageLevel;
    public int healthLevel;

    public bool sound;
    public bool haptic;

    public int killCount;
    public int getGoldCount;
    public int stageClearCount;

    public int outUnixTime;
    public int questSetTime;

    public int[] questIndex = new int[3];
    public bool[] hasGetQuestReward = new bool[3];

    public bool adRemoval;
}

public class DataManager : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(this);
    }
    public static void SaveData()
    {
        PlayerData playerData = new PlayerData
        {
            currentStage = GameManager.instance.currentStage,
            gold = GameManager.instance.gold,
            ruby = GameManager.instance.ruby,

            squadLevel = GameManager.instance.squadLevel,
            damageLevel = GameManager.instance.damageLevel,
            healthLevel = GameManager.instance.healthLevel,

            sound = GameManager.instance.sound,
            haptic = GameManager.instance.haptic,

            killCount = GameManager.instance.killCount,
            getGoldCount = GameManager.instance.getGoldCount,
            stageClearCount = GameManager.instance.stageClearCount,

            outUnixTime = Utils.GetUnixTime(),

            questSetTime = GameManager.instance.questSetTime,

            questIndex = GameManager.instance.questIndex,
            hasGetQuestReward = GameManager.instance.hasGetQuestReward,

            adRemoval = GameManager.instance.adRemoval
        };

        string str = JsonUtility.ToJson(playerData);

        File.WriteAllText(Application.persistentDataPath + "/PlayerData.json", str);
    }

    public static void LoadData()
    {
        var path = $"{Application.persistentDataPath}/PlayerData.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);

            GameManager.instance.currentStage = playerData.currentStage;

            GameManager.instance.gold = playerData.gold;
            GameManager.instance.ruby = playerData.ruby;

            GameManager.instance.squadLevel = playerData.squadLevel;
            GameManager.instance.damageLevel = playerData.damageLevel;
            GameManager.instance.healthLevel = playerData.healthLevel;

            GameManager.instance.sound = playerData.sound;
            GameManager.instance.haptic = playerData.haptic;

            GameManager.instance.killCount = playerData.killCount;
            GameManager.instance.getGoldCount = playerData.getGoldCount;
            GameManager.instance.stageClearCount = playerData.stageClearCount;

            GameManager.instance.outUnixTime = playerData.outUnixTime;
            GameManager.instance.questSetTime = playerData.questSetTime;

            GameManager.instance.questIndex = playerData.questIndex;
            GameManager.instance.hasGetQuestReward = playerData.hasGetQuestReward;

            GameManager.instance.adRemoval = playerData.adRemoval;
        }
        else
        {
            PlayerData playerData = new PlayerData
            {
                currentStage = 1,
                gold = 2,
                ruby = 10,

                squadLevel = 1,
                damageLevel = 1,
                healthLevel = 1,

                sound = true,
                haptic = true,

                killCount = 0,
                getGoldCount = 0,
                stageClearCount = 0,

                outUnixTime = Utils.GetUnixTime(),
                questSetTime = 0,
                questIndex = new int[3],
                hasGetQuestReward = new bool[3],

                adRemoval = false
            };

            string str = JsonUtility.ToJson(playerData);

            File.WriteAllText(Application.persistentDataPath + "/PlayerData.json", str);
            LoadData();
        }
    }

    [ContextMenu("ResetData")]
    public void ResetDataFunc()
    {
        ResetData();
        LoadingSceneManager.LoadScene("MainScene");
    }

    public static void ResetData()
    {
        PlayerData playerData = new PlayerData
        {
            currentStage = 1,
            gold = 2,
            ruby = 10,

            squadLevel = 1,
            damageLevel = 1,
            healthLevel = 1,

            sound = true,
            haptic = true,

            killCount = 0,
            getGoldCount = 0,
            stageClearCount = 0,

            outUnixTime = Utils.GetUnixTime(),
            questSetTime = 0,
            questIndex = new int[3],
            hasGetQuestReward = new bool[3],

            adRemoval = false
        };

        string str = JsonUtility.ToJson(playerData);

        File.WriteAllText(Application.persistentDataPath + "/PlayerData.json", str);
        LoadingSceneManager.LoadScene("MainScene");
    }
}
