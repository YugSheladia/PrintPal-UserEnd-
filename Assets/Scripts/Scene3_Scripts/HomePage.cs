//Script2

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HomePage : MonoBehaviour
{
    [Header("Scene3Manager stuff")]
    [SerializeField] Scene3Manager Scene3ManagerScript;

    [SerializeField] Scene3_Backend Scene3_BackendScript;

    [SerializeField] OrderStatusManager OrderStatusManagerScript;


    [Header("Other stuff")]
    [SerializeField] private TMP_Text Username;



    private void Start()
    {
        Username.text = "Hey " + Script1.UserName;

        Scene3ManagerScript.MessageText.text = "Hello " + Script1.UserName;
        Scene3ManagerScript.EnableMessagePanel();
    }

    public void ClickedCreateOrder()
    {
        Scene3ManagerScript.CreateOrderPage();
    } 

    public void ClickedRecentOrders()
    {
        Scene3ManagerScript.EnableInvisiblePanel();

        Scene3_BackendScript.GetRecentOrders();
    }

    public void ClickedOrderHistory()
    {
        Scene3ManagerScript.EnableInvisiblePanel();

        Scene3_BackendScript.GetCollectedOrders();
    }

    public void ClickedViewProfile()
    {
        SceneManager.LoadScene("Scene4_UpdateProfileInfo");
    } 

}
