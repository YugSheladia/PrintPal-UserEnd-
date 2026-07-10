using UnityEngine;
using TMPro;

public class Password : MonoBehaviour
{
    [SerializeField] private TMP_InputField PasswordInput;

    [SerializeField] private Scene2Manager Scene2ManagerScript;
    [SerializeField] private Scene2_Backend Scene2_BackendScript;
    
    public void ClickedSubmitButton()
    {
        Scene2_BackendScript.UserInfoObject.Password = PasswordInput.text;

        Scene2ManagerScript.EnableInvisiblePanel();

        Scene2_BackendScript.GetStationeries();
    }

    public void ClickedPreviousPageButton()
    {
        Scene2ManagerScript.DisableAllPanels();
        Scene2ManagerScript.EnableSetPhoneNumberPanel();
    }
}
