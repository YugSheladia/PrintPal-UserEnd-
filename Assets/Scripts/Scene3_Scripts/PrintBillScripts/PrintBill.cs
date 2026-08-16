//Script7

using TMPro;
using UnityEngine;


public class BillRequest
{
    public int PageSelection;
    public int SideSelection;

    public int PagesPerSheet;
    
    public int Color;
    public int Copies;
}

public class BillResponse
{
    public bool Success;
    public string Message;

    public int TotalPages;
    public int EffectiveSheets;

    public float UnitPrice;
    public float BillAmount;
}




public class PrintBill : MonoBehaviour
{
    Scene3Manager       Scene3ManagerScript;
    Scene3_Backend      Scene3_BackendScript;
    PrintBillsManager   PrintBillsManagerScript;

    [SerializeField] public TMP_Text PrintNumberText;
    [SerializeField] TMP_Text PDFNameText;
    [SerializeField] TMP_Text PagesValueText;
    [SerializeField] TMP_Text CopiesValueText;
    [SerializeField] TMP_Text ColorValueText;
    [SerializeField] TMP_Text SideSelectionValueText;
    [SerializeField] TMP_Text PagesPerSheetText;

    [SerializeField] TMP_Text Price;
    [SerializeField] TMP_Text PriceCalculation;

    [SerializeField] TMP_Text OrientationText;
    [SerializeField] TMP_Text PageSelectionText;
    [SerializeField] TMP_Text CollationText;
    [SerializeField] TMP_Text MarginText;

    PrintDetails PrintDetailsObject;

    int             TotalPages;
    float           UnitPrice;
    public int      EffectiveSheets;
    public float    TotalPrice;

    BillRequest Request;
    BillResponse Response;


    public void SetRequiredScripts(
                                        Scene3Manager Scene3ManagerScript,
                                        Scene3_Backend Scene3_BackendScript, 
                                        PrintBillsManager PrintBillsManagerScript
                                  )
    {
        this.Scene3ManagerScript        = Scene3ManagerScript;
        this.Scene3_BackendScript       = Scene3_BackendScript;
        this.PrintBillsManagerScript    = PrintBillsManagerScript;
    }





    public void SetBillDetailsInBillScript(PrintDetails PrintDetailsObject)
    {
        this.PrintDetailsObject = PrintDetailsObject;
    }
    public void CalculatePrice()
    {
        Request = new BillRequest();

        Request.PageSelection = (int)PrintDetailsObject.PageSelection;
        Request.SideSelection = (int)PrintDetailsObject.SideSelection;

        Request.PagesPerSheet = (int)PrintDetailsObject.PagesPerSheet;

        Request.Color         = (int)PrintDetailsObject.Color;
        Request.Copies        = PrintDetailsObject.Copies;

        Scene3_BackendScript.CalculateBill(Request, PrintDetailsObject.PDFFilePath, OnBillCalculated);
    }
    private void OnBillCalculated(BillResponse response)
    {
        if (response == null)
        {
            Debug.LogError(response.Message);

            Scene3ManagerScript.MessageText.text = "An error occured, try restarting the app.";
            Scene3ManagerScript.EnableMessagePanel();

            Scene3ManagerScript.DisableAllPanels();
            Scene3ManagerScript.EnablePrintManager(); 

            return;
        }

        if (!response.Success)
        {
            Debug.LogError(response.Message);

            Scene3ManagerScript.MessageText.text = "An error occured, try restarting the app.";
            Scene3ManagerScript.EnableMessagePanel();

            Scene3ManagerScript.DisableAllPanels();
            Scene3ManagerScript.EnablePrintManager(); //

            return;
        }

        Response = response;

        PrintDetailsObject.Pages            = response.TotalPages;
        PrintDetailsObject.EffectiveSheets  = response.EffectiveSheets;
        PrintDetailsObject.BillAmount       = response.BillAmount;
        PrintDetailsObject.UnitPrice        = response.UnitPrice;

        TotalPages      = response.TotalPages;
        EffectiveSheets = response.EffectiveSheets;
        UnitPrice       = response.UnitPrice;
        TotalPrice      = response.BillAmount;

        SetBillDetailsInUI();

        PrintBillsManagerScript.BillsFinished(this);
    }
    public void SetBillDetailsInUI()
    {
        PrintNumberText.text = (PrintDetailsObject.PrintNumber + 1).ToString();

        PDFNameText.text = PrintDetailsObject.PDFName;

        PagesValueText.text = PrintDetailsObject.Pages.ToString();
        CopiesValueText.text = PrintDetailsObject.Copies.ToString();

        ColorValueText.text =
            PrintDetailsObject.Color == ColorEnum.BlackAndWhite
            ? "Black & White"
            : "Color";

        SideSelectionValueText.text = PrintDetailsObject.SideSelection.ToString();
        PagesPerSheetText.text = PrintDetailsObject.PagesPerSheet.ToString();

        Price.text = "`" + PrintDetailsObject.BillAmount.ToString("F2");

        int BillableSheets = PrintDetailsObject.EffectiveSheets;

        PriceCalculation.text =
            BillableSheets + " Sheets x " +
            PrintDetailsObject.Copies + " Copies x `" +
            PrintDetailsObject.UnitPrice.ToString("F2");

        MarginText.text = PrintDetailsObject.Margin.ToString();
        OrientationText.text = PrintDetailsObject.Orientation.ToString();
        PageSelectionText.text = PrintDetailsObject.PageSelection.ToString();
        CollationText.text = PrintDetailsObject.CollatedOrNot.ToString();
    }
}