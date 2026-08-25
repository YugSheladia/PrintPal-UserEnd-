//Script6

using TMPro;
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
    public int TotalBillsCompleted;

    [SerializeField] public TMP_Text StationeryName;




    public void UpdateStationeryNameInUI()
    {
        StationeryName.text = Script1.StationeryName;
    }


    public void CreateBills()
    {
        DestroyAllBills();

        FinalBillAmount = 0;
        TotalBillsCompleted = 0;

        foreach (PrintDetails Print in PrintManagerScript.PrintDetailsList)
        {
            GameObject PrintBillObject = Instantiate(
                                                        PrintBillPrefab,
                                                        PrintBillsParent.transform,
                                                        false
                                                    );

            PrintBill PrintBillScript = PrintBillObject.GetComponent<PrintBill>();

            PrintBillScript.SetRequiredScripts(Scene3ManagerScript ,Scene3_BackendScript, this);
            PrintBillScript.SetBillDetailsInBillScript(Print);

            PrintBillScript.CalculatePrice();
        }
    }
    public void BillsFinished(PrintBill Bill)
    {
        FinalBillAmount += Bill.TotalPrice;

        TotalBillsCompleted++;

        if(TotalBillsCompleted == PrintManagerScript.PrintDetailsList.Count)
        {
            CreateFinalBill();

            Scene3ManagerScript.DisableAllPanels();

            StationeryName.text = Script1.StationeryName;
            Scene3ManagerScript.EnablePrintBillsManager();
        }
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

        Debug.Log("Clicked Pay button.");

        CheckPrinterRequirementsBeforePayment();
    }

    private void CheckPrinterRequirementsBeforePayment()
    {
        bool RequiresColorPrinting = false;
        bool RequiresDoubleSidedPrinting = false;

        foreach (PrintDetails Print in PrintManagerScript.PrintDetailsList)
        {
            if (Print.Color == ColorEnum.Color)
            {
                RequiresColorPrinting = true;
            }

            if (Print.SideSelection == SideSelectionEnum.DoubleSided)
            {
                RequiresDoubleSidedPrinting = true;
            }
        }

        Debug.Log
        (
            "Order Printer Requirements\n" +
            "Color Printing Required = " + RequiresColorPrinting + "\n" +
            "Double Sided Printing Required = " + RequiresDoubleSidedPrinting
        );

        // No special printer capability is required.
        // Let the order proceed normally.
        if (!RequiresColorPrinting && !RequiresDoubleSidedPrinting)
        {
            Scene3_BackendScript.CreateOrder();

            return;
        }

        // We need to check the stationery's printers.
        StartCoroutine
        (
            CheckStationeryPrinterCapabilities
            (
                RequiresColorPrinting,
                RequiresDoubleSidedPrinting
            )
        );
    }

    private System.Collections.IEnumerator CheckStationeryPrinterCapabilities
    (
        bool RequiresColorPrinting,
        bool RequiresDoubleSidedPrinting
    )
    {
        int StationeryID =
            Script1.StationeryID;

        Scene3_BackendScript.GetStationeryPrinters
        (
            StationeryID,
            Printers =>
            {
                if (Printers == null)
                {
                    Debug.LogError
                    (
                        "Failed to retrieve stationery printers."
                    );

                    Scene3ManagerScript.DisableInvisiblePanel();

                    Scene3ManagerScript.MessageText.text =
                        "Unable to verify printer capabilities.\n" +
                        "Please try again later.";

                    Scene3ManagerScript.EnableMessagePanel();

                    return;
                }

                bool HasCompatiblePrinter = false;

                foreach (StationeryPrinterInfo Printer in Printers)
                {
                    bool SupportsRequiredColor =
                        !RequiresColorPrinting ||
                        Printer.SupportsColorPrinting;

                    bool SupportsRequiredDoubleSided =
                        !RequiresDoubleSidedPrinting ||
                        Printer.SupportsDoubleSidedPrinting;

                    if
                    (
                        SupportsRequiredColor &&
                        SupportsRequiredDoubleSided
                    )
                    {
                        HasCompatiblePrinter = true;

                        Debug.Log
                        (
                            "Compatible printer found: " +
                            Printer.PrinterName
                        );

                        break;
                    }
                }

                if (!HasCompatiblePrinter)
                {
                    Debug.LogWarning
                    (
                        "No compatible printer found for this order."
                    );

                    Scene3ManagerScript.DisableInvisiblePanel();

                    string Message =
                        "This stationery does not have a printer ";

                    if
                    (
                        RequiresColorPrinting &&
                        RequiresDoubleSidedPrinting
                    )
                    {
                        Message +=
                            "that supports both Color and Double-Sided printing.";
                    }
                    else if (RequiresColorPrinting)
                    {
                        Message +=
                            "that supports Color printing.";
                    }
                    else if (RequiresDoubleSidedPrinting)
                    {
                        Message +=
                            "that supports Double-Sided printing.";
                    }

                    Scene3ManagerScript.MessageText.text = Message;

                    Scene3ManagerScript.EnableMessagePanel();

                    return;
                }

                // A compatible printer exists.
                Debug.Log
                (
                    "Printer requirements satisfied. " +
                    "Creating order."
                );

                Scene3_BackendScript.CreateOrder();
            }
        );

        yield return null;
    }

    public void ConfirmOrder()
    {
        Debug.Log("Order Confirmed.");

        PrintManagerScript.ClearPrints();

        Scene3ManagerScript.DisableAllPanels();
        Scene3ManagerScript.HomePage();
    }

    public void ClickedPreviousPageButton()
    {
        Scene3ManagerScript.CreateOrderPage();

        DestroyAllBills();
    }

    public void ClickedChangeStationery()
    {
        Scene3ManagerScript.EnableInvisiblePanel();

        Scene3_BackendScript.GetStationeries();
    }
}