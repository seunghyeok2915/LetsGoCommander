using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Advertisements;
using GooglePlayGames;
using GooglePlayGames.BasicApi;


public class UIEndPage : MonoBehaviour, IUnityAdsListener
{
    public Image winImage;
    public Image loseImage;

    public Text normalGold;
    public Text addGold;

    public Button claimBtn;
    public Button addClaimBtn;

    private Sequence sequence;
    private int getGold;

    public string mySurfacingId = "stageReward";

    private string gameId = "4193705";


    private void Start()
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, false);
    }

    private void Update()
    {
        addClaimBtn.interactable = Advertisement.IsReady(mySurfacingId);
    }

    public void SetUIEndPage(GameManager gameManager, bool victoryStatus)
    {
        gameObject.SetActive(true);
        SoundManager.instance.PlaySound(0);

        getGold = 0;

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
            DataManager.SaveData();
            GameManager.instance.Vibrate();
            SoundManager.instance.PlaySound(6);
            CallNewScene();
        });

        addClaimBtn.onClick.AddListener(() =>
        {
            if (victoryStatus)
                gameManager.currentStage++;
            GameManager.instance.Vibrate();
            SoundManager.instance.PlaySound(6);
            if (gameManager.adRemoval)
            {
                GameManager.instance.GetGold(getGold * 2);
                DataManager.SaveData();
                Debug.LogWarning("ad Finish");
                CallNewScene();
            }
            else
            {
                ShowRewardedVideo();
            }


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

    private void CallNewScene()
    {
        LoadingSceneManager.LoadScene("MainScene");
    }

    private void ShowRewardedVideo()
    {
        Advertisement.Show(mySurfacingId);
        Debug.Log("AD CALLED");
    }


    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsReady(string surfacingId)
    {
        // If the ready Ad Unit or legacy Placement is rewarded, activate the button: 
        if (surfacingId == mySurfacingId)
        {
            addClaimBtn.interactable = true;
        }
    }

    public void OnUnityAdsDidFinish(string surfacingId, ShowResult showResult)
    {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished)
        {
            GameManager.instance.GetGold(getGold * 2);
            Debug.LogWarning("ad Finish");
            DataManager.SaveData();
            CallNewScene();
        }
        else if (showResult == ShowResult.Skipped)
        {
            // Do not reward the user for skipping the ad.
            Debug.LogWarning("ad Skipped");
            DataManager.SaveData();
            CallNewScene();
        }
        else if (showResult == ShowResult.Failed)
        {
            Debug.LogWarning("The ad did not finish due to an error.");
            DataManager.SaveData();
            CallNewScene();
        }
    }

    public void OnUnityAdsDidError(string message)
    {
        // Log the error.
    }

    public void OnUnityAdsDidStart(string surfacingId)
    {
        // Optional actions to 
    }


}
