//Script10

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderStatus : MonoBehaviour
{
    Scene3Manager  Scene3ManagerScript;
    Scene3_Backend Scene3_BackendScript;

    PrintBillsManager   PrintBillsManagerScript;
    OrderStatusManager  OrderStatusManagerScript;


    [Header("Prefabs")]
    [SerializeField] GameObject OrderStatusInfoPrefab;
    [SerializeField] GameObject PrintBillPrefab;
    [SerializeField] GameObject Print_FinalBillPrefab;


    [Header("UI")]
    [SerializeField] TMP_Text OrderStatusText;
    [SerializeField] TMP_Text StatusTimeStampText;
    [SerializeField] public TMP_Text StationeryNameText;
    [SerializeField] TMP_Text OrderIDText;
    [SerializeField] TMP_Text OrderOTPText;

    [SerializeField] GameObject ArchivedOrderButtonsPanel;
    [SerializeField] GameObject UnarchivedOrderButtonsPanel;

    [SerializeField] private Image ViewDetailsObject;
    [SerializeField] private Image StatusIndicator;

    [SerializeField] private GameObject StatusBox;
    [SerializeField] private Image      StatusBoxColor;
    [SerializeField] private TMP_Text   StatusBoxText;

    public OrderSummary OrderSummaryObject;

    GameObject OrderStatusInfoObject;

    enum OrderStatusEnum
    {
        Pending,
        Completed,
        Collected
    }

    OrderStatusEnum orderStatusEnum;


    

   

    //Called by OrderStatusManager.
    public void InitializeRequiredScripts  (
                                            Scene3Manager       Scene3ManagerScript,
                                            Scene3_Backend      Scene3_BackendScript,
                                            PrintBillsManager   PrintBillsManagerScript,
                                            OrderStatusManager  OrderStatusManagerScript
                                           )
    {
        this.Scene3ManagerScript    = Scene3ManagerScript;
        this.Scene3_BackendScript   = Scene3_BackendScript;

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
        orderStatusEnum = (OrderStatusEnum)OrderSummaryObject.Status;

        switch (orderStatusEnum)
        {
            case OrderStatusEnum.Pending:
                OrderStatusText.text = "Queued for Printing";

                StatusBoxText.text = "Pending";
                StatusTimeStampText.text = "Created at : " + OrderSummaryObject.CreatedAt.ToString();

                StatusIndicator.color = new Color32(246, 59, 57, 200);
                OrderStatusText.color = new Color32(246, 59, 57, 200);
                //ViewDetailsObject.color = new Color32(246, 59, 57, 200);
                StatusBoxColor.color = new Color32(246, 59, 57, 200);
                
                break;

            case OrderStatusEnum.Completed:
                OrderStatusText.text = "Ready for Pickup";

                StatusBoxText.text = "Printed";
                StatusTimeStampText.text = "Printed at : " + OrderSummaryObject.CompletedAt.ToString();

                StatusIndicator.color = new Color32(245, 207, 73, 230);
                OrderStatusText.color = new Color32(245, 207, 73, 230);
                //ViewDetailsObject.color = new Color32(245, 207, 73, 230);
                StatusBoxColor.color = new Color32(245, 207, 73, 230);

                break;

            case OrderStatusEnum.Collected:
                OrderStatusText.text = "Has been collected";

                StatusBoxText.text = "Collected";
                StatusTimeStampText.text = "Collected at : " + OrderSummaryObject.CollectedAt.ToString();

                StatusIndicator.color = new Color32(2, 141, 26, 220);
                OrderStatusText.color = new Color32(2, 141, 26, 220);
                //ViewDetailsObject.color = new Color32(2, 141, 26, 220);
                StatusBoxColor.color = new Color32(2, 141, 26, 220);

                break;                

            default:
                Debug.Log("Invalid Order Status");
                Destroy(gameObject);
                break;
        }

        Scene3_BackendScript.GetStationeryNameFromOrderID(OrderSummaryObject.OrderID, this);

        OrderIDText.text = OrderSummaryObject.OrderID.ToString();

        OrderOTPText.text = OrderSummaryObject.OrderOTP.ToString();
    }
    




    public void ClickedOrderStatusObject()
    {
        if (OrderSummaryObject == null)
        {
            Debug.LogError("Could not find OrderSummary Object.");
            return;
        }

        Scene3ManagerScript.EnableInvisiblePanel();
        CreateOrderStatusInfo();
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

            BillScript.SetRequiredScripts(Scene3ManagerScript, Scene3_BackendScript, PrintBillsManagerScript);
            BillScript.SetBillDetailsInBillScript(Print);
            BillScript.SetBillDetailsInUI();
        }

        CreateFinalBill(BillParent);

        Scene3ManagerScript.DisableInvisiblePanel();
        Scene3ManagerScript.DisableOrdersPanel();
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
    }
}
