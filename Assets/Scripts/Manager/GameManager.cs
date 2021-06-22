using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.IO;
using UnityEngine.SceneManagement;

public enum UpgradeEnum
{
    SQUAD,
    DAMAGE,
    HEALTH
}

[System.Serializable]
public class PlayerData
{
    public int currentStage;

    public int gold;
    public int ruby;

    public int squadLevel;
    public int damageLevel;
    public int healthLevel;
}

public class GameManager : MonoBehaviour
{
    public static bool bPlayingGame = false;

    public List<MapData> mapList = new List<MapData>();
    public GroupManager groupManager;

    public UIHome uiHome;
    public GameObject uiInGame;
    public UIEndPage uiEndPage;

    public float judgeDelay = 0.3f; // 판단 딜레이
    private WaitForSeconds ws;

    public int currentStage;

    public int gold = 2;
    public int ruby = 10;

    public int squadLevel = 1;
    public int damageLevel = 1;
    public int healthLevel = 1;

    private int squadUpgradeCost;
    private int damageUpgradeCost;
    private int healthUpgradeCost;

    private MapData mapData;
    private MapManager mapManager;

    private void Awake()
    {
        LoadData();
        mapManager = GetComponent<MapManager>();
        ws = new WaitForSeconds(judgeDelay);
    }

    private void Start()
    {
        bPlayingGame = false;

        CreateMap();
        mapData.GetEnemyList();
        mapData.EnemyBalance(currentStage);

        UIInit();
        SpawnSoldiers();

        CameraManager.transposer.m_FollowOffset = new Vector3(0, 7, -7.33f);

        SoundManager.instance.SetBGM(0);
    }

    void CreateMap()
    {
        var map = Instantiate(mapList[currentStage - 1], transform.position, Quaternion.identity, transform);
        mapData = map;

        mapManager._mapPrefab = map.gameObject;
        mapManager.GenerateNavmesh();
    }
    void SpawnSoldiers()
    {
        groupManager.transform.position = mapData.playerSpawnPos.position;

        int soldierLevel1 = squadLevel % 7;
        int soldierLevel2 = squadLevel / 7;
        int soldierLevel3 = soldierLevel2 / 5;

        for (int i = 0; i < soldierLevel1; i++)
        {
            groupManager.MakeSoldier(1);
        }

        for (int i = 0; i < soldierLevel2; i++)
        {
            groupManager.MakeSoldier(2);
        }

        for (int i = 0; i < soldierLevel3; i++)
        {
            groupManager.MakeSoldier(3);
        }
        SetSoldiers();
    }

    void SetSoldiers()
    {
        foreach (var item in groupManager.soldierAgents)
        {
            item.InitStatus(damageLevel, healthLevel);
        }
    }

    public static void LoadScene(string sceneName) // 로드 씬
    {
        LoadingSceneManager.LoadScene(sceneName);
    }

    public void PlayGame() //처음 화면에서 클릭 할때 불려진다.
    {
        StartCoroutine(StartGame());
        mapData.bossAreaCheck.onTriggerEnter.AddListener(() => StartCoroutine(SpawnBoss()));

        DOTween.To(() => CameraManager.transposer.m_FollowOffset, x => CameraManager.transposer.m_FollowOffset = x, new Vector3(0, 15, -10), 2f);
    }

    public void GetGold(int gold)
    {
        this.gold += gold;
    }

    private void UIInit()
    {
        uiHome.Init(this);

        uiHome.SetGold(gold, ruby);

        uiHome.SetUpgradeBtn(squadLevel, GetUpgradeCost(UpgradeEnum.SQUAD), UpgradeEnum.SQUAD);
        uiHome.SetUpgradeBtn(damageLevel, GetUpgradeCost(UpgradeEnum.DAMAGE), UpgradeEnum.DAMAGE);
        uiHome.SetUpgradeBtn(healthLevel, GetUpgradeCost(UpgradeEnum.HEALTH), UpgradeEnum.HEALTH);
    }

