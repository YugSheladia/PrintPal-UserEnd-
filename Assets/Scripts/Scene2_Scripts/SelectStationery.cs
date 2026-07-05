using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectStationery : MonoBehaviour
{
    [SerializeField] Scene2Manager Scene2ManagerScript;


    [SerializeField] TMP_Dropdown StationeryNameDropDown;

    public enum StationeryNameEnum
    {
        SFIT_Borivali = 0
    }



    public void ClickedSubmitButton()
    {
        if (Script1.CheckStationeryExistence((StationeryNameEnum)StationeryNameDropDown.value))
        {
            Scene2ManagerScript.DisableAllPanels();
            Scene2ManagerScript.EnableSetProfileInfoPanel();
        }
        else
        {
            Debug.Log("Input valid Stationery.");
        }
    }



    public void ClickedPreviousPage()
    {
        Scene2ManagerScript.DisableAllPanels();
        Scene2ManagerScript.EnablePasswordPanel();
    }
}
