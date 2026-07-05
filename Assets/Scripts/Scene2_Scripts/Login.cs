using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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
        print("Clicked Submit Button.");

        PhoneNumber = PhoneNumberInput.text;
        Password = PasswordInput.text;

        LoginInfo LoginInfoObject = new LoginInfo(PhoneNumber, Password);

        Scene2_BackendScript.VerifyLoginInfo(LoginInfoObject);
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
}
