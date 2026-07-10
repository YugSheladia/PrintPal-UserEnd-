using UnityEngine;

public class Scene2Manager : MonoBehaviour
{
    [SerializeField] private GameObject BlankPanel;

    [SerializeField] private GameObject SetPhoneNumberPanel;
    [SerializeField] private GameObject SetProfileInfoPanel;
    [SerializeField] private GameObject LoginPanel;
    [SerializeField] private GameObject PasswordPanel;
    [SerializeField] private GameObject SetStationeryNamePanel;
    [SerializeField] private GameObject InvisiblePanel;

    [SerializeField] private Scene2_Backend Scene2_BackendScript;


    private void Start()
    {
        //EnableSetPhoneNumberPanel();

        EnableLoginPanel();
        DisableBlankPanel();
    }

    public void DisableAllPanels()
    {
        DisableBlankPanel();
        DisableInvisiblePanel();

        DisableSetPhoneNumberPanel();
        DisableLoginPanel();
        DisablePasswordPanel();
        DisableSetProfileInfoPanel();
        DisableSetStationeryNamePanel();
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
        InvisiblePanel?.SetActive(false);
    }



    public void EnableSetPhoneNumberPanel()
    {
        SetPhoneNumberPanel.SetActive(true);
    }
    public void DisableSetPhoneNumberPanel()
    {
        SetPhoneNumberPanel.SetActive(false);
    }
    public void EnableLoginPanel()
    {
        LoginPanel.SetActive(true);
    }
    public void DisableLoginPanel()
    {
        LoginPanel?.SetActive(false);
    }
    public void EnablePasswordPanel()
    {
        PasswordPanel.SetActive(true);
    }
    public void DisablePasswordPanel()
    {
        PasswordPanel?.SetActive(false);
    }
    public void EnableSetProfileInfoPanel()
    {
        SetProfileInfoPanel.SetActive(true);
    }
    public void DisableSetProfileInfoPanel()
    {
        SetProfileInfoPanel?.SetActive(false);
    }
    public void EnableSetStationeryNamePanel()
    {
        SetStationeryNamePanel.SetActive(true);
    }
    public void DisableSetStationeryNamePanel()
    {
        SetStationeryNamePanel?.SetActive(false);
    }    
}

