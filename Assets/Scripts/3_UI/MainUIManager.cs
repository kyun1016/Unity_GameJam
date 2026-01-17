using UnityEngine;

public class MainUIManager : SingletonBase<MainUIManager>
{
    [Header("Main UI Panels")]
    [SerializeField] private GameObject MainMenuPanel;
    [SerializeField] private GameObject SettingsPanel;
    [SerializeField] private GameObject CreditsPanel;
    [SerializeField] private GameObject QuitPanel;

    public void BTN_Start()
    {
        Debug.Log("Start Button Clicked");
        // UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
    public void BTN_Setting()
    {
        Debug.Log("Setting Button Clicked");
    }
    public void BTN_Credit()
    {
        Debug.Log("Credit Button Clicked");
    }
    public void BTN_Exit()
    {
        Debug.Log("Exit Button Clicked");
        // Application.Quit();
    }
    public void BTN_ConfirmExit()
    {
        Debug.Log("Confirm Exit Button Clicked");
        Application.Quit();
    }
}
