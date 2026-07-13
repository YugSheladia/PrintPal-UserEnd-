using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ViewProfileInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text UserActualName;
    [SerializeField] private TMP_Text UserName;
    [SerializeField] private TMP_Text MobileNumber;
    [SerializeField] private TMP_Text UPILinkedMobileNumber;
    [SerializeField] private TMP_Text Email;

    [SerializeField] Scene4Manager Scene4ManagerScript;
    [SerializeField] Scene4_Backend Scene4_BackendScript;

    [SerializeField] UpdateProfileInfo UpdateProfileInfoScript;

    public void UpdateProfileInfo()
    {
        Scene4_BackendScript.StartCoroutine_GetUserInfoFromServer();
    }

    public void SetUI()
    {
        UserActualName.text         = Script1.UserActualName;
        UserName.text               = Script1.UserName;
        MobileNumber.text           = Script1.PhoneNumber;
        UPILinkedMobileNumber.text  = Script1.UPILinkedPhoneNumber;
        Email.text                  = Script1.UserEmail;
    }

    public void ClickedPreviousPageButton()
    {
        Scene4ManagerScript.DisableAllPanels();
        Scene4ManagerScript.EnableProfileMenuPanel();
    }

    public void ClickedEditButton()
    {
        Scene4ManagerScript.EnableInvisiblePanel();

        UpdateProfileInfoScript.IntializeUI();
    }
}
