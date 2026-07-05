using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Scene4_Backend;

public class UpdateProfileInfo : MonoBehaviour
{
    [Header("Input Fields")]
    [SerializeField] private TMP_InputField NameField;
    [SerializeField] private TMP_InputField UPIPhoneField;
    [SerializeField] private TMP_InputField EmailField;
    [SerializeField] private TMP_InputField UsernameField;

    [SerializeField] private Scene4Manager  Scene4ManagerScript;
    [SerializeField] private Scene4_Backend Scene4_BackendScript;

    [SerializeField] private ViewProfileInfo ViewProfileInfoScript;



    public void IntializeUI()
    {
        Scene4_BackendScript.StartCoroutine_GetUserInfoFromServer();
    }
    public void SetUI()
    {
        NameField.text      = Script1.UserActualName;
        UPIPhoneField.text  = Script1.UPILinkedPhoneNumber;
        EmailField.text     = Script1.UserEmail;
        UsernameField.text  = Script1.UserName;
    }


    public void ClickedSubmitButton()
    {
        SaveProfile();
        ViewProfileInfoScript.UpdateProfileInfo();

        Scene4ManagerScript.DisableAllPanels();
        Scene4ManagerScript.EnableViewProfileInfoPanel();
    }

    public void ClickedPreviousPage()
    {
        Scene4ManagerScript.DisableAllPanels();
        Scene4ManagerScript.EnableViewProfileInfoPanel();
    }
    
    public void SaveProfile()
    {
        UpdatedUserInfo UpdatedUserInfoObject = new UpdatedUserInfo();

        UpdatedUserInfoObject.PhoneNumber = Script1.PhoneNumber;

        UpdatedUserInfoObject.Name      = NameField.text        .Trim();
        UpdatedUserInfoObject.UPINumber = UPIPhoneField.text    .Trim();
        UpdatedUserInfoObject.Email     = EmailField.text       .Trim();
        UpdatedUserInfoObject.Username  = UsernameField.text    .Trim();

        Scene4_BackendScript.StartCoroutine_UpdateUserInfoInServer(UpdatedUserInfoObject);
    }
}
