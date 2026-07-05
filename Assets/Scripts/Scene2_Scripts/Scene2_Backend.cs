using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


//Wrapper class for Phone Number Response from server
[System.Serializable]
public class PhoneNumberAvailabilityResponse
{
    public bool Success;
    public string Message;
}



[System.Serializable]
public class UserInfo
{
    public string PhoneNumber;
    public string Password;

    public string Name;
    public string UPINumber;
    public string Email;
    public string Username;

    public int StationeryID;

    //Info Set by server -
    public int UserID;

    public UserInfo() { }

    public void PrintUserInfo()
    {
        Debug.Log("UserID           = " + UserID        +
                  "\nPhone number   = " + PhoneNumber   +
                  "\nPassword       = " + Password      +
                  "\nName           = " + Name          +
                  "\nUPI Number     = " + UPINumber     +
                  "\nEmail          = " + Email         +
                  "\nUsername       = " + Username      +
                  "\nStationeryID   = " + StationeryID);
    }
}

[System.Serializable]
public class UserInfoResponse
{
    public bool Success;
    public UserInfo UserInfo;
} 

[System.Serializable]
public class UserInfoRecieved
{
    public bool     Success;
    public string   Message;
    public int      UserID;
}




[System.Serializable]
public class LoginVerificationResponseFromServer
{
    public bool Success;
    public string Message;

    public UserInfo User;
}



public class Scene2_Backend : MonoBehaviour
{
    [SerializeField] Scene2Manager Scene2ManagerScript;


    [SerializeField] Login LoginScript;



    //Check phone number availability
    public void CheckPhoneNumberAvailability()
    {
        StartCoroutine(SubmitPhoneNumberToServer());
    }
    IEnumerator SubmitPhoneNumberToServer()
    {
        string UserInfoObjectJSON = JsonUtility.ToJson(UserInfoObject);

        byte[] UserInfoObjectRaw = System.Text.Encoding.UTF8.GetBytes(UserInfoObjectJSON);

        UnityWebRequest request = new UnityWebRequest(
                                                        "http://localhost:3000/phoneNumberAvailability",
                                                        "POST"
                                                     );

        request.uploadHandler = new UploadHandlerRaw(UserInfoObjectRaw);

        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader(
                                    "Content-Type",
                                    "application/json"
                                );

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string ServerResponseJSON = request.downloadHandler.text;

            Debug.Log("Raw Server Response : \n" + ServerResponseJSON);

            PhoneNumberAvailabilityResponse ServerResponseObject = JsonUtility.FromJson<PhoneNumberAvailabilityResponse>(ServerResponseJSON);

            if(ServerResponseObject.Success == true)
            {
                Scene2ManagerScript.DisableAllPanels();
                Scene2ManagerScript.EnablePasswordPanel();
            }
            else
            {
                Debug.Log("Number already taken.");
            }
        }
        else
        {
            Debug.LogError(request.error);
        }
    }



    //Create Account page
    public UserInfo UserInfoObject;
    public IEnumerator SubmitUserInfoToServer()
    {
        string UserInfoObjectJSON = JsonUtility.ToJson(UserInfoObject);

        byte[] UserInfoObjectRaw = System.Text.Encoding.UTF8.GetBytes(UserInfoObjectJSON);

        UnityWebRequest request = new UnityWebRequest(
                                                        "http://localhost:3000/create-user",
                                                        "POST"
                                                     );

        request.uploadHandler = new UploadHandlerRaw(UserInfoObjectRaw);

        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader(
                                    "Content-Type",
                                    "application/json"
                                );

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string ServerResponseJSON = request.downloadHandler.text;
            Debug.Log("Raw Server Response : \n" + ServerResponseJSON);

            UserInfoRecieved UserInfoRecievedObject = JsonUtility.FromJson<UserInfoRecieved>(ServerResponseJSON);
            UserInfoObject.UserID = UserInfoRecievedObject.UserID;

            Script1.SetUserInfo(UserInfoObject);

            UserInfoObject.PrintUserInfo();
        }
        else
        {
            Debug.LogError(request.error);
        }
    }




    //Login Page
    LoginInfo LoginInfoObject;
    public void VerifyLoginInfo(LoginInfo LoginInfoObject)
    {
        this.LoginInfoObject = LoginInfoObject;

        StartCoroutine(SubmitLoginInfoToServer());
    }
    IEnumerator SubmitLoginInfoToServer()
    {
        string LoginInfoJSON = JsonUtility.ToJson(LoginInfoObject);

        byte[] LoginInfoRaw = System.Text.Encoding.UTF8.GetBytes(LoginInfoJSON);

        UnityWebRequest request = new UnityWebRequest(
                                                        "http://localhost:3000/userLogin", 
                                                        "POST"
                                                     );

        request.uploadHandler = new UploadHandlerRaw(LoginInfoRaw);

        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader(
                                "Content-Type",
                                "application/json"
                                );

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string ServerResponse = request.downloadHandler.text;
            Debug.Log("Raw server response =\n" + ServerResponse);

            LoginVerificationResponseFromServer ServerResponseObject = 
                JsonUtility.FromJson<LoginVerificationResponseFromServer>(ServerResponse);

            if(ServerResponseObject.Success == true)
            {
                //Load info about the user.

                Debug.Log("User info received");

                Script1.SetUserInfo(ServerResponseObject.User);

                LoginScript.LoadHomePage();

                //StartCoroutine(GetSpecificUserInfoFromServer());


                Debug.Log("Unity - Sucessfully logged in.");
            }
            else
            {
                Debug.Log("Unity - Incorrect Login information.");
            }
        }
        else
        {
            Debug.LogError(request.error);
        }
    }
    IEnumerator GetSpecificUserInfoFromServer()
    {
        UnityWebRequest request = UnityWebRequest.Get("http://localhost:3000/userInfo/" + LoginInfoObject.PhoneNumber);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string ServerResponseJSON = request.downloadHandler.text;
            Debug.Log("Raw Server response = \n" + ServerResponseJSON);

            UserInfoResponse ServerResponseObject = JsonUtility.FromJson<UserInfoResponse>(ServerResponseJSON);

            if (ServerResponseObject.Success)
            {
                Debug.Log("User info received");

                Script1.SetUserInfo(ServerResponseObject.UserInfo);

                LoginScript.LoadHomePage();
            }
            else
            {
                Debug.LogError("User not found");
            }
        }
        else
        {
            Debug.LogError(request.error);
        }
    }
}