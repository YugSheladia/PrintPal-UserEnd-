using UnityEngine;
using TMPro;

public class Password : MonoBehaviour
{
    [SerializeField] private TMP_InputField PasswordInput;

    [SerializeField] private Scene2Manager Scene2ManagerScript;
    [SerializeField] private Scene2_Backend Scene2_BackendScript;

    [SerializeField] private SetProfileInfo SetProfileInfoScript;

    




    
    public void ClickedSubmitButton()
    {
        Scene2ManagerScript.EnableInvisiblePanel();

        if(CheckPasswordLength())
        {
            SetProfileInfoScript.UserInfoObject.Password = PasswordInput.text;

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
        else
        {
            Scene2ManagerScript.DisableAllPanels();

            Scene2ManagerScript.EnablePasswordPanel();
            Scene2ManagerScript.EnableMessagePanel();

            Scene2ManagerScript.MessageText.text = "Password has to contain more than 9 characters.";
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
