using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIInGame : MonoBehaviour
{
    public GameObject enemyLeftUI;
    public Text enemyLeftTxt;

    public GameObject bossInfoUI;
    public Text hpPercentTxt;
    public Image hpFillbarImage;

    public GameObject bossAppearAnimPanel;

    public Button closeBtn;

    private bool isBoss;

    private void Start()
    {
        closeBtn.onClick.AddListener(OnClickCloseBtn);
    }

    public void Show(bool isBoss)
    {
        this.isBoss = isBoss;
        gameObject.SetActive(true);

        if (isBoss)
        {
            StartCoroutine(ShowBossAnim());
            enemyLeftUI.SetActive(false);
            bossInfoUI.SetActive(true);
        }
        else
        {
            enemyLeftUI.SetActive(true);
            bossInfoUI.SetActive(false);
        }
    }

    public void SetEnemyLeftTxt(int enemyCount)
    {
        enemyLeftTxt.text = string.Format($"EnemyLeft\n<color=#ff0000>{enemyCount}</color>");
    }

    public void SetBossInfoBar(LivingEntity boss)
    {
        float percent = boss.curHp / boss.maxHp;
        hpFillbarImage.fillAmount = Mathf.Clamp01(percent);
        hpPercentTxt.text = string.Format($"{percent * 100:F0}%");

    }

    private IEnumerator ShowBossAnim()
    {
        bossAppearAnimPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        bossAppearAnimPanel.SetActive(false);
    }

    private void OnClickCloseBtn()
    {
        GameManager.instance.LoadScene("MainScene");
        DataManager.SaveData();
    }
}
