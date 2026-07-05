using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/*
Node.js is a program that allows JavaScript to run as a backend application.
Express is a library that makes building servers in Node.js much easier.
*/

[System.Serializable]
public class OrderDataSetByUser
{
    // Info passed on from server later.
    public int OrderID = 0;

    // Info chosen by the user.
    public string customerName;
    public int copies;
    public int pages;

    public void PrintDetails()
    {
        Debug.Log(
            "Inside OrderDataSetByUser :" +
            "\nCustomer Name = " + customerName +
            "\n| Copies = " + copies +
            "\n| Pages = " + pages +
            "\n| OrderID = " + OrderID
        );
    }
}

[System.Serializable]
public class OrderDataSetByServer
{
    // Info decided by the server.
    public bool success;
    public string message;
    public int orderID;
    //public int queuePosition;

    public void PrintDetails()
    {
        Debug.Log(
            "Inside OrderDataSetByServer:" +
            "\nSuccess = " + success +
            "\n| Message = " + message +
            "\n| OrderID = " + orderID +
            "\n| Queue Position = " + "Not decided yet"//queuePosition
        );
    }
}

[System.Serializable]
public class OrdersResponse
{
    public OrderDataSetByUser[] orders;
}



public class BackendTester : MonoBehaviour
{
    void Start()
    {
        // Network requests take time, so we use a Coroutine.
        StartCoroutine(TestBackend());
    }

    IEnumerator TestBackend()
    {
        OrderDataSetByUser order = new OrderDataSetByUser();

        order.customerName = "Yug";
        order.copies = 2;
        order.pages = 15;

        // Convert object -> JSON
        string json = JsonUtility.ToJson(order);

        // Convert JSON -> bytes
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(
                                                        "http://localhost:3000/create-order",
                                                        "POST"
                                                     );

        // Attach outgoing data.
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);

        // Store incoming response.
        request.downloadHandler = new DownloadHandlerBuffer();

        // Tell server we're sending JSON.
        request.SetRequestHeader(
                                "Content-Type",
                                "application/json"
                                );

        // Send request and wait.
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Raw Server Response:\n");
            Debug.Log(request.downloadHandler.text);

            string responseJson = request.downloadHandler.text;

            OrderDataSetByServer responseObject = JsonUtility.FromJson<OrderDataSetByServer>(responseJson);

            // Store server-generated Order ID inside user order.
            order.OrderID = responseObject.orderID;

            Debug.Log("----- SERVER RESPONSE -----");
            responseObject.PrintDetails();

            Debug.Log("----- USER ORDER -----");
            order.PrintDetails();
        }
        else
        {
            Debug.LogError(request.error);
        }
    }

    public void ClickedGetOrdersButton()
    {
        StartCoroutine(GetOrders());
    }

    IEnumerator GetOrders()
    {
        UnityWebRequest request = UnityWebRequest.Get("http://localhost:3000/OrderInfo");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;

            Debug.Log("Raw JSON:\n");
            Debug.Log(json);

            OrdersResponse response = JsonUtility.FromJson<OrdersResponse>(json);

            if (response.orders != null)
            {
                Debug.Log("----- ORDERS RECEIVED -----");

                foreach (OrderDataSetByUser order in response.orders)
                {
                    order.PrintDetails();
                }
            }
            else
            {
                Debug.Log("No orders found3.");
            }
        }
        else
        {
            Debug.LogError(request.error);
        }
    }
}