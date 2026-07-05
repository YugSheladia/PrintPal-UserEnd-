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
        print("Clicked Submit Button.");

        PhoneNumber = PhoneNumberInputField.text;

        Scene2_BackendScript.UserInfoObject.PhoneNumber = PhoneNumber;

        Scene2_BackendScript.CheckPhoneNumberAvailability();
    }

    public void ClickedPreviousPageButton()
    {
        Scene2ManagerScript.DisableAllPanels();
        Scene2ManagerScript.EnableLoginPanel();
    }
}
