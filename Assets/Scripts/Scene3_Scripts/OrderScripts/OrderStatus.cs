//Script10

using TMPro;
using UnityEngine;

public class OrderStatus : MonoBehaviour
{
    Scene3Manager       Scene3ManagerScript;
    PrintBillsManager   PrintBillsManagerScript;
    OrderStatusManager  OrderStatusManagerScript;


    [Header("Prefabs")]
    [SerializeField] GameObject OrderStatusInfoPrefab;
    [SerializeField] GameObject PrintBillPrefab;
    [SerializeField] GameObject Print_FinalBillPrefab;


    [Header("UI")]
    [SerializeField] TMP_Text OrderIDText;
    [SerializeField] TMP_Text ApproximateTimeLeftText;

    public OrderSummary OrderSummaryObject;

    GameObject OrderStatusInfoObject;

    

   

    //Called by OrderStatusManager.
    public void InitializeRequiredScripts  (
                                            Scene3Manager       Scene3ManagerScript,
                                            PrintBillsManager   PrintBillsManagerScript,
                                            OrderStatusManager  OrderStatusManagerScript
                                           )
    {
        this.Scene3ManagerScript        = Scene3ManagerScript;
        this.PrintBillsManagerScript    = PrintBillsManagerScript;
        this.OrderStatusManagerScript   = OrderStatusManagerScript;
    }
    public void SetOrderSummary(OrderSummary OrderSummaryObject)
    {
        this.OrderSummaryObject = OrderSummaryObject;

        UpdateUI();
    }
    private void UpdateUI()
    {
        if (OrderIDText != null)
        {
            OrderIDText.text = OrderSummaryObject.OrderID.ToString();
        }

        if (ApproximateTimeLeftText != null)
        {
            ApproximateTimeLeftText.text = OrderSummaryObject.ApproximateTimeLeft + " min";
        }
    }
    




    public void ClickedOrderStatusObject()
    {
        if (OrderSummaryObject == null)
        {
            Debug.LogError("Could not find OrderSummary Object.");
            return;
        }

        CreateOrderStatusInfo();

        Scene3ManagerScript.DisableRecentOrdersPanel();
    }
    private void CreateOrderStatusInfo()
    {
        OrderStatusInfoObject = Instantiate(
                                                OrderStatusInfoPrefab,
                                                OrderStatusManagerScript.transform,
                                                false
                                           );

        OrderStatusInfo OrderStatusInfoScript = OrderStatusInfoObject.GetComponent<OrderStatusInfo>();

        OrderStatusInfoScript.InitializeScene3ManagerScript(Scene3ManagerScript);

        OrderStatusInfoListParentScript Parent = OrderStatusInfoObject.GetComponentInChildren<OrderStatusInfoListParentScript>();

        if (Parent == null)
        {
            Debug.LogError("OrderStatusInfoListParentScript not found.");
            return;
        }

        Transform BillParent = Parent.transform;

        foreach (PrintDetails Print in OrderSummaryObject.Prints)
        {
            GameObject BillObject = Instantiate(
                                                    PrintBillPrefab,
                                                    BillParent,
                                                    false
                                               );

            PrintBill BillScript = BillObject.GetComponent<PrintBill>();

            BillScript.SetPrintBillsManagerScript(PrintBillsManagerScript);
            BillScript.SetBillDetailsInBillScript(Print);
        }

        CreateFinalBill(BillParent);
    }
    private void CreateFinalBill(Transform BillParent)
    {
        GameObject FinalBillObject = Instantiate(Print_FinalBillPrefab, BillParent, false);

        Print_FinalBill FinalBillScript = FinalBillObject.GetComponent<Print_FinalBill>();

        FinalBillScript.SetFinalBillAmount(
                                            OrderSummaryObject.FinalAmount,
                                            OrderSummaryObject.OrderID,
                                            OrderSummaryObject.OrderOTP
                                          );
    }



    public void ClickedPreviousPageButton()
    {
        if (OrderStatusInfoObject != null)
        {
            Destroy(OrderStatusInfoObject);
        }

        Scene3ManagerScript.DisableAllPanels();
        Scene3ManagerScript.HomePage();

        //Scene3ManagerScript.EnableRecentOrdersPanel();

    }
}