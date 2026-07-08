//Script11

using UnityEngine;

public class OrderStatusInfo : MonoBehaviour
{
    Scene3Manager Scene3ManagerScript;

    public void InitializeScene3ManagerScript(Scene3Manager Scene3ManagerScript)
    {
        this.Scene3ManagerScript = Scene3ManagerScript;
    }
    public void ClickedPreviousPageButton()
    {
        Scene3ManagerScript.OrdersPage();

        Destroy(gameObject);
    }
}
