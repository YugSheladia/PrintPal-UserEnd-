using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;



//==================================
// CREATE ORDER CLASSES
//==================================
[System.Serializable]
public class CreateOrderRequest
{
    public int UserID;
    public float FinalAmount;
    public int StationeryID;

    public List<PrintDTO> Prints;
}

[System.Serializable]
public class PrintDTO
{
    public int PrintNumber;
    public int PrintID;

    public string PDFName;
    public string PrintFilePath;

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
public class CreateOrderResponse
{
    public bool Success;

    public int OrderID;
    public int OTP;

    public int ApproximateTimeLeft;

    public PrintSummary[] Prints;

    public string Message;
}




//=======================================================
//Recieve all possible info about an issued order.
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
    public string PrintFilePath;

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


// Stationery TABLE
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



[System.Serializable]
public class UploadResponse
{
    public bool Success;
    public int PrintID;
    public string Message;
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
    


    //Creating an Order Request.
    CreateOrderRequest CurrentOrderRequest;
    CreateOrderResponse CreateOrderResponse;


    //Creating Wrapper objects.
    public OrderSummaryWrapper ConfirmedOrders = new();
    public PrintSummaryWrapper ConfirmedPrints = new();
    public PrintSettingsSummaryWrapper ConfirmedPrintSettings = new();
    public PrintBillsSummaryWrapper ConfirmedPrintBills = new();



    // CrateOrder function initialized Order Request, and then calls SubmitPrintSettingsToServer.
    public void CreateOrder()
    {
        CreateOrderRequest Request = new();

        Debug.Log("Inside Create Order, UserID = " + Script1.UserID);
        Request.UserID = Script1.UserID;

        Request.FinalAmount = PrintBillsManagerScript.FinalBillAmount;

        Debug.Log("Inside Create Order, StationeryID = " + Script1.StationeryID);
        Request.StationeryID = Script1.StationeryID;

        Request.Prints = new List<PrintDTO>();

        foreach (PrintDetails Print in PrintManagerScript.PrintDetailsList)
        {
            PrintDTO dto = new PrintDTO();

            dto.PrintNumber = Print.PrintNumber;
            //dto.PrintID = Print.PrintID;

            dto.PDFName = Print.PDFName;
            dto.PrintFilePath = Print.PrintFilePath;

            dto.Pages = Print.Pages;

            dto.Copies = Print.Copies;

            dto.PageSelection = (int)Print.PageSelection;
            dto.SideSelection = (int)Print.SideSelection;
            dto.Collated = (int)Print.CollatedOrNot;
            dto.Orientation = (int)Print.Orientation;
            dto.Margin = (int)Print.Margin;
            dto.PagesPerSheet = (int)Print.PagesPerSheet;
            dto.Color = (int)Print.Color;

            dto.EffectiveSheets = Print.EffectiveSheets;
            dto.BillAmount = Print.BillAmount;

            Request.Prints.Add(dto);
        }

        CurrentOrderRequest = Request;

        StartCoroutine(SubmitPrintSettingsToServer());
    }
    IEnumerator SubmitPrintSettingsToServer()
    {
        string json = JsonUtility.ToJson(CurrentOrderRequest);

        Debug.Log("JSON = " + json);

        byte[] raw = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(
                                                        "http://localhost:3000/create-order",
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
            Debug.LogError(request.downloadHandler.text);
            yield break;
        }

        CreateOrderResponse Response = new();
        
        Response = JsonUtility.FromJson<CreateOrderResponse>
                                                   (
                                                        request.downloadHandler.text
                                                   );

        if (!Response.Success)
        {
            Debug.LogError(Response.Message);
            yield break;
        }



        Debug.Log(
                    "Order Created\n\n" +
                    "Order ID = " + Response.OrderID +
                    "\nOTP = " + Response.OTP +
                    "\nApproximate Time = " + Response.ApproximateTimeLeft
                 );

        foreach(PrintSummary Print in Response.Prints)
        {
            Debug.Log(  "OrderID              = " + Print.OrderID         + 
                        "\nPrintID            = " + Print.PrintID         + 
                        "\nPDF Name           = " + Print.PDFName         + 
                        "\nPrint File Path    = " + Print.PrintFilePath   + 
                        "\nTotal Pages        = " + Print.TotalPages);
        }


        StartCoroutine(UploadAllPDFs(Response));
    }

