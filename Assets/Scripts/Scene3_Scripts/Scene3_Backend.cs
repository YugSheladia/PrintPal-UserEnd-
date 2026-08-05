using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;



//==================================
// CREATE ORDER CLASSES
//==================================
[System.Serializable]
public class CreateOrderRequest
{
    public int      UserID;
    public int      StationeryID;
    public float    FinalAmount;
    
    public List <int> PrintIDs;
}

[System.Serializable]
public class PrintDTO
{
    public int PrintNumber;
    public int PrintID;

    public string PDFName;
    public string PDFFilePath;

    public int Pages;
    public int Copies;

    public int PageSelection;
    public int SideSelection;
    public int Collated;
    public int Orientation;
    public int Margin;
    public int PagesPerSheet;
    public int Color;

    public int EffectiveSheets;
    public float BillAmount;
}

[System.Serializable]
public class CurrentOrderResponse
{
    public bool Success;

    public int OrderID;
    public int OTP;

    public int ApproximateTimeLeft;

    //public PrintSummary[] Prints;

    public string Message;
}




//=======================================================
// Receive all possible info about an issued order.
//=======================================================

// ORDERS TABLE
[System.Serializable]
public class OrderSummary
{
    public int OrderID;
    public int UserID;

    public int OrderOTP;

    public int ApproximateTimeLeft;

    public float FinalAmount;

    public int Status;

    public int ArchivedByUser;

    public string CreatedAt;
    public string CompletedAt;
    public string CollectedAt;

    public string ArchivedByUserAt;

    public int Completed;

    public List<PrintDetails> Prints = new();
}

[System.Serializable]
public class OrderSummaryWrapper
{
    public bool Success;

    public List<OrderSummary> Orders = new();
}


// PRINTS TABLE
[System.Serializable]
public class PrintSummary
{
    public int PrintID;

    public int PrintNumber;

    public int OrderID;

    public string PDFName;
    public string PDFFilePath;

    public int TotalPages;
}

[System.Serializable]
public class PrintSummaryWrapper
{
    public List<PrintSummary> Prints = new();
}


// PRINT SETTINGS TABLE
[System.Serializable]
public class PrintSettingsSummary
{
    public int PrintSettingsID;

    public int PrintID;

    public int PageSelection;
    public int SideSelection;
    public int Collated;
    public int Orientation;
    public int Margin;
    public int PagesPerSheet;

    public int Color;
    public int Copies;
}

[System.Serializable]
public class PrintSettingsSummaryWrapper
{
    public List<PrintSettingsSummary> PrintSettings = new();
}


// PRINT BILLS TABLE
[System.Serializable]
public class PrintBillsSummary
{
    public int PrintBillID;

    public int PrintID;

    public int TotalPages;

    public int EffectiveSheets;

    public int TotalCopies;

    public int Color;

    public int SideSelection;

    public int PagesPerSheet;

    public float BillAmount;
}

[System.Serializable]
public class PrintBillsSummaryWrapper
{
    public List<PrintBillsSummary> PrintBills = new();
}


// STATIONERIES TABLE
[System.Serializable]
public class StationerySummary
{
    public int StationeryID;

    public string StationeryName;

    public string UPINumber;
    public string PhoneNumber;
    public string Email;

    public int AutomaticPrinting;
    public int IsActive;

    public string CreatedAt;
}

[System.Serializable]
public class StationerySummaryWrapper
{
    public bool Success;
    public string Message;

    public StationerySummary[] Stationeries;
}


// PDF Upload Response
[System.Serializable]
public class UploadResponse
{
    public bool Success;
    public string Message;

    public int PrintID;
    public string PDFName;

    public int TotalPages;
    public int FileSize;
}




//==================================================
// MAIN CLASS
//==================================================
public class Scene3_Backend : MonoBehaviour
{
    [SerializeField] Scene3Manager      Scene3ManagerScript;
    [SerializeField] PrintManager       PrintManagerScript;
    [SerializeField] PrintBillsManager  PrintBillsManagerScript;
    [SerializeField] OrderStatusManager OrderStatusManagerScript;

    [SerializeField] UpdateStationery2 UpdateStationery2Script;


    // Creating an Order Request & Server Response.
    CreateOrderRequest      CurrentOrderRequest;
    CurrentOrderResponse    CurrentOrderResponse;


