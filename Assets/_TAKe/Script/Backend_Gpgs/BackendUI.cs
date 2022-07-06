using BackEnd;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BackendUI : MonoBehaviour
{
    public GameObject goImgLogoSSAP, goImgIntro;
    public CanvasGroup canLogoSSAP, canIntro;

    [SerializeField] GameObject pop_PopPrivacyPolicy;
    [SerializeField] GameObject pop_SignUpType;
    [SerializeField] GameObject pop_SignUpNickNameEnter;
    [SerializeField] GameObject pop_ServerStatus;
    [SerializeField] GameObject pop_UpdateVersion;

    [SerializeField] GameObject pop_Notice;
    [SerializeField] Text txNotice;
    [SerializeField] Button btnNoticeConfirm;

    public void IntroNotice(string s, UnityAction ua)
    {
        txNotice.text = s;
        btnNoticeConfirm.onClick.RemoveAllListeners();
        btnNoticeConfirm.onClick.AddListener(ua);
        pop_Notice.SetActive(true);
    }

    public IEnumerator CanvasLogo()
    {
        canLogoSSAP.alpha = 1.0f;
        canIntro.alpha = 0.0f;

        yield return new WaitForSeconds(2.0f);
        bool isIntroStart = false;
        while (canLogoSSAP.alpha > 0)
        {
            if (canLogoSSAP.alpha <= 0.2f && isIntroStart == false)
            {
                StartCoroutine(CanvasIntro());
                isIntroStart = true;
            }

            canLogoSSAP.alpha -= Time.deltaTime * 2;
            yield return null;
        }

        canLogoSSAP.alpha = 0.0f;
        while (canIntro.alpha < 1)
            yield return null;
    }

    IEnumerator CanvasIntro()
    {
        while (canIntro.alpha < 1)
        {
            canIntro.alpha += Time.deltaTime * 2;
            yield return null;
        }

        canIntro.alpha = 1.0f;
    }

    #region 서버 상태 알림 status 0 = 정상, 1 = 오프라인, 2 = 점검 
    public void PopServerStatus(int status) 
    {
        pop_ServerStatus.transform.GetChild(0).Find("status-1").gameObject.SetActive(false);
        pop_ServerStatus.transform.GetChild(0).Find("status-2").gameObject.SetActive(false);
        pop_ServerStatus.transform.GetChild(0).Find(string.Format("status-{0}", status)).gameObject.SetActive(true);
        pop_ServerStatus.SetActive(true);
    }
    #endregion

    #region 업데이트 알림 type 1 = 선택, 2 = 강제
    public void PopUpUpdateVersion(int type)
    {
        pop_UpdateVersion.transform.GetChild(0).Find("type-1").gameObject.SetActive(false);
        pop_UpdateVersion.transform.GetChild(0).Find("type-2").gameObject.SetActive(false);
        pop_UpdateVersion.transform.GetChild(0).Find(string.Format("type-{0}", type)).gameObject.SetActive(true);
        pop_UpdateVersion.SetActive(true);
    }
    public void Click_GoStore()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.two.people.zombie");
        Application.Quit();
    }
    public void Click_IgnoreUpdates()
    {
        pop_UpdateVersion.SetActive(false);
        BackendGpgsMng.GetInstance().LoginType();
    }
    #endregion

    #region # 개인정보 취급 방침
    // 개인 정보 취급 방침 팝업 
    public void PopUpPrivacyPolicy() => pop_PopPrivacyPolicy.SetActive(true);

    // 개인 정보 취급 방침 동의 
    public void ClickPrivacyPolicyYes ()
    {
        PlayerPrefs.SetString(PrefsKeys.prky_privacy_policy, "yes");
        Transform root = pop_SignUpType.transform.GetChild(0).transform;

        // 기기별 로그인 및 가입 유형 선택창 띄움 
#if UNITY_EDITOR
        root.Find("info").Find("signup_type_guest").gameObject.SetActive(true);
        root.Find("info").Find("signup_type_google").gameObject.SetActive(false);
        root.Find("info").Find("signup_type_ios").gameObject.SetActive(false);
#elif UNITY_ANDROID
        root.Find("info").Find("signup_type_guest").gameObject.SetActive(false);
        root.Find("info").Find("signup_type_google").gameObject.SetActive(true);
        root.Find("info").Find("signup_type_ios").gameObject.SetActive(false);
#elif  UNITY_IOS
        root.Find("info").Find("signup_type_guest").gameObject.SetActive(false);
        root.Find("info").Find("signup_type_google").gameObject.SetActive(false);
        root.Find("info").Find("signup_type_ios").gameObject.SetActive(true);
#endif

        pop_SignUpType.SetActive(true); // 가입(로그인) 방식 선택창 : 게스트, 구글, ios 
        pop_PopPrivacyPolicy.SetActive(false);
    }

    // 개인 정보 취급 방침 비 동의 
    public void ClickPrivacyPolicyNo() => AppQuit();


    public void AppQuit()
    {
        Application.Quit();
    }

    public void Click_Polocy1OpenURL()
    {
        //var bro = Backend.Notice.GetPolicy();
        //string url = bro.GetReturnValuetoJSON()["termsURL"].ToString();
        //LogPrint.Print("URL : " + url);
        //Application.OpenURL(url);

        Application.OpenURL("http://www.thebackend.io/cefb36c41ca31a636204b5a20146fa967d78aa9554a2d76d01e52ddb66b9a120/terms.html");
    }

    public async void Click_Polocy2OpenURL()
    {
        //var bro = Backend.Notice.GetPolicy();
        //string url = bro.GetReturnValuetoJSON()["privacyURL"].ToString();
        //LogPrint.Print("URL : " + url);
        //Application.OpenURL(url);

        Application.OpenURL("http://www.thebackend.io/cefb36c41ca31a636204b5a20146fa967d78aa9554a2d76d01e52ddb66b9a120/privacy.html");
    }
    #endregion

    #region # 가입 유형창에서 선택 후, 닉네임 입력창을 띄운후 닉네임 입력 후 가입시작 
    /// <summary> Guest, Google, Ios 중 시작하기 선택함 : 로그인 안되있을때 로그인 실행 후 가입이 안되있을 경우 닉네임 설정창이 뜨고 가입 되어있을 경우 데이터를 로드함 </summary>
    public void ClickStartType(string type)
    {
        LogPrint.PrintError("ClickStartType type : " + type);
        if (type == "guest")
        {
            BackendGpgsMng.GetInstance().ABackendLoginGuest();
        }
        else if(type == "google")
        {
            BackendGpgsMng.GetInstance().ABackendLoginGoogle();
        }
        else if(type == "ios")
        {

        }
        
        pop_SignUpType.SetActive(false);
    }

    /// <summary> 팝업 : 닉네임 설정 = guest, google, ios 중 가입에 사용할 타입 선택 </summary>
    public void PopUpNickName(string sign_type, bool overlap)
    {
        LogPrint.PrintError("PopUpSignUpNickNameCreate sign_type : " + sign_type);
        Transform root = pop_SignUpNickNameEnter.transform.GetChild(0).transform;
        root.Find("signup_nickname_confirm_guest").gameObject.SetActive(sign_type == "guest");
        root.Find("signup_nickname_confirm_google").gameObject.SetActive(sign_type == "google");
        root.Find("signup_nickname_confirm_ios").gameObject.SetActive(sign_type == "ios");
        root.GetComponentInChildren<InputField>().transform.Find("nickname_overlap").gameObject.SetActive(overlap);
        pop_SignUpNickNameEnter.gameObject.SetActive(true);
    }

    private bool IsNickNameError(string nickname)
    {
        if (nickname.Length <= 0)
        {
            LogPrint.PrintError("닉네임을 입력해주세요");
            return true;
        }
        else return false;
    }

    /// <summary> 디바이스 닉네임 설정 </summary>
    public void ClickStartNickNameCreateGuest(Text txt)
    {
        string nickname = txt.text.ToString().Trim();
        if (IsNickNameError(nickname))
            return;

        BackendGpgsMng.GetInstance().ACreateNickName("guest", nickname);
        pop_SignUpNickNameEnter.SetActive(false);
    }
    /// <summary> 안드로이드 닉네임 설정 </summary>
    public void ClickStartNickNameCreateGoogle(Text txt)
    {
        string nickname = txt.text.ToString().Trim();
        if (IsNickNameError(nickname))
            return;

        BackendGpgsMng.GetInstance().ACreateNickName("google", txt.text.ToString());
        pop_SignUpNickNameEnter.SetActive(false);
    }
    /// <summary> IOS  닉네임 설정 </summary>
    public void ClickStartNickNameCreateIos(Text txt)
    {
        string nickname = txt.text.ToString().Trim();
        if (IsNickNameError(nickname))
            return;

        BackendGpgsMng.GetInstance().ACreateNickName("ios", txt.text.ToString());
        pop_SignUpNickNameEnter.SetActive(false);
    }
    #endregion

    /// <summary> 뒤끝 데이터 정보 세팅이 완료되면 게임 씬으로 이동하도록 </summary>
    public void BackendWaitingToGameScene() => StartCoroutine("CBackendWaitingToGameScene");
    private IEnumerator CBackendWaitingToGameScene ()
    {
        while (!BackendGpgsMng.GetInstance().IsAllDataLoaded)
            yield return null;

        yield return null;

        yield return null;
        LoadSceneMng.GetInstance().sceneIndex = LoadSceneMng.SceneIndex.BattleMode;
        LoadSceneMng.GetInstance().LoadScene();
    }
}