    public IEnumerator StartGame()
    {
        SoundManager.instance.SetBGM(1);
        StartCoroutine(CheckGameEnd());

        bPlayingGame = true;
        while (mapData.enemyList.Count > 0)
        {

            yield return ws;
        }

        mapData.bossAreaCheck.gameObject.SetActive(true);
        yield return ws;
    }

    IEnumerator CheckGameEnd()
    {
        while (true)
        {
            if (bPlayingGame)
            {
                if (groupManager.CheckGameEnd())
                {
                    EndGame();
                }
            }
            yield return ws;
        }
    }

    private void EndGame()
    {
        uiEndPage.SetUIEndPage(this, false);
        uiInGame.gameObject.SetActive(false);

        DOTween.To(() => CameraManager.transposer.m_FollowOffset, x => CameraManager.transposer.m_FollowOffset = x, new Vector3(0, 4, -10), 2f);
        bPlayingGame = false;
    }

    public IEnumerator SpawnBoss() //BossArea 에 들어오면 불림
    {
        groupManager.transform.DOMove(mapData.playerCenterPos.position, 1.5f).OnComplete(() => groupManager.groupMovement.SetDirctionZero());

        DOTween.To(() => CameraManager.transposer.m_FollowOffset, x => CameraManager.transposer.m_FollowOffset = x, new Vector3(0, 10, -10), 0.5f);

        uiInGame.SetActive(false); // 조이스틱 끄기
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < mapData.enemySpawnCount; i++) // 적 스폰
        {
            EnemyAgent enemy = null;

            var rand = Random.Range(0, 100);
            if (rand < 50)
                enemy = PoolManager.GetItem<Zombie>().GetComponent<EnemyAgent>();
            else if (rand < 90)
                enemy = PoolManager.GetItem<ShootZombie>().GetComponent<EnemyAgent>();
            else
                enemy = PoolManager.GetItem<BigZombie>().GetComponent<EnemyAgent>();

            mapData.enemyList.Add(enemy);
            enemy.onDeath.AddListener(() => mapData.enemyList.Remove(enemy));
            enemy.transform.position = mapData.enemySpawnPos.position;
            yield return new WaitForSeconds(mapData.enemySpawnDelay);

        }
        mapData.EnemyBalance(currentStage);


        while (mapData.enemyList.Count > 0) // 적이 다 죽었는지 체크
        {
            yield return ws;
        }

        groupManager.DancePlayer();

        DOTween.To(() => CameraManager.transposer.m_FollowOffset, x => CameraManager.transposer.m_FollowOffset = x, new Vector3(0, 4, -10), 2f);

        uiEndPage.SetUIEndPage(this, true); //End 화면 구현
        bPlayingGame = false;
        yield return ws;
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

                    groupManager.MakeSoldier(1);
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
            SetSoldiers();
            SaveData();
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
                cost = (int)(squadLevel * Mathf.Pow(1.07f, squadLevel - 1));
                break;
            case UpgradeEnum.DAMAGE:
                cost = (int)(damageLevel * Mathf.Pow(1.07f, damageLevel - 1));
                break;
            case UpgradeEnum.HEALTH:
                cost = (int)(healthLevel * Mathf.Pow(1.07f, healthLevel - 1));
                break;
            default:
                Debug.LogError("Unknown Type");
                break;
        }
        return cost;
    }

    public void SaveData()
    {
        PlayerData playerData = new PlayerData
        {
            currentStage = this.currentStage,
            gold = this.gold,
            ruby = this.ruby,

            squadLevel = this.squadLevel,
            damageLevel = this.damageLevel,
            healthLevel = this.healthLevel,
        };

        string str = JsonUtility.ToJson(playerData);

        File.WriteAllText(Application.persistentDataPath + "/PlayerData.json", str);
    }

    void LoadData()
    {
        var path = $"{Application.persistentDataPath}/PlayerData.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);

            currentStage = playerData.currentStage;

            gold = playerData.gold;
            ruby = playerData.ruby;

            squadLevel = playerData.squadLevel;
            damageLevel = playerData.damageLevel;
            healthLevel = playerData.healthLevel;
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
            };

            string str = JsonUtility.ToJson(playerData);

            File.WriteAllText(Application.persistentDataPath + "/PlayerData.json", str);
            LoadData();
        }
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

}
