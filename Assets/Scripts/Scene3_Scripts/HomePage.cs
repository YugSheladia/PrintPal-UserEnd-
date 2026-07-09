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
        Username.text = "Hey " + Script1.UserName + " :)";  
    }

    public void ClickedCreateOrder()
    {
        Scene3ManagerScript.CreateOrderPage();
    } 

    public void ClickedRecentOrders()
    {
        Scene3ManagerScript.EnableInvisiblePanel();
        Scene3_BackendScript.GetOrdersForCurrentUser();
    }

    public void ClickedOrderHistory()
    {
        Scene3_BackendScript.GetArchivedOrdersForCurrentUser();
    }

    public void ClickedViewProfile()
    {
        SceneManager.LoadScene("Scene4_UpdateProfileInfo");
    } 

}
