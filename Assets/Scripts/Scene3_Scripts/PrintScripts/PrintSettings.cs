//Script5

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrintSettings : MonoBehaviour
{
    [Header("Scene5Manager stuff")]
    Scene3Manager Scene3ManagerScript;


    [Header("Order stuff")]
    Print PrintScript;


    [Header("Other stuff")]
    [SerializeField] private TMP_Dropdown   PageSelectionDropDown;
    [SerializeField] private TMP_Dropdown   SideSelectionDropDown;
    [SerializeField] private TMP_Dropdown   CollatedDropDown;
    [SerializeField] private TMP_Dropdown   OrientationDropDown;
    [SerializeField] private TMP_Dropdown   MarginDropDown;
    [SerializeField] private TMP_Dropdown   PagesPerSheetDropDown;
    [SerializeField] private TMP_Dropdown   ColorDropDown;
    [SerializeField] private TMP_InputField Copies;
    [SerializeField] private TMP_InputField Pages;


    PrintDetails PrintDetailsObject;
 

    //Called by Print.
    public void InitialiseScene5ManagerScript(Scene3Manager Scene3ManagerScript)
    {
        this.Scene3ManagerScript = Scene3ManagerScript;
    }
    public void SetPrintScript(Print Print)
    {
        PrintScript = Print;
    }



    public void ClickedContinueButton()
    {
        PageSelectionEnum   PageSelection   = (PageSelectionEnum)   PageSelectionDropDown   .value;
        SideSelectionEnum   SideSelection   = (SideSelectionEnum)   SideSelectionDropDown   .value;
        CollatedOrNotEnum   CollatedOrNot   = (CollatedOrNotEnum)   CollatedDropDown        .value;
        OrientationEnum     Orientation     = (OrientationEnum)     OrientationDropDown     .value;
        MarginEnum          Margin          = (MarginEnum)          MarginDropDown          .value;
        PagesPerSheetEnum   PagesPerSheet   = (PagesPerSheetEnum)   PagesPerSheetDropDown   .value;
        ColorEnum           Color           = (ColorEnum)           ColorDropDown           .value;

        int CopiesInt  = int.Parse(Copies.text);
        int PagesInt   = int.Parse(Pages.text);

        PrintDetailsObject = PrintScript.GetOrderData();

        PrintDetailsObject.PageSelection  = PageSelection;
        PrintDetailsObject.SideSelection  = SideSelection;
        PrintDetailsObject.CollatedOrNot  = CollatedOrNot;
        PrintDetailsObject.Orientation    = Orientation;
        PrintDetailsObject.Margin         = Margin;
        PrintDetailsObject.PagesPerSheet  = PagesPerSheet;
        PrintDetailsObject.Color          = Color;

        PrintDetailsObject.Copies = CopiesInt;
        PrintDetailsObject.Pages  = PagesInt;

        print("Inside PrintSettings.ClickedContinueButton() => " + SideSelection +SideSelection.ToString());
        
        Scene3ManagerScript.EnablePrintManager();
        PrintScript.DisablePrintSettings();
    }
}