    //Uploading PDFs
    IEnumerator UploadAllPDFs(CreateOrderResponse Response)
    {
        foreach (PrintSummary Print in Response.Prints)
        {
            yield return StartCoroutine(UploadPDF(Print.PrintID, Print.PrintFilePath));
        }

        PrintBillsManagerScript.ConfirmOrders();
    }
    IEnumerator UploadPDF(int PrintID, string PrintFilePath)
    {
        if(!File.Exists(PrintFilePath))
        {
            Debug.LogError("File not found = " + PrintFilePath);
            yield break;
        }    

        byte[] pdfBytes = File.ReadAllBytes(PrintFilePath);

        WWWForm form = new WWWForm();

        form.AddField("PrintID", PrintID);

        Debug.Log("Uploading file: " + Path.GetFileName(PrintFilePath));
        Debug.Log("Extension: " + Path.GetExtension(PrintFilePath));

        form.AddBinaryData
        (
            "PDF",
            pdfBytes,
            Path.GetFileName(PrintFilePath),
            "application/pdf"
        );

        using (UnityWebRequest request =
               UnityWebRequest.Post("http://localhost:3000/upload/print", form))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                Debug.LogError(request.downloadHandler.text);
                yield break;
            }

            UploadResponse response =
                JsonUtility.FromJson<UploadResponse>(request.downloadHandler.text);

            if (!response.Success)
            {
                Debug.LogError("Upload failed.");
                yield break;
            }

            Debug.Log("Uploaded PDF for PrintID = " + PrintID);
        }
    }
   






    // Download all Unarchived User Orders.
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

    // Downlaod All Archived User Orders.
    public void GetArchivedOrdersForCurrentUser()
    {
        StartCoroutine(GetArchivedOrders());
    }
    IEnumerator GetArchivedOrders()
    {
        Debug.Log("Downloading Orders...");

        // Clear previously downloaded data.
        ConfirmedOrders.Orders.Clear();
        ConfirmedPrints.Prints.Clear();
        ConfirmedPrintSettings.PrintSettings.Clear();
        ConfirmedPrintBills.PrintBills.Clear();

        UnityWebRequest request = UnityWebRequest.Get
                                                  (
                                                        $"http://localhost:3000/userOrders/archived?UserID={Script1.UserID}"
                                                  );

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
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

        Debug.Log("Finished downloading Archived Orders.");
    }


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
                continue;
            }

            Debug.Log(request.downloadHandler.text);

            PrintSummaryWrapper Response = JsonUtility.FromJson<PrintSummaryWrapper>
                                                       (
                                                            request.downloadHandler.text
                                                       );

            if (Response == null)
            {
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
                continue;
            }

            Debug.Log(request.downloadHandler.text);

            PrintSettingsSummaryWrapper Response = JsonUtility.FromJson<PrintSettingsSummaryWrapper>
                                                               (
                                                                    request.downloadHandler.text
                                                               );

            if (Response == null)
            {
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
                continue;
            }

            Debug.Log(request.downloadHandler.text);

            PrintBillsSummaryWrapper Response = JsonUtility.FromJson<PrintBillsSummaryWrapper>
                                                            (
                                                                request.downloadHandler.text
                                                            );

            if (Response == null)
            {
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
            }
        }
    }







    //Setting Orders as Archived / Unarchived
    [System.Serializable]
    public class OrderIDRequest
    {
        public int OrderID;
    }

    public void SetOrderAsArchived(int OrderID)
    {
        StartCoroutine(Coroutine_SetOrderAsArchived(OrderID));
    }
    IEnumerator Coroutine_SetOrderAsArchived(int OrderID)
    {
        OrderIDRequest requestBody = new();
        requestBody.OrderID = OrderID;

        string json = JsonUtility.ToJson(requestBody);

        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(
                                                            "http://localhost:3000/order/user-archive",
                                                            "POST"
                                                      );

        request.uploadHandler = new UploadHandlerRaw(body);

        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader(
                                    "Content-Type",
                                    "application/json"
                                );

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Order marked as Archived.");
        }
        else
        {
            Debug.LogError(request.error);
        }
    }

    public void SetOrderAsUnarchived(int OrderID)
    {
        StartCoroutine(Coroutine_SetOrderAsUnarchived(OrderID));
    }
    IEnumerator Coroutine_SetOrderAsUnarchived(int OrderID)
    {
        OrderIDRequest requestBody = new();
        requestBody.OrderID = OrderID;

        string json = JsonUtility.ToJson(requestBody);

        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(
                                                            "http://localhost:3000/order/user-unarchive",
                                                            "POST"
                                                      );

        request.uploadHandler = new UploadHandlerRaw(body);

        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader(
                                    "Content-Type",
                                    "application/json"
                                );

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Order marked as Unarchived.");
        }
        else
        {
            Debug.LogError(request.error);
        }
    }
}