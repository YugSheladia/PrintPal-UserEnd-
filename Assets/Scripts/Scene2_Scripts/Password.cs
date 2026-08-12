using UnityEngine;
using TMPro;

public class Password : MonoBehaviour
{
    [SerializeField] private TMP_InputField PasswordInput;
    [SerializeField] private TMP_InputField PasswordInput2;

    [SerializeField] private Scene2Manager Scene2ManagerScript;
    [SerializeField] private Scene2_Backend Scene2_BackendScript;

    [SerializeField] private SetProfileInfo SetProfileInfoScript;

    




    
    public void ClickedSubmitButton()
    {
        Scene2ManagerScript.EnableInvisiblePanel();

        if(CheckPasswordLength() && CheckPasswordSameness())
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
            Scene2ManagerScript.MessageText.text = "Password has to contain more than 9 characters.";

            return false;
        }
        return true;
    }

    public bool CheckPasswordSameness()
    {
        if (PasswordInput.text != PasswordInput2.text)
        {
            Scene2ManagerScript.MessageText.text = "Passwords are not matching.";

            return false;
        }

        return true;
    }
}
