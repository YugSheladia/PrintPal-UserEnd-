//Scrtip4

using TMPro;
using UnityEngine;


public class Print : MonoBehaviour
{
    [Header("Scene5Manager stuff")]
    Scene3Manager Scene3ManagerScript;


    [Header("OrderManager stuff")]
    PrintManager PrintManagerScript;


    [Header("Order stuff")]
    [SerializeField] private    TMP_Text        OrderNumberText;
    [SerializeField] public     TMP_InputField  PDFFileNameInputField;


    [Header("PrintSettings stuff")]
    [SerializeField] private GameObject PrintSettingsPrefab;
    private Transform PrintSettingsParentTransform;
    GameObject PrintSettingsObject;
    PrintSettings PrintSettingsScript;


    [Header("Other stuff")]
    private PrintDetails PrintDetailsObject;



    //functions called by OrderManager ->
    public void InitializeRequiredScripts(
                                            Scene3Manager Scene3ManagerScript, 
                                            PrintManager PrintManagerScript,
                                            Transform PrintSettingsParentTransform
                                         )
    {       
        this.Scene3ManagerScript            = Scene3ManagerScript;
        this.PrintManagerScript             = PrintManagerScript;
        this.PrintSettingsParentTransform   = PrintSettingsParentTransform;
    }
    public void SetOrderData(PrintDetails PrintData)
    {
        this.PrintDetailsObject = PrintData;
    }
    public void SetOrderNumberInUI()
    {
        OrderNumberText.text = "#" + (PrintDetailsObject.PrintNumber + 1).ToString();
    }
    public PrintDetails GetOrderData()
    {
        return PrintDetailsObject;
    }



    public void EnablePrintSettings()
    {
        PrintSettingsObject.SetActive(true);
    }
    public void DisablePrintSettings()
    {
        PrintSettingsObject.SetActive(false);
    }



    public void OnFileNameChanged(string FileName)
    {
        PrintDetailsObject.PDFName = FileName;
    }
    public void ClickedPrintSettingsButton()
    {
        if (PrintSettingsObject)
        {

        }
        else
        {
            PrintSettingsObject = Instantiate(PrintSettingsPrefab, PrintSettingsParentTransform);
            PrintSettingsScript = PrintSettingsObject.GetComponent<PrintSettings>();

            PrintSettingsScript.InitialiseScene5ManagerScript(Scene3ManagerScript);
            PrintSettingsScript.SetPrintScript(this);
        }

        EnablePrintSettings();
        Scene3ManagerScript.DisablePrintManager();
    }
    public void ClickedRemoveOrderButton()
    {
        if (PrintSettingsScript != null)
        {
            Destroy(PrintSettingsScript.gameObject);
        }

        PrintManagerScript.RemoveOrder(this);

        Destroy(gameObject);
    }



    public void DestroyOrderSettingsObject()
    {
        if (PrintSettingsObject != null)
        {
            Destroy(PrintSettingsObject);
        }
    }  
}


/*
 * Template height = Height of the dropdown popup.
 * 
 * Item height = Height of each option, and decides how close each items are to each other.
 * 
 * Content height = It is an invisible container containing all items.
 * Consider each item height = 80 px, and there are 5 such items... then 5*80 = 400 px
 * Therefore Content height should be = 400 px.
 * 
 * Example -
 * DropDownObject height = 100
 * font size = 48
 * 
 * Template height = 200
 * Content = 100
 * Item = 100
*/
