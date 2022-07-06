using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using UnityEngine.SceneManagement;
using LitJson;

public class LoginScript : MonoBehaviour
{
    private ModalPanel modalPanel;
    public InputField loginCustomid;
    public InputField loginCustompw;

    public InputField signupCustomid;
    public InputField signupCustompw;

    public InputField nickname;

    public GameObject loginPanel;
    public GameObject signupPanel;
    public GameObject nicknamePanel;
    enum Panel
    {
        login,
        signup,
        nickname
    }
    Panel currentPanel;

    int sceneIdx;
    private void Awake()
    {
        // 오류 모달
        modalPanel = ModalPanel.Instance();
    }

    void Start()
    {
        Screen.SetResolution(Screen.width, Screen.height, true);

        sceneIdx = SceneManager.GetActiveScene().buildIndex;
        if (!Backend.IsInitialized)
        {
            Backend.Initialize(InitializeCallback);
        }
        else
        {
            InitializeCallback();
        }

        ActivePanel(Panel.login);
    }

    private void ActivePanel(Panel panel)
    {
        switch(panel)
        {
            case Panel.login:
                loginPanel.SetActive(true);
                signupPanel.SetActive(false);
                nicknamePanel.SetActive(false);
                break;

            case Panel.signup:
                loginPanel.SetActive(false);
                signupPanel.SetActive(true);
                nicknamePanel.SetActive(false);
                break;

            case Panel.nickname:
                loginPanel.SetActive(false);
                signupPanel.SetActive(false);
                nicknamePanel.SetActive(true);
                break;
        }
        currentPanel = panel;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentPanel == Panel.login)
                Application.Quit();
            else
                ActivePanel(Panel.login);
        }
    }

    private void InitializeCallback()
    {
        if(Backend.IsInitialized)
        {
            Debug.Log(Backend.Utils.GetServerTime());
        }
    }

    public void Login()
    {
        string id = loginCustomid.text;
        string pw = loginCustompw.text;


        BackendReturnObject loginResult = Backend.BMember.GuestLogin();

        // 로그인 성공시 채널목록으로 이동
        if (loginResult.IsSuccess())
        {
            Debug.Log("login success");
            if (DoesNicknameExist())
            {
                SceneManager.LoadScene("ChatTest-channellist");
            }
            else
            {
                ActivePanel(Panel.nickname);
            }
        }
        else
        {
            FailedModalShow("로그인에 실패했습니다.\n" + loginResult.GetMessage());
        }
    }

    private bool DoesNicknameExist()
    {
        BackendReturnObject bro = Backend.BMember.GetUserInfo();
        if(bro.IsSuccess())
        {
            JsonData nicknameJson = bro.GetReturnValuetoJSON()["row"]["nickname"];

            // 닉네임 여부를 확인
            return nicknameJson != null;
        }

        return false;
    }


    public void GoSignUp()
    {
        ActivePanel(Panel.signup);
    }

    private void GoNickname()
    {
        ActivePanel(Panel.nickname);
    }

    public void SignUp()
    {
        string id = signupCustomid.text;
        string pw = signupCustompw.text;

        if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(pw))
        {
            BackendReturnObject SignUpResult = Backend.BMember.CustomSignUp(id, pw);
            Debug.Log(SignUpResult);

            // 로그인 성공시 채널목록으로 이동
            if (SignUpResult.IsSuccess())
            {
                ActivePanel(Panel.nickname);
                Debug.Log("signup success");
            }
            else
            {
                FailedModalShow(SignUpResult.GetMessage());
            }
        }
        else
        {
            FailedModalShow("ID 혹은 PW를 입력해주세요.");
        }
    }


    public void SetNickname()
    {
        string nick = nickname.text;
        BackendReturnObject nicknameCreated = Backend.BMember.CreateNickname(nick);
        if(nicknameCreated.IsSuccess())
        {
            SceneManager.LoadScene("ChatTest-channellist");
        }
        else
        {
            FailedModalShow(nicknameCreated.GetMessage());
        }
    }


    void FailedModalShow(string message)
    {
        modalPanel.AlertShow(message);
    }
}
