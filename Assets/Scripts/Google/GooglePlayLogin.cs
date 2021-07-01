using UnityEngine;
// Text UI 사용
using UnityEngine.UI;
// 구글 플레이 연동
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GooglePlayLogin : MonoBehaviour
{
    public Text email;
    public bool loginStaus;

    void Start()
    {
        PlayGamesPlatform.InitializeInstance(
 new PlayGamesClientConfiguration.Builder().Build());
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        // Google Play Games 활성화
        TryGoogleLogin();
    }

    [ContextMenu("Login")]
    public void TryGoogleLogin()
    {
        email.text = "Trying Google Login";
        if (!Social.localUser.authenticated) // 로그인 되어 있지 않다면
        {
            Social.localUser.Authenticate(success => // 로그인 시도
            {
                if (success) // 성공하면
                {
                    email.text = Social.localUser.userName;
                    loginStaus = true;
                }
                else // 실패하면
                {
                    email.text = "Login Fail Try Later";
                    loginStaus = false;
                }
            });
        }
    }

    [ContextMenu("Logout")]
    public void TryGoogleLogout()
    {
        if (Social.localUser.authenticated) // 로그인 되어 있다면
        {
            PlayGamesPlatform.Instance.SignOut(); // 로그아웃
            email.text = "Logout";
        }
    }
}