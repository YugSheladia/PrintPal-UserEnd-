using TMPro;
using UnityEngine;
using System.Collections;

public class Scene4Manager : MonoBehaviour
{
    [SerializeField] private GameObject BlankPanel;

    [SerializeField] private GameObject ProfileMenuPanel;
    [SerializeField] private GameObject ViewYourProfileInfoPanel;
    [SerializeField] private GameObject UpdateStationeryPanel;
    [SerializeField] private GameObject UpdateProfileInfoPanel;

    [SerializeField] public GameObject InvisiblePanel;

    [SerializeField] public GameObject  MessagePanel;
    [SerializeField] public TMP_Text    MessageText;











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

        DisableInvisiblePanel();
    }









    public void EnableBlankPanel()
    {
        BlankPanel.SetActive(true);
    }
    public void DisableBlankPanel()
    {
        BlankPanel?.SetActive(false);
    }
    public void EnableInvisiblePanel()
    {
        InvisiblePanel.SetActive(true);
    }
    public void DisableInvisiblePanel()
    {
        InvisiblePanel.SetActive(false);
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




    public void EnableMessagePanel()
    {
        StartCoroutine(CoroutineEnableMessagePanel());
    }
    public void DisableMessagePanel()
    {
        MessagePanel.SetActive(false);
    }
    IEnumerator CoroutineEnableMessagePanel()
    {
        MessagePanel.SetActive(true);
        yield return new WaitForSeconds(5);

        DisableMessagePanel();
    }
}
