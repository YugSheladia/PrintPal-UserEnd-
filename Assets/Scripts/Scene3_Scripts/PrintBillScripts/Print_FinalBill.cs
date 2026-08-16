//Script8

using TMPro;
using UnityEngine;

public class Print_FinalBill : MonoBehaviour
{
    [SerializeField] TMP_Text FinalPriceText;

    [SerializeField] TMP_Text OTPText;
    [SerializeField] TMP_Text OrderIDText;

    [SerializeField] TMP_Text OTPValueText;
    [SerializeField] TMP_Text OrderIDValueText;

    private float FinalBillAmount;

    public int OTP;
    public int OrderID;


    // Used for NEW orders
    public void SetFinalBillAmount(float FinalBillAmount)
    {
        this.FinalBillAmount = FinalBillAmount;
        FinalPriceText.text = "`" + FinalBillAmount.ToString("F2");

        OrderIDValueText.text   = "Will be generated after successful payament.";
        OTPValueText.text       = "Will be generated after successful payament.";
    } 

    // Used when recreating an EXISTING order with known OTP
    public void SetFinalBillAmount(float FinalBillAmount, int ExistingOrderID, int ExistingOTP)
    {
        this.FinalBillAmount = FinalBillAmount;

        OrderID     = ExistingOrderID;
        OTP         = ExistingOTP;

        SetDetailsInUI();
    } 


    private void SetDetailsInUI()
    {
        FinalPriceText.text = "`" + FinalBillAmount.ToString("F2");

        OrderIDText.text = "Order ID :";
        OTPText.text = "OTP :";

        OrderIDValueText.text    = OrderID.ToString();
        OTPValueText.text        = OTP.ToString();
    }
}