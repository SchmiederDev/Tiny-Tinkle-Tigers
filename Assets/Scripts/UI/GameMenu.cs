using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject Menu;

    [SerializeField]
    private Button StartButton;

    [SerializeField]
    private Button HowToPlayBtn;

    [SerializeField]
    private Button QuitBtn;


    [SerializeField]
    private Button TinkleTigersButton;

    [SerializeField]
    private GameObject HowToPlay_Panel;

    [SerializeField]
    private TextAsset HowToPlay_File;

    [SerializeField]
    private TMP_Text HowToPlay_Text;

    [SerializeField]
    private Button HowToPlay_CancelBtn;

    private void Awake()
    {
        Activate_GameMenu();
        Init_HowToPlay_Panel();
        Init_BtnClickEvents();
    }

    public void Activate_GameMenu()
    {
        Menu.SetActive(true);
        DisableTinkleTigersBtn();
        Time.timeScale = 0f;
    }

    public void Deactivate_GameMenu()
    {
        Menu.SetActive(false);
        EnableTinkleTigersBtn();
        Time.timeScale = 1f;
    }

    private void Init_HowToPlay_Panel()
    {
        HowToPlay_Panel.SetActive(false);
        HowToPlay_Text.text = HowToPlay_File.text;
    }

    private void Init_BtnClickEvents()
    {
        StartButton.onClick.AddListener(Deactivate_GameMenu);
        
        HowToPlayBtn.onClick.AddListener(HowToPlayBtnClick);
        HowToPlay_CancelBtn.onClick.AddListener(HowToPlayCancelBtnClick);
        
        QuitBtn.onClick.AddListener(QuitGameBtnClick);

        TinkleTigersButton.onClick.AddListener(TinkleTigersBtnClick);
    }

    private void EnableTinkleTigersBtn()
    {
        TinkleTigersButton.gameObject.SetActive(true);
    }

    private void DisableTinkleTigersBtn()
    {
        TinkleTigersButton.gameObject.SetActive(false);
    }

    private void StartBtnClick()
    {
        Deactivate_GameMenu();
        
    }

    private void QuitGameBtnClick()
    {
        Debug.Log("Game was quit.");
        Application.Quit();
    }

    private void TinkleTigersBtnClick()
    {
        Activate_GameMenu();
        DisableTinkleTigersBtn();
    }

    // To be used in built/ release version. Since back button touch not registered in Unity Remote.
    // Perhaps instead of Tinkle Tigers (Logo) Button.
    private void CheckForBackBtnClick()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Activate_GameMenu();
                return;
            }
        }
    }

    private void HowToPlayBtnClick()
    {
        HowToPlay_Panel.SetActive(true);
    }

    private void HowToPlayCancelBtnClick()
    {
        HowToPlay_Panel.SetActive(false);
    }
}
