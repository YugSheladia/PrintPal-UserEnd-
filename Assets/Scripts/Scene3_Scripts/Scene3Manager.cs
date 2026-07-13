//Script1

using System.Collections;
using TMPro;
using UnityEngine;

public class Scene3Manager : MonoBehaviour
{
    [Header("Scene3 Manager stuff")]
    [SerializeField] GameObject BlankPanelObject;
    [SerializeField] GameObject InvisiblePanel;

    [Header("HomePage")]
    [SerializeField] GameObject HomePageObject;

    [Header("OrderManager stuff ")]
    [SerializeField] GameObject PrintManagerObject;

    [Header("OrderSettings stuff")]
    [SerializeField] GameObject OrderSettingsParent;

    [Header("PrintBillsManager stuff")]
    [SerializeField] GameObject PrintBillsManagerObject;

    [Header("OrderStatusManager stuff")]
    [SerializeField] GameObject OrderStatusManagerObject;


    [SerializeField] GameObject OrdersPanel;


    [SerializeField] GameObject UpdateStationery2Panel;


    [SerializeField] public GameObject MessagePanel;
    [SerializeField] public TMP_Text MessageText;




    private void Start()
    {
        HomePage();
    }



    public void HomePage()
    {
        DisableAllPanels();
        EnableHomePage();
    }
    public void CreateOrderPage()
    {
        DisableAllPanels();
        EnablePrintManager();
    }
    public void OrdersPage()
    {
        DisableAllPanels();
        EnableOrderStatusManager();
        EnableOrdersPanel();
    }




    public void DisableAllPanels()
    {
        Debug.Log("Called DisableAllPanels");
        DisableBlank();
        DisableInvisiblePanel();

        DisableHomePage();
        DisablePrintManager();
        DisablePrintBillsManager();
        DisableOrderStatusManager();
        DisableOrdersPanel();
        DisableStationeryPreferencePanel();
    }



    public void EnableBlank()
    {
        BlankPanelObject.SetActive(true);
    }
    public void DisableBlank()
    {
        BlankPanelObject.SetActive(false);
    }
    public void EnableInvisiblePanel()
    {
        InvisiblePanel.SetActive(true);
    }
    public void DisableInvisiblePanel()
    {
        InvisiblePanel.SetActive(false);
    }



    public void EnableHomePage()
    {
        HomePageObject.SetActive(true);
    }
    public void DisableHomePage()
    {
        HomePageObject.SetActive(false);
    }
    public void EnablePrintManager()
    {
        PrintManagerObject.SetActive(true);
    }
    public void DisablePrintManager()
    {
        PrintManagerObject.SetActive(false);
    }
    public void EnablePrintBillsManager()
    {
        PrintBillsManagerObject.SetActive(true);
    }
    public void DisablePrintBillsManager()
    {
        PrintBillsManagerObject.SetActive(false);
    }
    public void EnableOrderStatusManager()
    {
        OrderStatusManagerObject.SetActive(true);
    }
    public void DisableOrderStatusManager()
    {
        OrderStatusManagerObject.SetActive(false);
    }
    public void EnableOrdersPanel()
    {
        OrdersPanel.SetActive(true);
    }
    public void DisableOrdersPanel()
    {
        OrdersPanel.SetActive(false);
    }


    public void EnableStationeryPreferencePanel()
    {
        UpdateStationery2Panel.SetActive(true);
    }
    public void DisableStationeryPreferencePanel()
    {
        UpdateStationery2Panel.SetActive(false);
    }



    public void EnableMessagePanel()
    {
        MessagePanel.SetActive(true);
        StartCoroutine(CoroutineEnableMessagePanel());
    }
    public void DisableMessagePanel()
    {
        MessagePanel.SetActive(false);
    }
    IEnumerator CoroutineEnableMessagePanel()
    {
        yield return new WaitForSeconds(5);

        DisableMessagePanel();
    }
}