    // Wrapper objects.
    public OrderSummaryWrapper          ConfirmedOrders         = new();
    public PrintSummaryWrapper          ConfirmedPrints         = new();
    public PrintSettingsSummaryWrapper  ConfirmedPrintSettings  = new();
    public PrintBillsSummaryWrapper     ConfirmedPrintBills     = new();



    bool OperationSucceeded;

    public void CreateOrder()
    {
        StartCoroutine(CreateOrderRoutine());
    }

    IEnumerator CreateOrderRoutine()
    {
        //Very important : Setting OperationSucceded = true.
        //Say the user clicks on Pay, and for some reason, due to some error, OperationSucceeded becomes false.
        //If the user clicks on Pay again, it is important that OperationSucceeded is reset, hence the following line of code.
        OperationSucceeded = true;

        CurrentOrderRequest = new CreateOrderRequest();

        CurrentOrderRequest.UserID          = Script1.UserID;
        CurrentOrderRequest.StationeryID    = Script1.StationeryID;
        CurrentOrderRequest.FinalAmount     = PrintBillsManagerScript.FinalBillAmount;

        CurrentOrderRequest.PrintIDs        = new List<int>();


        yield return ValidateAllPDFs();


        if (!OperationSucceeded)
        {
            Scene3ManagerScript.MessageText.text = "An error occured, try restarting the app.";
            Scene3ManagerScript.EnableMessagePanel();

            Debug.Log("fffff");
            Scene3ManagerScript.DisableAllPanels();
            Scene3ManagerScript.EnablePrintBillsManager();

            yield break;
        }

        yield return UploadAllPrints();


        if (!OperationSucceeded)
        {
            Scene3ManagerScript.MessageText.text = "An error occured, try restarting the app.";
            Scene3ManagerScript.EnableMessagePanel();

            Debug.Log("gggg");
            Scene3ManagerScript.DisableAllPanels();
            Scene3ManagerScript.EnablePrintBillsManager();

            yield break;
        }
        
        yield return SubmitOrder();

        if (!OperationSucceeded)
        {
            Scene3ManagerScript.MessageText.text = "An error occured, try restarting the app.";
            Scene3ManagerScript.EnableMessagePanel();

            Debug.Log("hhhhh");
            Scene3ManagerScript.DisableAllPanels();
            Scene3ManagerScript.EnablePrintBillsManager();

            yield break;
        }

        Debug.Log("Done");

        PrintBillsManagerScript.ConfirmOrder();
    }

    IEnumerator ValidateAllPDFs()
    {
        foreach (PrintDetails print in PrintManagerScript.PrintDetailsList)
        {
            if (!File.Exists(print.PDFFilePath))
            {
                Debug.LogError("File does not exist:\n" + print.PDFFilePath);
                OperationSucceeded = false;

                Scene3ManagerScript.MessageText.text = "PDF could not be found";
                Scene3ManagerScript.EnableMessagePanel();

                yield break;
            }

            if (Path.GetExtension(print.PDFFilePath).ToLower() != ".pdf")
            {
                Debug.LogError("Not a PDF:\n" + print.PDFFilePath);
                OperationSucceeded = false;

                Scene3ManagerScript.MessageText.text = "File is of incorrect extension";
                Scene3ManagerScript.EnableMessagePanel();

                yield break;
            }

            FileInfo fileInfo = new FileInfo(print.PDFFilePath);

            if (fileInfo.Length == 0)
            {
                Debug.LogError("PDF is empty:\n" + print.PDFFilePath);
                OperationSucceeded = false;

                Scene3ManagerScript.MessageText.text = "PDF is empty";
                Scene3ManagerScript.EnableMessagePanel();

                yield break;
            }

            byte[] header = new byte[5];

            using (FileStream stream = File.OpenRead(print.PDFFilePath))
            {
                stream.Read(header, 0, 5);
            }

            string headerString = System.Text.Encoding.ASCII.GetString(header);

            if (headerString != "%PDF-")
            {
                Debug.LogError("Invalid PDF:\n" + print.PDFFilePath);
                OperationSucceeded = false;

                Scene3ManagerScript.MessageText.text = "Invalid PDF.";
                Scene3ManagerScript.EnableMessagePanel();

                yield break;
            }
        }

        Debug.Log("All PDFs validated successfully.");
    }

