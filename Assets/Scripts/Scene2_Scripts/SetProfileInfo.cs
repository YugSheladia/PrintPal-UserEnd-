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

    public void ClickedSubmitButton()
    {
        if (CheckProfileDetails())
        {
            Scene2ManagerScript.EnableInvisiblePanel();
            StartCoroutine(SaveProfile());
        }
    }

    public void ClickedPreviousPage()
    {
        Scene2ManagerScript.DisableAllPanels();
        Scene2ManagerScript.EnableSetStationeryNamePanel();

        Scene2_BackendScript.GetStationeries();
    }

    public IEnumerator SaveProfile()
    {
        UserActualName = NameField.text.Trim();
        UserUPIPhoneNumber = UPIPhoneField.text.Trim();
        UserEmail = EmailField.text.Trim();
        Username = UsernameField.text.Trim();

        Scene2_BackendScript.UserInfoObject.Name = UserActualName;
        Scene2_BackendScript.UserInfoObject.UPINumber = UserUPIPhoneNumber;
        Scene2_BackendScript.UserInfoObject.Email = UserEmail;
        Scene2_BackendScript.UserInfoObject.Username = Username;

        yield return Scene2_BackendScript.SubmitUserInfoToServer();
    }








    public bool CheckProfileDetails()
    {
        if(!CheckNameLength())
        {
            Scene2ManagerScript.DisableAllPanels();
            Scene2ManagerScript.EnableSetProfileInfoPanel();
            Scene2ManagerScript.EnableErrorPanel();

            Scene2ManagerScript.ErrorText.text = "Name field is empty.";

            return false;
        }
        if(!CheckUPINumberLength())
        {
            Scene2ManagerScript.DisableAllPanels();
            Scene2ManagerScript.EnableSetProfileInfoPanel();
            Scene2ManagerScript.EnableErrorPanel();

            Scene2ManagerScript.ErrorText.text = "UPI Number has to be of 10 digits.";

            return false;
        }
        if (!CheckEmailLength())
        {
            Scene2ManagerScript.DisableAllPanels();
            Scene2ManagerScript.EnableSetProfileInfoPanel();
            Scene2ManagerScript.EnableErrorPanel();

            Scene2ManagerScript.ErrorText.text = "Email field is empty.";

            return false;
        }
        if (!CheckUsernameLength())
        {
            Scene2ManagerScript.DisableAllPanels();
            Scene2ManagerScript.EnableSetProfileInfoPanel();
            Scene2ManagerScript.EnableErrorPanel();

            Scene2ManagerScript.ErrorText.text = "Username field is empty.";

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

