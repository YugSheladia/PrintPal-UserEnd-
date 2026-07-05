using UnityEngine;
using UnityEngine.SceneManagement;

public class ProfileMenu : MonoBehaviour
{
    [SerializeField] Scene4Manager Scene4ManagerScript;
    [SerializeField] ViewProfileInfo ViewProfileInfoScript;
    


    public void ClickedViewProfileInfo()
    {
        ViewProfileInfoScript.UpdateProfileInfo();

        Scene4ManagerScript.DisableAllPanels();
        Scene4ManagerScript.EnableViewProfileInfoPanel();
    }


    public void ClickedStationery()
    {
        Scene4ManagerScript.DisableAllPanels();
        Scene4ManagerScript.EnableUpdateStationeryPanel();
    }


    public void ClickedLogout()
    {
        Script1.SetUserInfo(null);

        SceneManager.LoadScene("Scene1_Loading+Landing");
    }


    public void ClickedPreviousPage()
    {
        SceneManager.LoadScene("Scene3_Home");
    }

}
