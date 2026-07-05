//Script9

using UnityEngine;

public class OrderStatusManager : MonoBehaviour
{
    [Header("Scene3Manager")]
    [SerializeField] Scene3Manager Scene3ManagerScript;

    [Header("PrintManager")]
    [SerializeField] PrintManager PrintManagerScript;

    [Header("PrintBillsManager")]
    [SerializeField] PrintBillsManager PrintBillsManagerScript;

    [Header("Order Status")]
    [SerializeField] GameObject OrderStatusPrefab;
    [SerializeField] GameObject OrderStatusListParent;

    [SerializeField] Scene3_Backend Scene3_BackendScript;

   

    public void CreateOrderStatusPanels()
    {
        foreach (OrderSummary Order in Scene3_BackendScript.ConfirmedOrders.Orders) 
        {
            CreateOrderStatusPanel(Order);
        }
    }
    public void CreateOrderStatusPanel(OrderSummary Order)
    {
        foreach (OrderStatus Panel in OrderStatusListParent.GetComponentsInChildren<OrderStatus>())
        {
            if (Panel.OrderSummaryObject.OrderID == Order.OrderID) { return; }
        }

        GameObject OrderStatusObject = Instantiate(
                                                    OrderStatusPrefab,
                                                    OrderStatusListParent.transform
                                                  );

        OrderStatus OrderStatusScript =  OrderStatusObject.GetComponent<OrderStatus>();

        OrderStatusScript.InitializeRequiredScripts(
                                                        Scene3ManagerScript, 
                                                        PrintBillsManagerScript, 
                                                        this
                                                    );

        OrderStatusScript.SetOrderSummary(Order);
    }
    



    public void ClickedPreviousPageButton()
    {
        foreach (Transform Child in OrderStatusListParent.transform)
        {
            Destroy(Child.gameObject);
        }

        Scene3ManagerScript.HomePage();
    }
}