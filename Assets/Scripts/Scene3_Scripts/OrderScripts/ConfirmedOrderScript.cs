//Script13

using System.Collections.Generic;

[System.Serializable]
public class ConfirmedOrder
{
    public int OrderID;

    public int OTP;

    public float FinalAmount;

    public List<PrintDetails> Prints = new List<PrintDetails>();

    public int ApproximateTimeLeft;

    public string CreatedAt;

    public bool ReachedServer = false;
}