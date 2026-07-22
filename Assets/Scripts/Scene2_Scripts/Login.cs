using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices.WindowsRuntime;

public class LoginInfo
{
    public string PhoneNumber;
    public string Password;

    public LoginInfo() { }

    public LoginInfo(string PhoneNumber, string Password)
    {
        this.PhoneNumber = PhoneNumber;
        this.Password = Password;
    }
}

public class Login : MonoBehaviour
{
    [SerializeField] TMP_InputField PhoneNumberInput;
    [SerializeField] TMP_InputField PasswordInput;

    string PhoneNumber;
    string Password;

    [SerializeField] Scene2Manager Scene2ManagerScript;
    [SerializeField] Scene2_Backend Scene2_BackendScript;






    public void ClickedSubmitButton()
    {
        Scene2ManagerScript.EnableInvisiblePanel();

        if(CheckLoginInfo())
        {
            Debug.Log("Appropriately filled all fields.");

            Scene2ManagerScript.EnableInvisiblePanel();

            PhoneNumber = PhoneNumberInput.text;
            Password = PasswordInput.text;

            LoginInfo LoginInfoObject = new LoginInfo(PhoneNumber, Password);

            Scene2_BackendScript.VerifyLoginInfo
            (
                LoginInfoObject,
                Success =>
                {
                    if (Success)
                    {
                        LoadHomePage();
                    }
                    else
                    {
                        Scene2ManagerScript.DisableAllPanels();

                        Scene2ManagerScript.EnableLoginPanel();
                        Scene2ManagerScript.EnableMessagePanel();
                    }
                }
            );
        }
        else
        {
            Debug.Log("Some field is wrongly filled.");

            Scene2ManagerScript.DisableAllPanels();

            Scene2ManagerScript.EnableMessagePanel();
            Scene2ManagerScript.EnableLoginPanel();
        }
        
    }
    public void ClickedCreateAccountButton()
    {
        Scene2ManagerScript.DisableAllPanels();
        Scene2ManagerScript.EnableSetPhoneNumberPanel();
    }




    public void LoadHomePage()
    {
        SceneManager.LoadScene("Scene3_Home");
    }




    public bool CheckLoginInfo()
    {
        if (!CheckPhoneNumberLength())
        {
            return false;
        }
        if (!CheckPasswordLength())
        {
            return false;
        }

        return true;
    }
    public bool CheckPhoneNumberLength()
    {
        if (PhoneNumberInput.text.Length <= 0)
        {
            Scene2ManagerScript.MessageText.text = "Phone number field can't be empty.";

            return false;

        }
        if (PhoneNumberInput.text.Length != 10)
        {
            Scene2ManagerScript.MessageText.text = "Phone number has to be of 10 digits.";

            return false;

        }

        return true;
    }
    public bool CheckPasswordLength()
    {
        if (PasswordInput.text.Length <= 0)
        {
            Scene2ManagerScript.MessageText.text = "Password field can't be empty.";

            return false;
        }

        return true;
    }
}
