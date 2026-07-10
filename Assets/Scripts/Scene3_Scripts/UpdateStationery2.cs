using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpdateStationery2 : MonoBehaviour
{
    [SerializeField] Scene3Manager Scene3ManagerScript;
    [SerializeField] Scene3_Backend Scene3_BackendScript;

    [SerializeField] public TMP_Dropdown StationeryNameDropDown;



    public string StationeryName;



    public void ClickedSubmitButton()
    {
        if (Scene3_BackendScript.StationeryResponse == null ||
            Scene3_BackendScript.StationeryResponse.Stationeries == null)
        {
            Debug.LogError("Stationery list has not been loaded.");
            return;
        }

        if (StationeryNameDropDown.value < 0 ||
            StationeryNameDropDown.value >=
            Scene3_BackendScript.StationeryResponse.Stationeries.Length)
        {
            Debug.LogError("Invalid stationery selected.");
            return;
        }

        StationeryNameSummary selected =
            Scene3_BackendScript.StationeryResponse.Stationeries[
                                                                    StationeryNameDropDown.value
                                                                ];

        Debug.Log
               (
                    $"Selected: {selected.StationeryName} (ID = {selected.StationeryID})"
               );

        StationeryName = selected.StationeryName;

        Scene3ManagerScript.EnableInvisiblePanel();
        StartCoroutine(Scene3_BackendScript.UpdateStationeryPreference(selected.StationeryID));
    }

    public void ClickedPreviousPage()
    {
        Scene3ManagerScript.DisableAllPanels();
        Scene3ManagerScript.EnablePrintBillsManager();
    }
}
