using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene2Manager : MonoBehaviour
{
    [SerializeField] private GameObject BlankPanel;

    //[SerializeField] private GameObject LandingPagePanel;

    [SerializeField] private GameObject SetPhoneNumberPanel;
    [SerializeField] private GameObject SetProfileInfoPanel;
    [SerializeField] private GameObject PasswordPanel;
    [SerializeField] private GameObject SetStationeryNamePanel;

    [SerializeField] private GameObject LoginPanel;
    
    [SerializeField] private GameObject InvisiblePanel;

    [SerializeField] private GameObject MessagePanel;
    [SerializeField] public TMP_Text MessageText;

    [SerializeField] private Scene2_Backend Scene2_BackendScript;


    private void Start()
    {
        EnableLoginPanel();
        DisableBlankPanel();
    }





    public void DisableAllPanels()
    {
        DisableBlankPanel();
        DisableInvisiblePanel();

        //DisableLandingPage();

        DisableSetPhoneNumberPanel();
        DisablePasswordPanel();
        DisableSetProfileInfoPanel();
        DisableSetStationeryNamePanel();

        DisableLoginPanel();
        

        DisableMessagePanel();
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




    public void EnableLoginPanel()
    {
        LoginPanel.SetActive(true);
    }
    public void DisableLoginPanel()
    {
        LoginPanel?.SetActive(false);
    }



    /*
    public void EnableLandingPage()
    {
        LandingPagePanel.SetActive(true);
    }
    public void DisableLandingPage()
    {
        LandingPagePanel.SetActive(false);
    }
    */
    public void ClickedPreviousPageButtonInLandingPage()
    {
        DisableAllPanels();
        EnableLoginPanel();
    }
    public void ClickedGetStartedButton()
    {
        DisableAllPanels();
        EnableSetPhoneNumberPanel();
    }





    public void EnableSetPhoneNumberPanel()
    {
        SetPhoneNumberPanel.SetActive(true);
    }
    public void DisableSetPhoneNumberPanel()
    {
        SetPhoneNumberPanel.SetActive(false);
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








    private Coroutine MessagePanelCoroutine;
    public void EnableMessagePanel()
    {
        if (MessagePanelCoroutine != null)
        {
            StopCoroutine(MessagePanelCoroutine);
        }

        MessagePanel.SetActive(true);

        MessagePanelCoroutine = StartCoroutine(CoroutineMessagePanelHandler());
    }
    public void DisableMessagePanel()
    {
        if (MessagePanelCoroutine != null)
        {
            StopCoroutine(MessagePanelCoroutine);
            MessagePanelCoroutine = null;
        }

        MessagePanel.SetActive(false);
    }
    IEnumerator CoroutineMessagePanelHandler()
    {
        yield return new WaitForSeconds(5);

        MessagePanel.SetActive(false);
        MessagePanelCoroutine = null;
    }
}

