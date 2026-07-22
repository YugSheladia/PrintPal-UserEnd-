using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetProfileInfo : MonoBehaviour
{
    [Header("Input Fields")]
    [SerializeField] private TMP_InputField NameField;
    [SerializeField] private TMP_InputField UPIPhoneField;
    [SerializeField] private TMP_InputField EmailField;
    [SerializeField] private TMP_InputField UsernameField;

    private string UserActualName;
    private string UserUPIPhoneNumber;
    private string UserEmail;
    private string Username;

    [SerializeField] private Scene2Manager Scene2ManagerScript;
    [SerializeField] private Scene2_Backend Scene2_BackendScript;

    public UserInfo UserInfoObject;






    public void ClickedSubmitButton()
    {
        if (CheckProfileDetails())
        {
            Scene2ManagerScript.EnableInvisiblePanel();

            UserActualName = NameField.text.Trim();
            UserUPIPhoneNumber = UPIPhoneField.text.Trim();
            UserEmail = EmailField.text.Trim();
            Username = UsernameField.text.Trim();

            UserInfoObject.Name = UserActualName;
            UserInfoObject.UPINumber = UserUPIPhoneNumber;
            UserInfoObject.Email = UserEmail;
            UserInfoObject.Username = Username;

            Scene2_BackendScript.SubmitUserInfoToServer
            (
                UserInfoObject,
                Success =>
                {
                    if (Success)
                    {
                        SceneManager.LoadScene("Scene3_Home");
                    }
                    else
                    {
                        Scene2ManagerScript.DisableAllPanels();

                        Scene2ManagerScript.EnableMessagePanel();
                        Scene2ManagerScript.EnableSetProfileInfoPanel();
                    }
                }
            );
        }
    }

    public void ClickedPreviousPage()
    {
        Scene2ManagerScript.DisableAllPanels();
        Scene2ManagerScript.EnableSetStationeryNamePanel();

        Scene2_BackendScript.GetStationeries
        (
            Success =>
            {
                if (Success)
                {
                    Scene2ManagerScript.DisableAllPanels();

                    Scene2ManagerScript.EnableSetStationeryNamePanel();
                }
                else
                {
                    Scene2ManagerScript.DisableAllPanels();

                    Scene2ManagerScript.EnableMessagePanel();
                    Scene2ManagerScript.EnablePasswordPanel();
                }
            }
        );
    }









    public bool CheckProfileDetails()
    {
        if(!CheckNameLength())
        {
            Scene2ManagerScript.DisableAllPanels();
            Scene2ManagerScript.EnableSetProfileInfoPanel();
            Scene2ManagerScript.EnableMessagePanel();

            Scene2ManagerScript.MessageText.text = "Name field is empty.";

            return false;
        }
        if(!CheckUPINumberLength())
        {
            Scene2ManagerScript.DisableAllPanels();
            Scene2ManagerScript.EnableSetProfileInfoPanel();
            Scene2ManagerScript.EnableMessagePanel();

            Scene2ManagerScript.MessageText.text = "UPI Number has to be of 10 digits.";

            return false;
        }
        if (!CheckEmailLength())
        {
            Scene2ManagerScript.DisableAllPanels();
            Scene2ManagerScript.EnableSetProfileInfoPanel();
            Scene2ManagerScript.EnableMessagePanel();

            Scene2ManagerScript.MessageText.text = "Email field is empty.";

            return false;
        }
        if (!CheckUsernameLength())
        {
            Scene2ManagerScript.DisableAllPanels();
            Scene2ManagerScript.EnableSetProfileInfoPanel();
            Scene2ManagerScript.EnableMessagePanel();

            Scene2ManagerScript.MessageText.text = "Username field is empty.";

            return false;
        }
        else
        {
            return true;
        }
    }


    public bool CheckNameLength()
    {
        if (NameField.text.Length <= 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public bool CheckUPINumberLength()
    {
        if (UPIPhoneField.text.Length != 10)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public bool CheckEmailLength()
    {
        if (EmailField.text.Length <= 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public bool CheckUsernameLength()
    {
        if (UsernameField.text.Length <= 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}

