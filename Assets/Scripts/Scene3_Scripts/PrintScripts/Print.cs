//Scrtip4

using TMPro;
using UnityEngine;
using System.IO;


public class Print : MonoBehaviour
{
    [Header("Scene5Manager stuff")]
    Scene3Manager Scene3ManagerScript;
    Scene3_Backend Scene3_BackendScript;


    [Header("PrintManager stuff")]
    PrintManager PrintManagerScript;


    [Header("Print stuff")]
    [SerializeField] private TMP_Text PrintNumberText;
    [SerializeField] private TMP_Text PDFNameText;


    [Header("PrintSettings stuff")]
    [SerializeField] private GameObject PrintSettingsPrefab;
    private Transform PrintSettingsParentTransform;
    GameObject PrintSettingsObject;
    PrintSettings PrintSettingsScript;


    [Header("Other stuff")]
    private PrintDetails PrintDetailsObject;



    [SerializeField] public GameObject PDFNotAttachedPanel;
    [SerializeField] public GameObject PDFAttachedPanel;





    //functions called by PrintManager ->
    public void InitializeRequiredScripts(
                                            Scene3Manager Scene3ManagerScript,
                                            Scene3_Backend Scene3_BackendScript,
                                            PrintManager PrintManagerScript,
                                            Transform PrintSettingsParentTransform
                                         )
    {       
        this.Scene3ManagerScript            = Scene3ManagerScript;
        this.Scene3_BackendScript           = Scene3_BackendScript;
        this.PrintManagerScript             = PrintManagerScript;
        this.PrintSettingsParentTransform   = PrintSettingsParentTransform;
    }
    public void SetOrderData(PrintDetails PrintData)
    {
        this.PrintDetailsObject = PrintData;
    }
    public void SetPrintNumberInUI()
    {
        PrintNumberText.text = (PrintDetailsObject.PrintNumber+1).ToString();
    }
    public PrintDetails GetOrderData()
    {
        return PrintDetailsObject;
    }



    public void ClickedSelectPDFFile()
    {
        SelectPDF();
    }
    public void SelectPDF()
    {
        NativeFilePicker.PickFile(
            (path) =>
            {
                if (string.IsNullOrEmpty(path))
                {
                    Debug.Log("No file selected.");
                    return;
                }

                Debug.Log("Selected PDF:");
                Debug.Log(path);

                PrintDetailsObject.PDFFilePath = path;
                PrintDetailsObject.PDFName = Path.GetFileName(path);

                PDFNameText.text = Path.GetFileName(path);

                PDFNotAttachedPanel.SetActive(false);
                PDFAttachedPanel.SetActive(true);
            },
            new string[]
            {
            "application/pdf"
            });
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

        Scene3ManagerScript.DisableAllPanels();
        EnablePrintSettings();

        //Scene3ManagerScript.DisablePrintManager();
    }
    public void EnablePrintSettings()
    {
        PrintSettingsObject.SetActive(true);
    }
    public void DisablePrintSettings()
    {
        PrintSettingsObject.SetActive(false);
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
