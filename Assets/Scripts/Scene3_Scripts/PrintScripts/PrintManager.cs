//Script3

using UnityEngine;
using System.Collections.Generic;


public enum PageSelectionEnum
{
    AllPages,
    OddPages,
    EvenPages
}
public enum SideSelectionEnum
{
    SingleSided,
    DoubleSided
}
public enum CollatedOrNotEnum
{
    Collated,
    Uncollated
}
public enum OrientationEnum
{
    Portrait,
    Landscape
}
public enum MarginEnum
{
    Normal,
    Narrow,
    Moderate,
    Wide,
    Mirrored,
    DoubleSided
}
public enum PagesPerSheetEnum
{
    _1,
    _2,
    _4,
    _8,
    _16
}
public enum ColorEnum
{
    BlackAndWhite,
    Color
}


public class PrintDetails
{
    public Print PrintScript;

    public int OrderID;
    public int PrintID;

    public int      PrintNumber;
    public string   PDFName;

    public string PrintFilePath;

    public PageSelectionEnum    PageSelection   = PageSelectionEnum.AllPages;
    public SideSelectionEnum    SideSelection   = SideSelectionEnum.SingleSided;
    public CollatedOrNotEnum    CollatedOrNot   = CollatedOrNotEnum.Collated;
    public OrientationEnum      Orientation     = OrientationEnum.Portrait;
    public MarginEnum           Margin          = MarginEnum.Normal;
    public PagesPerSheetEnum    PagesPerSheet   = PagesPerSheetEnum._1;
    public ColorEnum            Color           = ColorEnum.BlackAndWhite;

    public int Copies = 1;
    public int Pages = 1;

    public int EffectiveSheets;
    public float BillAmount;

    public PrintDetails()
    {

    }


    // CHANGE:
    // Copy constructor used when saving orders.
    public PrintDetails(PrintDetails Source)
    {
        PrintNumber = Source.PrintNumber;
        PDFName     = Source.PDFName;

        PageSelection   = Source.PageSelection;
        SideSelection   = Source.SideSelection;
        CollatedOrNot   = Source.CollatedOrNot;
        Orientation     = Source.Orientation;
        Margin          = Source.Margin;
        PagesPerSheet   = Source.PagesPerSheet;
        Color           = Source.Color;

        Copies  = Source.Copies;
        Pages   = Source.Pages;
    }


    public void PrintOrderDetails()
    {
        Debug.Log(
            "#"                     + (PrintNumber + 1) +
            "\nPDF Name         = " + PDFName           +
            "\nPage Selection   = " + PageSelection     +
            "\nSide Selection   = " + SideSelection     +
            "\nCollated         = " + CollatedOrNot     +
            "\nOrientation      = " + Orientation       +
            "\nMargin           = " + Margin            +
            "\nPagesPerSheet    = " + PagesPerSheet     +
            "\nCopies           = " + Copies            +
            "\nPages            = " + Pages);
    }
}


public class PrintManager : MonoBehaviour
{
    [Header("Scene3Manager stuff")]
    [SerializeField] Scene3Manager Scene3ManagerScript;
    [SerializeField] Scene3_Backend Scene3_BackendScript;


    [Header("Order stuff")]
    [SerializeField] private GameObject PrintPrefab;
    [SerializeField] private Transform PrintListParent;


    [Header("OrderSettings stuff")]
    [SerializeField] GameObject PrintSettingsParent;


    [Header("PrintBillsManager stuff")]
    [SerializeField] PrintBillsManager PrintBillsManagerScript;


    [Header("Other stuff")]
    public List<PrintDetails> PrintDetailsList = new();




    public void AddOrder()
    {
        GameObject  NewPrint = Instantiate(PrintPrefab, PrintListParent);
        Print       PrintScript = NewPrint.GetComponent<Print>();

        PrintDetails NewPrintDetails = new();

        NewPrintDetails.PrintScript = PrintScript;
        NewPrintDetails.PrintNumber = PrintDetailsList.Count;

        PrintDetailsList.Add(NewPrintDetails);

        PrintScript.InitializeRequiredScripts(Scene3ManagerScript, Scene3_BackendScript, this, PrintSettingsParent.transform);
        
        PrintScript.SetOrderData(NewPrintDetails);
        PrintScript.SetPrintNumberInUI();

        Debug.Log("Total Prints = " + PrintDetailsList.Count);
    }



    public void RemoveOrder(Print PrintScript)
    {
        if (PrintScript == null)
        {
            return;
        }

        PrintDetailsList.Remove(PrintScript.GetOrderData());

        ShiftOrders();
    }
    public void ShiftOrders()
    {
        for (int c1 = 0; c1 < PrintDetailsList.Count; c1++)
        {
            PrintDetailsList[c1].PrintNumber = c1;
            PrintDetailsList[c1].PrintScript.SetPrintNumberInUI();
        }

        Debug.Log("OrdersDetailList count: " + PrintDetailsList.Count);
    }



    public void ClickedPrintButton()
    {
        if (PrintDetailsList.Count <= 0)
        {
            print("Atleast 1 print is required.");
        }
        else
        {
            PrintBillsManagerScript.CreateBills();    
        }
    }
    public void ClickedPreviousPageButton()
    {
        Scene3ManagerScript.HomePage();
    }



    public void ClearOrders()
    {
        DestroyAllOrderObjects();
        ClearOrdersDetailList();

        PrintBillsManagerScript.DestroyAllBills();
    }   
    private void DestroyAllOrderObjects()
    {
        // CHANGE:
        // Destroy actual Order objects in hierarchy.

        Print[] Orders = PrintListParent.GetComponentsInChildren<Print>();

        foreach (Print OrderScript in Orders)
        {
            if (OrderScript != null)
            {
                OrderScript.DestroyOrderSettingsObject();

                Destroy(OrderScript.gameObject);
            }
        }
    }
    private void ClearOrdersDetailList()
    {
        PrintDetailsList.Clear();
    }
}