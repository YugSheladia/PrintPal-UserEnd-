using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetProfileInfo : MonoBehaviour
{
    [Header("Input Fields")]
    [SerializeField] private TMP_InputField NameField;
    [SerializeField] private TMP_InputField UPIPhoneField;
    [SerializeField] private TMP_InputField EmailField;
    [SerializeField] private TMP_InputField UsernameField;

    private string UserActualName;
    private string UserUPIPhoneNumber;
    private string UserEmail;
    private string Username;

    [SerializeField] private Scene2Manager  Scene2ManagerScript;
    [SerializeField] private Scene2_Backend Scene2_BackendScript;

    public void ClickedSubmitButton()
    {
        StartCoroutine(SaveProfile());

        SceneManager.LoadScene("Scene3_Home");
    }

    public void ClickedPreviousPage()
    {
        Scene2ManagerScript.DisableAllPanels();
        Scene2ManagerScript.EnableSetStationeryNamePanel();
    }
    
    public IEnumerator SaveProfile()
    {
        UserActualName      = NameField.text.Trim();
        UserUPIPhoneNumber  = UPIPhoneField.text.Trim();
        UserEmail           = EmailField.text.Trim();
        Username            = UsernameField.text.Trim();

        Scene2_BackendScript.UserInfoObject.StationeryID = Script1.StationeryID;

        Scene2_BackendScript.UserInfoObject.Name        = UserActualName;
        Scene2_BackendScript.UserInfoObject.UPINumber   = UserUPIPhoneNumber;
        Scene2_BackendScript.UserInfoObject.Email       = UserEmail;
        Scene2_BackendScript.UserInfoObject.Username    = Username;

        yield return Scene2_BackendScript.SubmitUserInfoToServer();
    }
}
