using TMPro;
using UnityEngine;


public class SelectStationery : MonoBehaviour
{
    [SerializeField] Scene2Manager Scene2ManagerScript;
    [SerializeField] Scene2_Backend Scene2_BackendScript;

    [SerializeField] public TMP_Dropdown StationeryNameDropDown;


    public string StationeryName;


    public void ClickedSubmitButton()
    {
        if (Scene2_BackendScript.StationeryResponse == null ||
            Scene2_BackendScript.StationeryResponse.Stationeries == null)
        {
            Debug.LogError("Stationery list has not been loaded.");
            return;
        }

        if (StationeryNameDropDown.value < 0 ||
            StationeryNameDropDown.value >=
            Scene2_BackendScript.StationeryResponse.Stationeries.Length)
        {
            Debug.LogError("Invalid stationery selected.");
            return;
        }

        StationeryNameSummary selected =
                                        Scene2_BackendScript.StationeryResponse.Stationeries[
                                                                                                StationeryNameDropDown.value
                                                                                             ];

        Debug.Log
               (
                    $"Selected: {selected.StationeryName} (ID = {selected.StationeryID})"
               );

        // Store the selected StationeryID in the user's info.
        Scene2_BackendScript.UserInfoObject.StationeryID = selected.StationeryID;
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