using UnityEngine;
using UnityEngine.SceneManagement;

public class ProfileMenu : MonoBehaviour
{
    [SerializeField] Scene4Manager Scene4ManagerScript;
    [SerializeField] Scene4_Backend Scene4_BackendScript;

    [SerializeField] ViewProfileInfo ViewProfileInfoScript;
    


    public void ClickedViewProfileInfo()
    {
        Scene4ManagerScript.EnableInvisiblePanel();

        ViewProfileInfoScript.UpdateProfileInfo();
    }


    public void ClickedStationery()
    {
        Scene4ManagerScript.EnableInvisiblePanel();

        Scene4_BackendScript.GetStationeries();
    }


    public void ClickedLogout()
    {
        Scene4ManagerScript.EnableInvisiblePanel();

        Script1.SetUserInfo(null);

        SceneManager.LoadScene("Scene1_Loading+Landing");
    }


    public void ClickedPreviousPage()
    {
        Scene4ManagerScript.EnableInvisiblePanel();

        SceneManager.LoadScene("Scene3_Home");
    }

}
