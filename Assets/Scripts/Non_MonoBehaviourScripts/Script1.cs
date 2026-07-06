//Script0

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Script1
{
    public static int    UserID;
    public static string UserActualName;
    public static string UserName;
    public static string UserEmail;

    public static string PhoneNumber;
    public static string UPILinkedPhoneNumber;

    public static string Password;

    public static int StationeryID;
    public static string StationeryName;


    public static List<ConfirmedOrder> OrdersConfirmed = new List<ConfirmedOrder>();


    public static void SetUserInfo(UserInfo UserInfoObject)
    {
        if(UserInfoObject != null)
        {
            UserID                  = UserInfoObject.UserID;
            UserActualName          = UserInfoObject.Name;
            UserName                = UserInfoObject.Username;
            UserEmail               = UserInfoObject.Email;
            PhoneNumber             = UserInfoObject.PhoneNumber;
            UPILinkedPhoneNumber    = UserInfoObject.UPINumber;
            Password                = UserInfoObject.Password;
            StationeryID            = UserInfoObject.StationeryID;
        }
        else
        {
            UserID                  = -1;
            UserActualName          = null;
            UserName                = null;
            UserEmail               = null;
            PhoneNumber             = null;
            UPILinkedPhoneNumber    = null;
            Password                = null;
            StationeryID            = -1;
            StationeryName          = null;
        }
    }


}