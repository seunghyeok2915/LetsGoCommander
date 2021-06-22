using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIEndPage : MonoBehaviour
{
    public Image winImage;
    public Image loseImage;

    public Text normalGold;
    public Text addGold;

    public Button claimBtn;
    public Button addClaimBtn;

    private Sequence sequence;

    public void SetUIEndPage(GameManager gameManager, bool victoryStatus)
    {
        gameObject.SetActive(true);
        SoundManager.instance.PlaySound(0);

        int getGold = 0;

        if (victoryStatus)
        {
            getGold = (int)(gameManager.currentStage * Mathf.Pow(1.07f, gameManager.currentStage - 1));
            ActiveResult(true);
        }
        else
        {
            getGold = (int)(gameManager.currentStage * Mathf.Pow(1.07f, gameManager.currentStage - 1)) / 2;
            ActiveResult(false);
        }


        normalGold.text = string.Format($"+{getGold}");
        addGold.text = string.Format($"+{getGold * 2} Ad");

        claimBtn.onClick.AddListener(() =>
        {
            gameManager.GetGold(getGold);
            if (victoryStatus)
                gameManager.currentStage++;
            gameManager.SaveData();
            CallNewScene();
        });

        addClaimBtn.onClick.AddListener(() =>
        {
            gameManager.GetGold(getGold * 2);
            if (victoryStatus)
                gameManager.currentStage++;
            gameManager.SaveData();
            CallNewScene();
        });
    }

    void ActiveResult(bool win)
    {
        sequence.Kill();
        sequence = DOTween.Sequence();

        if (win)
        {
            winImage.gameObject.SetActive(true);
            loseImage.gameObject.SetActive(false);

            winImage.color = new Color(1, 1, 1, 0);
            winImage.DOFade(1f, 0.5f);
            sequence.Append(winImage.gameObject.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.2f));
            sequence.Append(winImage.gameObject.transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f));
        }
        else
        {
            winImage.gameObject.SetActive(false);
            loseImage.gameObject.SetActive(true);

            loseImage.color = new Color(1, 1, 1, 0);
            loseImage.DOFade(1f, 0.5f);
            sequence.Append(loseImage.gameObject.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.2f));
            sequence.Append(loseImage.gameObject.transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f));
        }
    }

    void CallNewScene()
    {
        LoadingSceneManager.LoadScene("MainScene");
    }
}
