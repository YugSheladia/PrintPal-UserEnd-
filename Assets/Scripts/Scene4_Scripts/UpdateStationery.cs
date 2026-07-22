using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpdateStationery : MonoBehaviour
{
    [SerializeField] Scene4Manager Scene4ManagerScript;
    [SerializeField] Scene4_Backend Scene4_BackendScript;

    [SerializeField] public TMP_Dropdown StationeryNameDropDown;



    public string StationeryName;



    public void ClickedSubmitButton()
    {
        Scene4ManagerScript.EnableInvisiblePanel();

        if (Scene4_BackendScript.StationeryResponse == null ||
            Scene4_BackendScript.StationeryResponse.Stationeries == null)
        {
            Debug.LogError("Stationery list has not been loaded.");

            Scene4ManagerScript.MessageText.text = "\"Stationery list has not been loaded. Restart the app and try again.";
            Scene4ManagerScript.EnableMessagePanel();

            Scene4ManagerScript.DisableAllPanels();
            Scene4ManagerScript.EnableProfileMenuPanel();

            return;
        }

        if (StationeryNameDropDown.value < 0 ||
            StationeryNameDropDown.value >=
            Scene4_BackendScript.StationeryResponse.Stationeries.Length)
        {
            Debug.LogError("Invalid stationery selected.");

            Scene4ManagerScript.MessageText.text = "Invalid stationery selected. Restart the app if you think this is a mistake.";
            Scene4ManagerScript.EnableMessagePanel();

            Scene4ManagerScript.DisableAllPanels();
            Scene4ManagerScript.EnableUpdateStationeryPanel();

            return;
        }

        StationeryNameSummary selected =
            Scene4_BackendScript.StationeryResponse.Stationeries[
                                                                    StationeryNameDropDown.value
                                                                ];

        Debug.Log
               (
                    $"Selected: {selected.StationeryName} (ID = {selected.StationeryID})"
               );

        StationeryName = selected.StationeryName;

        Scene4_BackendScript.UpdateStationeryPreference
        (
            selected.StationeryID,
            Success =>
            {
                if(Success)
                {
                    Scene4ManagerScript.DisableAllPanels();

                    Scene4ManagerScript.EnableMessagePanel();
                    Scene4ManagerScript.EnableProfileMenuPanel();
                }
                else
                {
                    Scene4ManagerScript.DisableAllPanels();

                    Scene4ManagerScript.EnableMessagePanel();
                    Scene4ManagerScript.EnableUpdateStationeryPanel();
                }
            }
        );
    }

    public void ClickedPreviousPage()
    {
        Scene4ManagerScript.DisableAllPanels();
        Scene4ManagerScript.EnableProfileMenuPanel();
    }
}
