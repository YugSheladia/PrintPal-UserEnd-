using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpdateStationery : MonoBehaviour
{
    [SerializeField] Scene4Manager Scene4ManagerScript;

    [SerializeField] TMP_Dropdown StationeryNameDropDown;



    public void ClickedSubmitButton()
    {
        if (Script1.CheckStationeryExistence((SelectStationery.StationeryNameEnum)StationeryNameDropDown.value))
        {
            Scene4ManagerScript.DisableAllPanels();
            Scene4ManagerScript.EnableProfileMenuPanel();

            Debug.Log("StationeryID = " + Script1.StationeryID);
        }
        else
        {
            Debug.Log("Input valid Stationery.");
        }
    }

    public void ClickedPreviousPage()
    {
        Scene4ManagerScript.DisableAllPanels();
        Scene4ManagerScript.EnableProfileMenuPanel();
    }
}
