using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public enum UpgradeEnum
{
    SQUAD,
    DAMAGE,
    HEALTH
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool bPlayingGame = false;

    public List<MapData> mapList = new List<MapData>();
    public GroupManager groupManager;

    public UIHome uiHome;
    public UIInGame uiInGame;
    public UIEndPage uiEndPage;
    public UIOfflineIncome uIOfflineIncome;

    public float judgeDelay = 0.3f; // 판단 딜레이
    private WaitForSeconds ws;

    public int currentStage;

    public int gold = 2;
    public int ruby = 10;

    public int squadLevel = 1;
    public int damageLevel = 1;
    public int healthLevel = 1;

    public bool sound = true;
    public bool haptic = true;

    public int outUnixTime;
    public int questSetTime;

    public int killCount;
    public int getGoldCount;
    public int stageClearCount;

    public int[] questIndex;
    public bool[] hasGetQuestReward;

    private int squadUpgradeCost;
    private int damageUpgradeCost;
    private int healthUpgradeCost;

    private MapData mapData;
    private MapManager mapManager;

    private void Awake()
    {
        instance = this;
        mapManager = GetComponent<MapManager>();
        ws = new WaitForSeconds(judgeDelay);
    }

    private void Start()
    {
        DataManager.LoadData();

        int offEarnTime = Utils.GetUnixTime() - outUnixTime;
        if (offEarnTime > 7200)
        {
            uIOfflineIncome.Popup(offEarnTime);
        }

        bPlayingGame = false;

        CreateMap();

        mapData.GetEnemyList();
        mapData.EnemyBalance(currentStage);


        UIInit();
        SpawnSoldiers();

        CameraManager.transposer.m_FollowOffset = new Vector3(0, 7, -7.33f);

        SoundManager.instance.SetBGM(0);
    }

    private void CreateMap()
    {
        try
        {
            var map = Instantiate(mapList[currentStage - 1], transform.position, Quaternion.identity, transform);
            mapData = map;

            mapManager._mapPrefab = map.gameObject;
            mapManager.GenerateNavmesh();
        }
        catch
        {
            currentStage = 1;

            gold = 2;
            ruby = 10;

            squadLevel = 1;
            damageLevel = 1;
            healthLevel = 1;

            var map = Instantiate(mapList[currentStage - 1], transform.position, Quaternion.identity, transform);
            mapData = map;

            mapManager._mapPrefab = map.gameObject;
            mapManager.GenerateNavmesh();
        }
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

    private void SetSoldiers()
    {
        for (var i = 0; i < groupManager.soldierAgents.Count; i++)
        {
            groupManager.soldierAgents[i].InitStatus(damageLevel, healthLevel);
        }
    }

    public void LoadScene(string sceneName) // 로드 씬
    {
        LoadingSceneManager.LoadScene(sceneName);
    }

    public void PlayGame() //처음 화면에서 클릭 할때 불려진다.
    {
        StartCoroutine(StartGame());

        uiInGame.Show(mapData.isBoss);

        if (!mapData.isBoss)
            mapData.bossAreaCheck.onTriggerEnter.AddListener(() => StartCoroutine(SpawnBoss()));

        CameraManager.SetCameraTarget(groupManager.centerSlot.soldier.transform);

        DOTween.To(() => CameraManager.transposer.m_FollowOffset, x => CameraManager.transposer.m_FollowOffset = x, new Vector3(0, 15, -10), 2f);
    }

    public void GetGold(int gold)
    {
        this.gold += gold;
        getGoldCount += gold;

        uiHome.SetGold(this.gold, ruby);
    }

    private void UIInit()
    {
        uiHome.Init(this);

        uiHome.SetGold(gold, ruby);

        uiHome.SetUpgradeBtn(squadLevel, GetUpgradeCost(UpgradeEnum.SQUAD), UpgradeEnum.SQUAD);
        uiHome.SetUpgradeBtn(damageLevel, GetUpgradeCost(UpgradeEnum.DAMAGE), UpgradeEnum.DAMAGE);
        uiHome.SetUpgradeBtn(healthLevel, GetUpgradeCost(UpgradeEnum.HEALTH), UpgradeEnum.HEALTH);
    }

    private IEnumerator StartGame()
    {
        SoundManager.instance.SetBGM(1);
        StartCoroutine(CheckGameEnd());

        bPlayingGame = true;
        while (mapData.enemyList.Count > 0)
        {
            if (mapData.isBoss)
            {
                uiInGame.SetBossInfoBar(mapData.boss);
            }
            uiInGame.SetEnemyLeftTxt(mapData.enemyList.Count);
            yield return ws;
        }
        uiInGame.SetEnemyLeftTxt(mapData.enemyList.Count);

        if (!mapData.isBoss)
            mapData.bossAreaCheck.gameObject.SetActive(true);
        else
            WinGame();


        yield return ws;
    }

    private IEnumerator CheckGameEnd()
    {
        while (true)
        {
            if (bPlayingGame)
            {
                if (groupManager.CheckGameEnd())
                {
                    LoseGame();
                }
            }
            yield return ws;
        }
    }

    private IEnumerator SpawnBoss() //BossArea 에 들어오면 불림
    {
        groupManager.transform.DOMove(mapData.bossAreaPlayerCenterPos.position, 1.5f).OnComplete(() => groupManager.groupMovement.SetDirctionZero());
        CameraManager.SetCameraTarget(groupManager.transform);
        DOTween.To(() => CameraManager.transposer.m_FollowOffset, x => CameraManager.transposer.m_FollowOffset = x, new Vector3(0, 10, -10), 0.5f);

        uiInGame.gameObject.SetActive(false); // 조이스틱 끄기
        yield return new WaitForSeconds(2f);
        for (var i = 0; i < mapData.enemySpawnCount; i++) // 적 스폰
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

        WinGame();
        yield return ws;
    }

    private void WinGame()
    {
        groupManager.DancePlayer();

        DOTween.To(() => CameraManager.transposer.m_FollowOffset, x => CameraManager.transposer.m_FollowOffset = x, new Vector3(0, 4, -10), 2f);

        uiEndPage.SetUIEndPage(this, true); //End 화면 구현
        uiInGame.gameObject.SetActive(false);

        stageClearCount++;

        bPlayingGame = false;
    }

    private void LoseGame()
    {
        uiEndPage.SetUIEndPage(this, false);
        uiInGame.gameObject.SetActive(false);

        DOTween.To(() => CameraManager.transposer.m_FollowOffset, x => CameraManager.transposer.m_FollowOffset = x, new Vector3(0, 4, -10), 2f);
        bPlayingGame = false;
    }

    public void UpgradeLevel(UpgradeEnum upgradeEnum)
    {
        var costGold = 0;
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
            DataManager.SaveData();
        }
    }

    public void Vibrate()
    {
        if (haptic)
            Handheld.Vibrate();
    }

    private int GetUpgradeCost(UpgradeEnum upgradeEnum)
    {
        var cost = 0;
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

    public bool TryUseRuby(int cost)
    {
        if (ruby >= cost)
        {
            ruby -= cost;
            uiHome.SetGold(gold, ruby);
            return true;
        }

        return false;
    }

    private void OnApplicationFocus(bool focusStatus)
    {
        if (!focusStatus)
            DataManager.SaveData();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            DataManager.SaveData();
    }

    private void OnApplicationQuit()
    {
        DataManager.SaveData();
    }

}
