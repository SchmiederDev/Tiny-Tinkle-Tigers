using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject Menu;

    [SerializeField]
    private Button StartButton;

    [SerializeField]
    private Button ControlBtn;

    [SerializeField]
    private Button QuitBtn;


    [SerializeField]
    private Button LeakyButton;

    private void Awake()
    {
        Activate_GameMenu();
        StartButton.onClick.AddListener(Deactivate_GameMenu);
        QuitBtn.onClick.AddListener(QuitGameBtnClick);

        LeakyButton.onClick.AddListener(LeakyKittensBtnClick);
        //DisableLeakyBtn();
    }

    public void Activate_GameMenu()
    {
        Menu.SetActive(true);
        DisableLeakyBtn();
        Time.timeScale = 0f;
    }

    public void Deactivate_GameMenu()
    {
        Menu.SetActive(false);
        EnableLeakyBtn();
        Time.timeScale = 1f;
    }

    private void EnableLeakyBtn()
    {
        LeakyButton.gameObject.SetActive(true);
    }

    private void DisableLeakyBtn()
    {
        LeakyButton.gameObject.SetActive(false);
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

    private void LeakyKittensBtnClick()
    {
        Activate_GameMenu();
        DisableLeakyBtn();
    }

    // To be used in built/ release version. Since back button touch not registered in Unity Remote.
    // Perhaps instead of Leaky Kittens (Logo) Button.
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
}
