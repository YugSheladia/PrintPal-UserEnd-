//Script6

using Unity.VisualScripting;
using UnityEngine;

public class PrintBillsManager : MonoBehaviour
{
    [Header("Scene3Manager stuff")]
    [SerializeField] Scene3Manager Scene3ManagerScript;
    [SerializeField] Scene3_Backend Scene3_BackendScript;

    [Header("OrderManager stuff")]
    [SerializeField] PrintManager PrintManagerScript;

    [Header("Bill stuff")]
    [SerializeField] GameObject PrintBillPrefab;
    [SerializeField] GameObject Print_FinalBillPrefab;

    [SerializeField] GameObject PrintBillsParent;

    GameObject FinalBillObject;
    Print_FinalBill FinalBillScript;

    public float FinalBillAmount;



    public void CreateBills()
    {
        DestroyAllBills();

        FinalBillAmount = 0;

        foreach (PrintDetails Print in PrintManagerScript.PrintDetailsList)
        {
            GameObject PrintBillObject = Instantiate(
                                                        PrintBillPrefab,
                                                        PrintBillsParent.transform,
                                                        false
                                                    );

            PrintBill PrintBillScript = PrintBillObject.GetComponent<PrintBill>();

            PrintBillScript.SetBillDetailsInBillScript(Print);

            Print.EffectiveSheets   = PrintBillScript.EffectiveSheets;
            Print.BillAmount        = PrintBillScript.TotalPrice;

            FinalBillAmount += PrintBillScript.TotalPrice;
        }


        CreateFinalBill();

        Scene3ManagerScript.EnablePrintBillsManager();
        Scene3ManagerScript.DisablePrintManager();
    }
    public void DestroyAllBills()
    {
        FinalBillAmount = 0;

        foreach (Transform Child in PrintBillsParent.transform)
        {
            Destroy(Child.gameObject);
        }

        FinalBillObject = null;
        FinalBillScript = null;
    }
    private void CreateFinalBill()
    {
        FinalBillObject =Instantiate(
                                    Print_FinalBillPrefab,
                                    PrintBillsParent.transform,
                                    false
                                    );

        FinalBillScript = FinalBillObject.GetComponent<Print_FinalBill>();

        FinalBillScript.SetFinalBillAmount(FinalBillAmount);
    }







    public void ClickedPayButton()
    {
        Scene3ManagerScript.EnableInvisiblePanel();
        Debug.Log("Clicked Pay button");

        Scene3_BackendScript.CreateOrder();
    }
    
    public void ConfirmOrders()
    {
        Debug.Log("Orders Confirmed.");

        PrintManagerScript.ClearOrders();


        Scene3ManagerScript.DisableAllPanels();
        Scene3ManagerScript.HomePage();
    }

    public void ClickedPreviousPageButton()
    {
        Scene3ManagerScript.CreateOrderPage();

        DestroyAllBills();
    }
}