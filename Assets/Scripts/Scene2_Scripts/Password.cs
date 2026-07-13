using UnityEngine;
using TMPro;

public class Password : MonoBehaviour
{
    [SerializeField] private TMP_InputField PasswordInput;

    [SerializeField] private Scene2Manager Scene2ManagerScript;
    [SerializeField] private Scene2_Backend Scene2_BackendScript;

    
    
    public void ClickedSubmitButton()
    {
        Scene2ManagerScript.EnableInvisiblePanel();

        if(CheckPasswordLength())
        {
            Scene2_BackendScript.UserInfoObject.Password = PasswordInput.text;

            Scene2_BackendScript.GetStationeries();
        }
        else
        {
            Scene2ManagerScript.DisableAllPanels();
            Scene2ManagerScript.EnablePasswordPanel();
            Scene2ManagerScript.EnableErrorPanel();

            Scene2ManagerScript.ErrorText.text = "Password has to contain more than 9 characters.";
        }
    }

    public void ClickedPreviousPageButton()
    {
        Scene2ManagerScript.DisableAllPanels();
        Scene2ManagerScript.EnableSetPhoneNumberPanel();
    }



    public bool CheckPasswordLength()
    {
        if (PasswordInput.text.Length <= 9)
        {
            return false;
        }
        return true;
    }
}
