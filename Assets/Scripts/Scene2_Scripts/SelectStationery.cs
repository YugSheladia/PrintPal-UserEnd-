using TMPro;
using UnityEngine;


public class SelectStationery : MonoBehaviour
{
    [SerializeField] Scene2Manager Scene2ManagerScript;
    [SerializeField] Scene2_Backend Scene2_BackendScript;

    [SerializeField] SetProfileInfo SetProfileInfoScript;

    [SerializeField] public TMP_Dropdown StationeryNameDropDown;


    public string StationeryName;


    





    public void ClickedSubmitButton()
    {
        if (Scene2_BackendScript.StationeryResponse == null ||
            Scene2_BackendScript.StationeryResponse.Stationeries == null)
        {
            Scene2ManagerScript.DisableAllPanels();
            Scene2ManagerScript.EnableSetStationeryNamePanel();
            Scene2ManagerScript.EnableMessagePanel();

            Scene2ManagerScript.MessageText.text = "Stationery list could'nt load.\nRestart the app and try again.";

            return;
        }

        if (StationeryNameDropDown.value < 0 ||
            StationeryNameDropDown.value >=
            Scene2_BackendScript.StationeryResponse.Stationeries.Length)
        {
            Scene2ManagerScript.DisableAllPanels();
            Scene2ManagerScript.EnableSetStationeryNamePanel();
            Scene2ManagerScript.EnableMessagePanel();

            Scene2ManagerScript.MessageText.text = "Invalid Stationery selected.\nRestart the app and try again.";

            return;
        }

        StationeryNameSummary selected =
                                        Scene2_BackendScript.StationeryResponse.Stationeries[
                                                                                                StationeryNameDropDown.value
                                                                                             ];


        // Store the selected StationeryID in the user's info.
        SetProfileInfoScript.UserInfoObject.StationeryID = selected.StationeryID;
        StationeryName = selected.StationeryName;

        Scene2ManagerScript.DisableAllPanels();
        Scene2ManagerScript.EnableSetProfileInfoPanel();
    }

    public void ClickedPreviousPage()
    {
        Scene2ManagerScript.DisableAllPanels();
        Scene2ManagerScript.EnablePasswordPanel();
    }
}