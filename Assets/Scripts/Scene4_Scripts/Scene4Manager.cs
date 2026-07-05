using UnityEngine;

public class Scene4Manager : MonoBehaviour
{
    [SerializeField] private GameObject BlankPanel;

    [SerializeField] private GameObject ProfileMenuPanel;
    [SerializeField] private GameObject ViewYourProfileInfoPanel;
    [SerializeField] private GameObject UpdateStationeryPanel;
    [SerializeField] private GameObject UpdateProfileInfoPanel;

    private void Start()
    {
        DisableAllPanels();
        EnableProfileMenuPanel();
    }


    public void DisableAllPanels()
    {
        DisableBlankPanel();
        DisableProfileMenuPanel();
        DisableViewProfileInfoPanel();
        DisableUpdateStationeryPanel();
        DisableUpdateProfilePanel();
    }

    public void EnableBlankPanel()
    {
        BlankPanel.SetActive(true);
    }
    public void DisableBlankPanel()
    {
        BlankPanel?.SetActive(false);
    }
    public void EnableProfileMenuPanel()
    {
        ProfileMenuPanel.SetActive(true);
    }
    public void DisableProfileMenuPanel()
    {
        ProfileMenuPanel.SetActive(false);
    }
    public void EnableViewProfileInfoPanel()
    {
        ViewYourProfileInfoPanel.SetActive(true);
    }
    public void DisableViewProfileInfoPanel()
    {
        ViewYourProfileInfoPanel.SetActive(false);
    }
    public void EnableUpdateStationeryPanel()
    {
        UpdateStationeryPanel.SetActive(true);
    }
    public void DisableUpdateStationeryPanel()
    {
        UpdateStationeryPanel.SetActive(false);
    }
    public void EnableUpdateProfilePanel()
    {
        UpdateProfileInfoPanel.SetActive(true);
    }
    public void DisableUpdateProfilePanel()
    {
        UpdateProfileInfoPanel.SetActive(false);
    }
    
}
