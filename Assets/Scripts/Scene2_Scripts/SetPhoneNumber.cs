using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetPhoneNumber : MonoBehaviour
{
    [SerializeField] private TMP_InputField PhoneNumberInputField;

    [SerializeField] private Scene2Manager  Scene2ManagerScript;
    [SerializeField] private Scene2_Backend Scene2_BackendScript;

    public string PhoneNumber;

    public void ClickedSubmitButton()
    {
        Scene2ManagerScript.EnableInvisiblePanel();

        if(CheckPhoneNumberLength())
        {
            PhoneNumber = PhoneNumberInputField.text;

            Scene2_BackendScript.UserInfoObject.PhoneNumber = PhoneNumber;

            Scene2_BackendScript.CheckPhoneNumberAvailability();
        }
        else
        {
            Scene2ManagerScript.DisableAllPanels();
            Scene2ManagerScript.EnableSetPhoneNumberPanel();
            Scene2ManagerScript.EnableErrorPanel();

            Scene2ManagerScript.ErrorText.text = "Phone number has to be of 10 digits.";
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
