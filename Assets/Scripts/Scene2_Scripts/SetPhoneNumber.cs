using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetPhoneNumber : MonoBehaviour
{
    [SerializeField] private TMP_InputField PhoneNumberInputField;

    [SerializeField] private Scene2Manager  Scene2ManagerScript;
    [SerializeField] private Scene2_Backend Scene2_BackendScript;

    [SerializeField] private SetProfileInfo SetProfileInfoScript;

    public string PhoneNumber;
    UserInfo UserInfoObject;





    public void ClickedSubmitButton()
    {
        Scene2ManagerScript.EnableInvisiblePanel();

        UserInfoObject = new UserInfo();

        if (CheckPhoneNumberLength())
        {
            PhoneNumber = PhoneNumberInputField.text;

            UserInfoObject.PhoneNumber = PhoneNumber;

            Scene2_BackendScript.CheckPhoneNumberAvailability
            (
                UserInfoObject,
                Success =>
                {
                    if (Success)
                    {
                        SetProfileInfoScript.UserInfoObject.PhoneNumber = PhoneNumber;

                        Scene2ManagerScript.DisableAllPanels();
                        Scene2ManagerScript.EnablePasswordPanel();
                    }
                    else
                    {
                        Scene2ManagerScript.DisableAllPanels();

                        Scene2ManagerScript.EnableSetPhoneNumberPanel();
                        Scene2ManagerScript.EnableMessagePanel();
                    }
                }
            );
        }
        else
        {
            Scene2ManagerScript.DisableAllPanels();

            Scene2ManagerScript.EnableSetPhoneNumberPanel();
            Scene2ManagerScript.EnableMessagePanel();

            Scene2ManagerScript.MessageText.text = "Phone number has to be of 10 digits.";
        }
    }
    public void ClickedPreviousPageButton()
    {
        Scene2ManagerScript.DisableAllPanels();
        Scene2ManagerScript.EnableLoginPanel();
    }





    public bool CheckPhoneNumberLength()
    {
        if(PhoneNumberInputField.text.Length != 10)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
