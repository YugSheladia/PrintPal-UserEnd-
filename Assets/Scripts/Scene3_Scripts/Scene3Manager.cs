//Script1

using UnityEngine;

public class Scene3Manager : MonoBehaviour
{
    [Header("Scene3 Manager stuff")]
    [SerializeField] GameObject BlankPanelObject;

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


    [SerializeField] GameObject RecentOrdersPanel;


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
    public void RecentOrdersPage()
    {
        DisableAllPanels();
        EnableOrderStatusManager();
        EnableRecentOrdersPanel();
    }




    public void DisableAllPanels()
    {
        DisableHomePage();
        DisablePrintManager();
        DisablePrintBillsManager();
        DisableOrderStatusManager();
        DisableRecentOrdersPanel();
    }



    public void EnableBlank()
    {
        BlankPanelObject.SetActive(true);
    }
    public void DisableBlank()
    {
        BlankPanelObject.SetActive(false);
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
    public void EnableRecentOrdersPanel()
    {
        RecentOrdersPanel.SetActive(true);
    }
    public void DisableRecentOrdersPanel()
    {
        RecentOrdersPanel.SetActive(false);
    }
}
