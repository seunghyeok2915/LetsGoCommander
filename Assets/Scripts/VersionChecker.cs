using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using DG.Tweening;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net;

public class VersionChecker : MonoBehaviour
{
    public LoadingSceneManager loadingSceneManager;

    public string openURL = "";
    public string versionURL = "";
    string latsetVersion;
    public GameObject newVersionAvailable;
    public Button downloadBtn;
    public Text versionInfoTxt;

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }

    private void Start()
    {
        downloadBtn.onClick.AddListener(() => OpenURL(openURL));
        Instate();
        string marketVersion = "";

        HtmlWeb web = new HtmlWeb();
        HtmlDocument doc = web.Load(versionURL);

        foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//span[@class='htlgb']"))
        {
            marketVersion = node.InnerText.Trim();
            if (marketVersion != null)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(marketVersion, @"^\d{1}\.\d{1}\.\d{1}$") || System.Text.RegularExpressions.Regex.IsMatch(marketVersion, @"^\d{1}\.\d{1}\.\d{2}$"))
                {
                    Debug.Log("내 앱 버전 :" + Application.version);
                    Debug.Log("마켓 버전 : " + marketVersion);

                    string latsetVersion = marketVersion.ToString();

                    if (Application.version != latsetVersion)
                    {
                        versionInfoTxt.text = string.Format($"Current Version : {Application.version}\nLatest Version: {latsetVersion}");
                        newVersionAvailable.SetActive(true);
                        newVersionAvailable.transform.localScale = Vector3.zero;
                        newVersionAvailable.transform.DOScale(new Vector3(1, 1, 1), 1f).SetEase(Ease.OutElastic);
                    }
                    else
                    {
                        newVersionAvailable.SetActive(false);
                        StartCoroutine(loadingSceneManager.LoadScene());
                    }
                }
            }
        }
    }

    public bool Validator(
        object sender,
        X509Certificate certificate,
        X509Chain chain,
        SslPolicyErrors policyErrors)
    {

        //Just accept and move on...
        Debug.Log("Validation successful!");
        return true;
    }

    public void Instate()
    {
        ServicePointManager.ServerCertificateValidationCallback = Validator;
    }
}