    IEnumerator UploadAllPrints()
    {
        Debug.Log("1");
        foreach (PrintDetails print in PrintManagerScript.PrintDetailsList)
        {
            Debug.Log("2");
            if (!File.Exists(print.PDFFilePath))
            {
                Debug.Log("3");
                Debug.LogError("File not found:\n" + print.PDFFilePath);

                Debug.Log("4");
                OperationSucceeded = false;

                Debug.Log("5");
                Scene3ManagerScript.MessageText.text = "PDF could not be found";

                Debug.Log("6");
                Scene3ManagerScript.EnableMessagePanel();

                Debug.Log("7");
                Scene3ManagerScript.DisableAllPanels();

                Debug.Log("8");
                Scene3ManagerScript.EnablePrintBillsManager();

                Debug.Log("9");
                yield break;
            }

            Debug.Log("10");
            byte[] pdfBytes = File.ReadAllBytes(print.PDFFilePath);

            Debug.Log("11");
            WWWForm form = new WWWForm();

            //--------------------------------------------------
            // PDF
            //--------------------------------------------------

            Debug.Log("12");
            form.AddBinaryData(
                "PDF",
                pdfBytes,
                Path.GetFileName(print.PDFFilePath),
                "application/pdf"
            );

            //--------------------------------------------------
            // Print information
            //--------------------------------------------------

            Debug.Log("13");
            form.AddField("PrintNumber", print.PrintNumber);

            form.AddField("PDFName", Path.GetFileName(print.PDFFilePath));
            form.AddField("PrintFilePath", print.PDFFilePath);

            form.AddField("PageSelection", (int)print.PageSelection);
            form.AddField("SideSelection", (int)print.SideSelection);
            form.AddField("Collated", (int)print.CollatedOrNot);
            form.AddField("Orientation", (int)print.Orientation);
            form.AddField("Margin", (int)print.Margin);
            form.AddField("PagesPerSheet", (int)print.PagesPerSheet);
            form.AddField("Color", (int)print.Color);

            form.AddField("Copies", print.Copies);

            form.AddField("EffectiveSheets", print.EffectiveSheets);

            Debug.Log("14");
            //Looks weird, but this is just to avoid issues the bill amount being a float value. 
            form.AddField("BillAmount", print.BillAmount.ToString(CultureInfo.InvariantCulture));

            Debug.Log("15");
            using (UnityWebRequest request = UnityWebRequest.Post(
                                                                    "http://localhost:3000/upload-print", form)
                                                                 )
            {
                Debug.Log("16");
                yield return request.SendWebRequest();

                Debug.Log("17");
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("18");
                    Debug.LogError(request.error);

                    if (!string.IsNullOrEmpty(request.downloadHandler.text))
                    {
                        Debug.Log("19");
                        Debug.LogError(request.downloadHandler.text);

                        Scene3ManagerScript.MessageText.text = "An error occured, try restarting the app.";
                        Scene3ManagerScript.EnableMessagePanel();

                        Debug.Log("29");
                        Scene3ManagerScript.DisableAllPanels();
                        Scene3ManagerScript.EnablePrintBillsManager();
                    }

                    Debug.Log("20");
                    OperationSucceeded = false;

                    Debug.Log("21");
                    Scene3ManagerScript.MessageText.text = "An error occured, try restarting the app.";

                    Debug.Log("22");
                    Scene3ManagerScript.EnableMessagePanel();

                    Debug.Log("23");
                    Scene3ManagerScript.DisableAllPanels();

                    Debug.Log("24");
                    Scene3ManagerScript.EnablePrintBillsManager();

                    Debug.Log("25");
                    yield break;
                }

                Debug.Log("26");
                UploadResponse response = JsonUtility.FromJson<UploadResponse>(request.downloadHandler.text);

                if (!response.Success)
                {
                    Debug.Log("27");
                    Debug.LogError(response.Message);
                    OperationSucceeded = false;

                    Debug.Log("28");
                    Scene3ManagerScript.MessageText.text = "An error occured, try restarting the app.";
                    Scene3ManagerScript.EnableMessagePanel();

                    Debug.Log("29");
                    Scene3ManagerScript.DisableAllPanels();
                    Scene3ManagerScript.EnablePrintBillsManager();

                    yield break;
                }

                //--------------------------------------------------
                // Save PrintID for CreateOrder()
                //--------------------------------------------------
                Debug.Log("31");
                CurrentOrderRequest.PrintIDs.Add(response.PrintID);

                //--------------------------------------------------
                // Optional: update local data
                //--------------------------------------------------
                Debug.Log("32");
                print.PrintID   = response.PrintID;     //Added by me.
                print.Pages     = response.TotalPages;
                print.FileSize  = response.FileSize;
                print.PDFName   = response.PDFName;

                Debug.Log("33");
                Debug.Log($"Uploaded Print #{print.PrintNumber} (ID = {response.PrintID})");
            }
        }
        Debug.Log("34");
        Debug.Log("All prints uploaded successfully.");
    }

    IEnumerator SubmitOrder()
    {
        string json = JsonUtility.ToJson(CurrentOrderRequest);

        Debug.Log(json);

        byte[] raw = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(
                                                        "http://localhost:3000/create-order",
                                                        "POST"
                                                     );

        request.uploadHandler   = new UploadHandlerRaw(raw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader(
                                    "Content-Type",
                                    "application/json"
                                );

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);

            if (!string.IsNullOrEmpty(request.downloadHandler.text))
            {
                Debug.LogError(request.downloadHandler.text);
            }

            OperationSucceeded = false;

            Scene3ManagerScript.MessageText.text = "An error occured, try restarting the app.";
            Scene3ManagerScript.EnableMessagePanel();

            yield break;
        }

        CurrentOrderResponse = JsonUtility.FromJson<CurrentOrderResponse>(
                                                                            request.downloadHandler.text
                                                                         );

        if (!CurrentOrderResponse.Success)
        {
            Debug.LogError(CurrentOrderResponse.Message);
            OperationSucceeded = false;

            Scene3ManagerScript.MessageText.text = "An error occured, try restarting the app.";
            Scene3ManagerScript.EnableMessagePanel();

            yield break;
        }

        Debug.Log(
                    "Order Created \n" +
                    "\nOrder ID = " + CurrentOrderResponse.OrderID +
                    "\nOTP = " + CurrentOrderResponse.OTP +
                    "\nApproximate Time = " + CurrentOrderResponse.ApproximateTimeLeft
                 );
    }









    //==============================================================
    //Order Downloading stuff -
    //==============================================================

    // Download all User Orders.
    public void GetOrdersForCurrentUser()
    {
        StartCoroutine(GetOrders());
    }
    IEnumerator GetOrders()
    {
        Debug.Log("Downloading Orders...");

        // Clear previously downloaded data.
        ConfirmedOrders.Orders.Clear();
        ConfirmedPrints.Prints.Clear();
        ConfirmedPrintSettings.PrintSettings.Clear();
        ConfirmedPrintBills.PrintBills.Clear();

        UnityWebRequest request = UnityWebRequest.Get
                                                  (
                                                        "http://localhost:3000/orders/user/id/"
                                                        + Script1.UserID
                                                  );

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);

            Scene3ManagerScript.MessageText.text = "An error occured, try restarting the app.";
            Scene3ManagerScript.EnableMessagePanel();

            yield break;
        }

        Debug.Log(request.downloadHandler.text);

        OrderSummaryWrapper Response = JsonUtility.FromJson<OrderSummaryWrapper>
                                                   (
                                                        request.downloadHandler.text
                                                   );

        if (Response == null || !Response.Success)
        {
            Debug.LogError("Failed to download orders.");

            Scene3ManagerScript.MessageText.text = "Failed to download orders, try restarting the app.";
            Scene3ManagerScript.EnableMessagePanel();

            yield break;
        }

        ConfirmedOrders.Orders.AddRange(Response.Orders);

        Debug.Log
        (
            "Downloaded "
            + ConfirmedOrders.Orders.Count
            + " Orders."
        );

        yield return StartCoroutine(GetPrints());

        Debug.Log("Finished downloading Orders.");
    }
    public void GetRecentOrders()
    {
        StartCoroutine(GetRecentOrdersFromServer());
    }
    IEnumerator GetRecentOrdersFromServer()
    {
        ConfirmedOrders.Orders.Clear();
        ConfirmedPrints.Prints.Clear();
        ConfirmedPrintSettings.PrintSettings.Clear();
        ConfirmedPrintBills.PrintBills.Clear();

        UnityWebRequest request = UnityWebRequest.Get
                                                  (
                                                        $"http://localhost:3000/get-active-orders/{Script1.UserID}"
                                                  );

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);

            Scene3ManagerScript.MessageText.text = "An error occured, try restarting the app.";
            Scene3ManagerScript.EnableMessagePanel();

            Scene3ManagerScript.DisableAllPanels();
            Scene3ManagerScript.EnableHomePage();

            yield break;
        }

        Debug.Log(request.downloadHandler.text);

        OrderSummaryWrapper Response = JsonUtility.FromJson<OrderSummaryWrapper>
                                                   (
                                                        request.downloadHandler.text
                                                   );

        if (Response == null || !Response.Success)
        {
            Debug.LogError("Failed to download orders.");

            Scene3ManagerScript.MessageText.text = "Failed to download orders, try restarting the app.";
            Scene3ManagerScript.EnableMessagePanel();

            Scene3ManagerScript.DisableAllPanels();
            Scene3ManagerScript.EnableHomePage();

            yield break;
        }

        ConfirmedOrders.Orders.AddRange(Response.Orders);

        Debug.Log
        (
            "Downloaded "
            + ConfirmedOrders.Orders.Count
            + " Orders."
        );

        yield return StartCoroutine(GetPrints());

        Debug.Log("Finished downloading recent Orders.");
    }
    public void GetCollectedOrders()
    {
        StartCoroutine(GetCollectedOrdersFromServer());
    }
    IEnumerator GetCollectedOrdersFromServer()
    {
        ConfirmedOrders.Orders.Clear();
        ConfirmedPrints.Prints.Clear();
        ConfirmedPrintSettings.PrintSettings.Clear();
        ConfirmedPrintBills.PrintBills.Clear();

        UnityWebRequest request = UnityWebRequest.Get
                                                  (
                                                        $"http://localhost:3000/get-collected-orders/{Script1.UserID}"
                                                  );

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);

            Scene3ManagerScript.MessageText.text = "An error occured, try restarting the app.";
            Scene3ManagerScript.EnableMessagePanel();

            Scene3ManagerScript.DisableAllPanels();
            Scene3ManagerScript.EnableHomePage();

            yield break;
        }

        Debug.Log(request.downloadHandler.text);

        OrderSummaryWrapper Response = JsonUtility.FromJson<OrderSummaryWrapper>
                                                   (
                                                        request.downloadHandler.text
                                                   );

        if (Response == null || !Response.Success)
        {
            Debug.LogError("Failed to download orders.");

            Scene3ManagerScript.MessageText.text = "Failed to download orders, try restarting the app.";
            Scene3ManagerScript.EnableMessagePanel();

            Scene3ManagerScript.DisableAllPanels();
            Scene3ManagerScript.EnableHomePage();

            yield break;
        }

        ConfirmedOrders.Orders.AddRange(Response.Orders);

        Debug.Log
        (
            "Downloaded "
            + ConfirmedOrders.Orders.Count
            + " Orders."
        );

        yield return StartCoroutine(GetPrints());

        Debug.Log("Finished downloading Collected Orders.");
    }





    //Used by above functions.
    IEnumerator GetPrints()
    {
        Debug.Log("Downloading Prints...");

        foreach (OrderSummary Order in ConfirmedOrders.Orders)
        {
            UnityWebRequest request = UnityWebRequest.Get
                                                      (
                                                            "http://localhost:3000/prints/order/"
                                                            + Order.OrderID
                                                      );

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);

                Scene3ManagerScript.MessageText.text = "An error occured, try restarting the app.";
                Scene3ManagerScript.EnableMessagePanel();

                continue;
            }

            Debug.Log(request.downloadHandler.text);

            PrintSummaryWrapper Response = JsonUtility.FromJson<PrintSummaryWrapper>
                                                       (
                                                            request.downloadHandler.text
                                                       );

            if (Response == null)
            {
                Scene3ManagerScript.MessageText.text = "An error occured, try restarting the app.";
                Scene3ManagerScript.EnableMessagePanel();

                continue;
            }

            if (Response.Prints != null)
            {
                ConfirmedPrints.Prints.AddRange(Response.Prints);
            }
        }

        Debug.Log
        (
            "Downloaded "
            + ConfirmedPrints.Prints.Count
            + " Prints."
        );

        yield return StartCoroutine(GetPrintSettings());

        Debug.Log("Finished downloading Prints.");
    }
    IEnumerator GetPrintSettings()
    {
        Debug.Log("Downloading Print Settings...");

        foreach (PrintSummary Print in ConfirmedPrints.Prints)
        {
            UnityWebRequest request = UnityWebRequest.Get
                                                      (
                                                            "http://localhost:3000/settings/print/"
                                                            + Print.PrintID
                                                      );

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);

                Scene3ManagerScript.MessageText.text = "An error occured, try restarting the app.";
                Scene3ManagerScript.EnableMessagePanel();

                continue;
            }

            Debug.Log(request.downloadHandler.text);

            PrintSettingsSummaryWrapper Response = JsonUtility.FromJson<PrintSettingsSummaryWrapper>
                                                               (
                                                                    request.downloadHandler.text
                                                               );

            if (Response == null)
            {
                Scene3ManagerScript.MessageText.text = "An error occured, try restarting the app.";
                Scene3ManagerScript.EnableMessagePanel();

                continue;
            }

            if (Response.PrintSettings != null)
            {
                ConfirmedPrintSettings.PrintSettings.AddRange(Response.PrintSettings);
            }
        }

        Debug.Log
        (
            "Downloaded "
            + ConfirmedPrintSettings.PrintSettings.Count
            + " Print Settings."
        );

        yield return StartCoroutine(GetPrintBills());
    }
    IEnumerator GetPrintBills()
    {
        Debug.Log("Downloading Print Bills...");

        foreach (PrintSummary Print in ConfirmedPrints.Prints)
        {
            UnityWebRequest request = UnityWebRequest.Get
                                                      (
                                                            "http://localhost:3000/bill/print/"
                                                            + Print.PrintID
                                                      );
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);

                Scene3ManagerScript.MessageText.text = "An error occured, try restarting the app.";
                Scene3ManagerScript.EnableMessagePanel();

                continue;
            }

            Debug.Log(request.downloadHandler.text);

            PrintBillsSummaryWrapper Response = JsonUtility.FromJson<PrintBillsSummaryWrapper>
                                                            (
                                                                request.downloadHandler.text
                                                            );

            if (Response == null)
            {
                Scene3ManagerScript.MessageText.text = "An error occured, try restarting the app.";
                Scene3ManagerScript.EnableMessagePanel();

                continue;
            }

            if (Response.PrintBills != null)
            {
                ConfirmedPrintBills.PrintBills.AddRange(Response.PrintBills);
            }
        }

        Debug.Log
        (
            "Downloaded "
            + ConfirmedPrintBills.PrintBills.Count
            + " Print Bills."
        );

        BuildPrintDetails();

        Debug.Log("Finished downloading everything.");
    }

    //Converting all orders recieved from server into PrintDetails type, for easy creation of OrderPanels.
    void BuildPrintDetails()
    {
        foreach (OrderSummary Order in ConfirmedOrders.Orders)
        {
            Order.Prints.Clear();

            foreach (PrintSummary Print in ConfirmedPrints.Prints)
            {
                if (Print.OrderID != Order.OrderID)
                    continue;

                PrintSettingsSummary Settings = null;

                foreach (PrintSettingsSummary Setting in ConfirmedPrintSettings.PrintSettings)
                {
                    if (Setting.PrintID == Print.PrintID)
                    {
                        Settings = Setting;
                        break;
                    }
                }

                PrintBillsSummary Bill = null;

                foreach (PrintBillsSummary BillInfo in ConfirmedPrintBills.PrintBills)
                {
                    if (BillInfo.PrintID == Print.PrintID)
                    {
                        Bill = BillInfo;
                        break;
                    }
                }

                if (Settings == null || Bill == null)
                {
                    continue;
                }

                PrintDetails Details = new PrintDetails();

                Details.PrintNumber = Print.PrintNumber;
                Details.PrintID = Print.PrintID;

                Details.PDFName = Print.PDFName;

                Details.Pages = Print.TotalPages;

                Details.Copies = Settings.Copies;

                Details.PageSelection =
                    (PageSelectionEnum)Settings.PageSelection;

                Details.SideSelection =
                    (SideSelectionEnum)Settings.SideSelection;

                Details.CollatedOrNot =
                    (CollatedOrNotEnum)Settings.Collated;

                Details.Orientation =
                    (OrientationEnum)Settings.Orientation;

                Details.Margin =
                    (MarginEnum)Settings.Margin;

                Details.PagesPerSheet =
                    (PagesPerSheetEnum)Settings.PagesPerSheet;

                Details.Color =
                    (ColorEnum)Settings.Color;

                Details.EffectiveSheets =
                    Bill.EffectiveSheets;

                Details.BillAmount =
                    Bill.BillAmount;

                Order.Prints.Add(Details);

                Details.PrintOrderDetails();
            }
        }

        Debug.Log("Successfully built PrintDetails for every order.");

        OrderStatusManagerScript.CreateOrderStatusPanels();

        Scene3ManagerScript.OrdersPage();
    }








    









    //Gettings the name of the stationery to which the order is assigned.
    [System.Serializable]
    public class GetStationeryNameRequest
    {
        public int OrderID;
    }

    [System.Serializable]
    public class GetStationeryNameResponse
    {
        public bool Success;
        public string Message;
        public string StationeryName;
    }

    public void GetStationeryNameFromOrderID(int OrderID, OrderStatus OrderStatusScript)
    {
        StartCoroutine(GetStationeryNameFromServer(OrderID, OrderStatusScript));
    }
    IEnumerator GetStationeryNameFromServer(int OrderID, OrderStatus OrderStatusScript)
    {
        GetStationeryNameRequest requestData = new GetStationeryNameRequest
        {
            OrderID = OrderID
        };

        string json = JsonUtility.ToJson(requestData);

        using (UnityWebRequest request = new UnityWebRequest("http://localhost:3000/order/getStationeryName", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);

                Scene3ManagerScript.MessageText.text = "An error occured, try restarting the app.";
                Scene3ManagerScript.EnableMessagePanel();

                yield break;
            }

            GetStationeryNameResponse response =
                JsonUtility.FromJson<GetStationeryNameResponse>(request.downloadHandler.text);

            if (response.Success)
            {
                Debug.Log("Stationery Name: " + response.StationeryName);

                OrderStatusScript.StationeryNameText.text = response.StationeryName;
            }
            else
            {
                Debug.LogError(response.Message);

                Scene3ManagerScript.MessageText.text = "An error occured, try restarting the app.";
                Scene3ManagerScript.EnableMessagePanel();
            }
        }
    }










    //Calculate bill
    public void CalculateBill(
                                BillRequest request,
                                string pdfPath,
                                System.Action<BillResponse> callback
                             )
    {
        StartCoroutine(Coroutine_CalculateBill(
                                                    request,
                                                    pdfPath,
                                                    callback
                                              ));
    }

    IEnumerator Coroutine_CalculateBill(
                                            BillRequest request,
                                            string pdfPath,
                                            System.Action<BillResponse> callback
                                       )
    {
        if (!File.Exists(pdfPath))
        {
            callback(new BillResponse
            {
                Success = false,
                Message = "PDF file not found."
            });

            Scene3ManagerScript.MessageText.text = "PDF could not be found, make sure all selected PDFs still exist.";
            Scene3ManagerScript.EnableMessagePanel();

            Scene3ManagerScript.DisableAllPanels();
            Scene3ManagerScript.EnablePrintManager();

            yield break;
        }

        byte[] pdfBytes = File.ReadAllBytes(pdfPath);

        WWWForm form = new WWWForm();

        //--------------------------------------------------
        // PDF
        //--------------------------------------------------

        form.AddBinaryData(
            "PDF",
            pdfBytes,
            Path.GetFileName(pdfPath),
            "application/pdf"
        );

        //--------------------------------------------------
        // Bill Request
        //--------------------------------------------------

        form.AddField("PagesPerSheet", request.PagesPerSheet);
        form.AddField("SideSelection", request.SideSelection);
        form.AddField("Color", request.Color);
        form.AddField("Copies", request.Copies);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(
                                                                    "http://localhost:3000/calculate-bill",
                                                                    form
                                                                ))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                BillResponse errorResponse;

                try
                {
                    errorResponse = JsonUtility.FromJson<BillResponse>(
                        webRequest.downloadHandler.text
                    );
                }
                catch
                {
                    errorResponse = new BillResponse
                    {
                        Success = false,
                        Message = webRequest.error
                    };
                }

                Scene3ManagerScript.MessageText.text = "An error ocurred, try restarting the app.";
                Scene3ManagerScript.EnableMessagePanel();

                Scene3ManagerScript.DisableAllPanels();
                Scene3ManagerScript.EnablePrintManager();

                callback(errorResponse);
                yield break;
            }

            BillResponse response = JsonUtility.FromJson<BillResponse>( webRequest.downloadHandler.text );

            callback(response);
        }
    }












    //Used for displaying the names of all registered stationeries.
    public GetStationeriesResponse StationeryResponse;
    public void GetStationeries()
    {
        StartCoroutine(Coroutine_GetStationeries());
    }
    IEnumerator Coroutine_GetStationeries()
    {
        UnityWebRequest request = new UnityWebRequest(
                                                        "http://localhost:3000/stationeries",
                                                        "GET"
                                                     );

        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);

            Scene3ManagerScript.MessageText.text = "An error occured, try restarting the app.";
            Scene3ManagerScript.EnableMessagePanel();

            Scene3ManagerScript.DisableAllPanels();
            Scene3ManagerScript.EnablePrintBillsManager();

            yield break;
        }

        Debug.Log(request.downloadHandler.text);

        StationeryResponse =
            JsonUtility.FromJson<GetStationeriesResponse>(
                                                             request.downloadHandler.text
                                                         );

        if (!StationeryResponse.Success)
        {
            Debug.Log("Failed to fetch stationeries.");

            Scene3ManagerScript.MessageText.text = "Failed to fetch stationeries, try restarting the app.";
            Scene3ManagerScript.EnableMessagePanel();

            Scene3ManagerScript.DisableAllPanels();
            Scene3ManagerScript.EnablePrintBillsManager();

            yield break;
        }

        if (StationeryResponse.Stationeries == null
            ||
            StationeryResponse.Stationeries.Length == 0)
        {
            Debug.Log("No stationeries found.");

            Scene3ManagerScript.MessageText.text = "No stationeries found, try restarting the app.";
            Scene3ManagerScript.EnableMessagePanel();

            Scene3ManagerScript.DisableAllPanels();
            Scene3ManagerScript.EnablePrintBillsManager();

            yield break;
        }

        PopulateDropdown();

        Scene3ManagerScript.DisableAllPanels();
        Scene3ManagerScript.EnableStationeryPreferencePanel();
    }
    void PopulateDropdown()
    {
        TMP_Dropdown dropdown = UpdateStationery2Script.StationeryNameDropDown;

        dropdown.ClearOptions();

        foreach (StationeryNameSummary stationery in StationeryResponse.Stationeries)
        {
            dropdown.options.Add(
                new TMP_Dropdown.OptionData(stationery.StationeryName)
            );
        }

        dropdown.RefreshShownValue();
    }





    //Used for updating stationery info through ProfileMenu -> Stationery.
    UpdateStationeryPreferenceRequest Request = new UpdateStationeryPreferenceRequest();
    public IEnumerator UpdateStationeryPreference(int StationeryID)
    {
        Debug.Log("Chosen StationeryID = " + StationeryID);
        Debug.Log("User ID = " + Script1.UserID);

        Request.StationeryPreference = StationeryID;
        Request.UserID = Script1.UserID;

        string json = JsonUtility.ToJson(Request);

        byte[] raw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(
                                                        "http://localhost:3000/update-stationeryPreference",
                                                        "POST"
                                                      );

        request.uploadHandler = new UploadHandlerRaw(raw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader(
                                    "Content-Type",
                                    "application/json"
                                );

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);

            Scene3ManagerScript.MessageText.text = "An error occured, try restarting the app.";
            Scene3ManagerScript.EnableMessagePanel();

            Scene3ManagerScript.DisableAllPanels();
            Scene3ManagerScript.EnableStationeryPreferencePanel();

            yield break;
        }

        UpdateStationeryPreferenceResponse response =
            JsonUtility.FromJson<UpdateStationeryPreferenceResponse>(
                                                                        request.downloadHandler.text
                                                                    );

        if (response.Success)
        {
            Debug.Log(response.Message);

            Script1.StationeryID = StationeryID;
            Script1.StationeryName = UpdateStationery2Script.StationeryName;

            Scene3ManagerScript.DisableAllPanels();

            PrintBillsManagerScript.UpdateStationeryNameInUI();
            Scene3ManagerScript.EnablePrintBillsManager();
        }
        else
        {
            Debug.LogError(response.Message);

            Scene3ManagerScript.MessageText.text = "An error occured, try restarting the app.";
            Scene3ManagerScript.EnableMessagePanel();

            Scene3ManagerScript.DisableInvisiblePanel();
        }
    }
}