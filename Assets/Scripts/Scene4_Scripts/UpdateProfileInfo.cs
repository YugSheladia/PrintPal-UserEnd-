using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Scene4_Backend;

public class UpdateProfileInfo : MonoBehaviour
{
    [Header("Input Fields")]
    [SerializeField] private TMP_InputField NameField;
    [SerializeField] private TMP_InputField UPIPhoneField;
    [SerializeField] private TMP_InputField EmailField;
    [SerializeField] private TMP_InputField UsernameField;

    [SerializeField] private Scene4Manager  Scene4ManagerScript;
    [SerializeField] private Scene4_Backend Scene4_BackendScript;

    [SerializeField] private ViewProfileInfo ViewProfileInfoScript;



    public void IntializeUI()
    {
        Scene4_BackendScript.GetUserInfoFromServer
        (
            Success =>
            {
                if (Success)
                {
                    SetUI();
                    ViewProfileInfoScript.SetUI();

                    Scene4ManagerScript.DisableAllPanels();
                    Scene4ManagerScript.EnableUpdateProfilePanel();
                }
                else
                {
                    Scene4ManagerScript.DisableAllPanels();

                    Scene4ManagerScript.EnableMessagePanel();
                    Scene4ManagerScript.EnableViewProfileInfoPanel();
                }
            }
        );
    }
    public void SetUI()
    {
        NameField.text      = Script1.UserActualName;
        UPIPhoneField.text  = Script1.UPILinkedPhoneNumber;
        EmailField.text     = Script1.UserEmail;
        UsernameField.text  = Script1.UserName;
    }






    public void ClickedSubmitButton()
    {
        Scene4ManagerScript.EnableInvisiblePanel();

        if(CheckUpdatedProfileInfo())
        {
            SaveProfile();
        }
        else
        {
            Scene4ManagerScript.EnableMessagePanel();
        }
    }

    public void ClickedPreviousPage()
    {
        Scene4ManagerScript.DisableAllPanels();
        Scene4ManagerScript.EnableViewProfileInfoPanel();
    }
    
    public void SaveProfile()
    {
        UpdatedUserInfo UpdatedUserInfoObject = new UpdatedUserInfo();

        UpdatedUserInfoObject.PhoneNumber = Script1.PhoneNumber;

        UpdatedUserInfoObject.Name = NameField.text.Trim();
        UpdatedUserInfoObject.UPINumber = UPIPhoneField.text.Trim();
        UpdatedUserInfoObject.Email = EmailField.text.Trim();
        UpdatedUserInfoObject.Username = UsernameField.text.Trim();

        Scene4_BackendScript.UpdateUserInfoInServer
        (
            UpdatedUserInfoObject,
            Success =>
            {
                if(Success)
                {
                    Script1.UserActualName          = UpdatedUserInfoObject.Name;
                    Script1.UPILinkedPhoneNumber    = UpdatedUserInfoObject.UPINumber;
                    Script1.UserEmail               = UpdatedUserInfoObject.Email;
                    Script1.UserName                = UpdatedUserInfoObject.Username;

                    ViewProfileInfoScript.UpdateProfileInfo();

                    Scene4ManagerScript.MessageText.text = "User information updated.";

                    Scene4ManagerScript.DisableAllPanels();

                    Scene4ManagerScript.EnableMessagePanel();
                    Scene4ManagerScript.EnableViewProfileInfoPanel();
                }
                else
                {
                    Scene4ManagerScript.MessageText.text = "User not found.\nRestart the app and try again.";

                    Scene4ManagerScript.DisableAllPanels();

                    Scene4ManagerScript.EnableMessagePanel();
                    Scene4ManagerScript.EnableUpdateProfilePanel();
                }
            }
        );
    }







    public bool CheckUpdatedProfileInfo()
    {
        if(!CheckNameField())
        {
            return false;
        }

        if(!CheckUPIField())   
        {
            return false;
        }

        if(!CheckEmailField())
        {
            return false;
        }

        if(!CheckUsernameField())
        {
            return false;
        }

        return true;
    }
    public bool CheckNameField()
    {
        if(NameField.text.Length == 0)
        {
            Scene4ManagerScript.MessageText.text = "Name field cannot be empty.";

            return false;
        }

        return true;
    }
    public bool CheckUPIField()
    {
        if (UPIPhoneField.text.Length == 0)
        {
            Scene4ManagerScript.MessageText.text = "UPI number field cannot be empty.";

            return false;
        }
        if (UPIPhoneField.text.Length != 10)
        {
            Scene4ManagerScript.MessageText.text = "UPI number has to be of 10 digits.";

            return false;
        }

        return true;
    }
    public bool CheckEmailField()
    {
        if (EmailField.text.Length == 0)
        {
            Scene4ManagerScript.MessageText.text = "Email field cannot be empty.";

            return false;
        }

        return true;
    }
    public bool CheckUsernameField()
    {
        if (UsernameField.text.Length == 0)
        {
            Scene4ManagerScript.MessageText.text = "Username field cannot be empty.";

            return false;
        }

        return true;
    }
}
