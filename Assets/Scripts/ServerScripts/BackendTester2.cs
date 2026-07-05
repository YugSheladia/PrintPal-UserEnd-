using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using System;
using NUnit.Framework.Constraints;

[System.Serializable]
public class OrderInfo
{
    public int OrderID;

    public string PDFName;
    public string NoOfPages;
    public string NoOfCopies;
    public string SideSelection;

    public void PrintOrderInfo()
    {
        Debug.Log("OrderID = " + OrderID +
            "\nPDF name = " + PDFName +
            "\nNo of pages = " + NoOfPages +
            "\nNo of copies = " + NoOfCopies +
            "\nSideSelection = " + SideSelection);
    }

    public OrderInfo()
    {

    }

    public OrderInfo( string PDFName, string NoOfPages ,string NoOfCopies, string SideSelection)
    {
        this.PDFName = PDFName;
        this.NoOfPages = NoOfPages;
        this.NoOfCopies = NoOfCopies;
        this.SideSelection = SideSelection;
    }
}

[System.Serializable]
public class OrderInfoRecieved
{
    public bool Success;
    public string Message;

    public int OrderID;
}

[System.Serializable]
public class OrderList
{
    public OrderInfo[] OrderInfoList;
}


public class BackendTester2 : MonoBehaviour
{
    [SerializeField] TMP_InputField PDFName_Input;
    [SerializeField] TMP_InputField NoOfPages_Input;
    [SerializeField] TMP_InputField NoOfCopies_Input;
    [SerializeField] TMP_InputField SideSelection_Input;

    OrderInfo CurrentOrderInfoObject;

    int OrderNo = 0;


    [SerializeField] TMP_InputField RequiredOrderInput;
    int RequiredOrderNo;


    public void ClickedSetOrdersButton()
    {
        string PDFName, NoOfPages, NoOfCopies, SideSelection;

        PDFName = PDFName_Input.text;
        NoOfPages = NoOfPages_Input.text;
        NoOfCopies = NoOfCopies_Input.text;
        SideSelection = SideSelection_Input.text;

        OrderInfo OrderInfoObject = new OrderInfo(PDFName, NoOfPages, NoOfCopies, SideSelection);
        CurrentOrderInfoObject = OrderInfoObject;

        StartCoroutine(SetOrderInfoInServer());

        print("\nOrder submitted." + 
              "\nOrder Number = " + OrderNo++);
    }
    IEnumerator SetOrderInfoInServer()
    {
        string OrderInfoObjectJSON = JsonUtility.ToJson(CurrentOrderInfoObject);

        byte[] OrderInfoObjectRaw = System.Text.Encoding.UTF8.GetBytes(OrderInfoObjectJSON);

        UnityWebRequest request = new UnityWebRequest(
                                                        "http://localhost:3000/create-order",
                                                        "POST"
                                                     );

        request.uploadHandler = new UploadHandlerRaw(OrderInfoObjectRaw);

        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader(
                                    "Content-Type",
                                    "application/json"
                                );

        yield return request.SendWebRequest();

        if(request.result == UnityWebRequest.Result.Success)
        {
            string ServerResponseJSON = request.downloadHandler.text;

            Debug.Log("Raw Server Response : \n" + ServerResponseJSON);

            OrderInfoRecieved OrderInfoRecievedObject = JsonUtility.FromJson<OrderInfoRecieved>(ServerResponseJSON);

            CurrentOrderInfoObject.OrderID = OrderInfoRecievedObject.OrderID;

            CurrentOrderInfoObject.PrintOrderInfo();
        }
        else
        {
            Debug.LogError(request.error);
        }
    }



    public void ClickedGetOrdersButton()
    {
        StartCoroutine(GetOrderInfoFromServer());
    }
    IEnumerator GetOrderInfoFromServer()
    {
        UnityWebRequest request = UnityWebRequest.Get("http://localhost:3000/OrderInfo");

        yield return request.SendWebRequest();

        if(request.result == UnityWebRequest.Result.Success)
        {
            string ServerResponseJSON = request.downloadHandler.text;

            Debug.Log("Raw Server response = \n" + ServerResponseJSON);

            OrderList ServerResponseObject = JsonUtility.FromJson<OrderList>(ServerResponseJSON);
            
            if(ServerResponseObject.OrderInfoList != null)
            {
                Debug.Log("Orders recieved\n");

                foreach (OrderInfo OrderInfoObject in ServerResponseObject.OrderInfoList)
                {
                    OrderInfoObject.PrintOrderInfo();
                }
            }
            else
            {
                Debug.LogError("No Orders found");
            }
        }
        else
        {
            Debug.LogError(request.error);
        }
    }



    public void ClickedGetSpecificOrder()
    {
        RequiredOrderNo = int.Parse(RequiredOrderInput.text);

        StartCoroutine(GetSpecificOrderInfoFromServer());
    }
    IEnumerator GetSpecificOrderInfoFromServer()
    {
        UnityWebRequest request = UnityWebRequest.Get("http://localhost:3000/order/"+RequiredOrderNo);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string ServerResponseJSON = request.downloadHandler.text;

            Debug.Log("Order No " + RequiredOrderNo + " found... " +
                      "\nRaw Order Info : \n" + ServerResponseJSON);

            OrderInfo OrderInfoObject = JsonUtility.FromJson<OrderInfo>(ServerResponseJSON);

            if (OrderInfoObject != null)
            {
                OrderInfoObject.PrintOrderInfo();
            }
            else
            {
                Debug.LogError("Order was null.");
            }
        }
        else
        {
            Debug.Log("Order No " + RequiredOrderNo + " Not found.");
        }
    }
}
