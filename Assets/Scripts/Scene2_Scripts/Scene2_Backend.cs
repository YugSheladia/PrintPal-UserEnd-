using JetBrains.Annotations;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;


//Wrapper class for Phone Number Response from server
[System.Serializable]
public class PhoneNumberAvailabilityResponse
{
    public bool Success;
    public string Message;
}





//Used for account creation and phone number availability check.
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
public class UserInfoRecieved
{
    public bool     Success;
    public string   Message;
    public int      UserID;
}





//Used for login verification.
[System.Serializable]
public class LoginVerificationResponseFromServer
{
    public bool Success;
    public string Message;

    public UserInfo User;
}





//Used for getting the ID and name of all registered stationeries.
[System.Serializable] 
public class StationeryNameSummary
{
    public int StationeryID;
    public string StationeryName;
}

[System.Serializable]
public class GetStationeriesResponse
{
    public bool Success;
    public StationeryNameSummary[] Stationeries;
}





//==================================
//Main class
//==================================
public class Scene2_Backend : MonoBehaviour
{
    [SerializeField] Scene2Manager Scene2ManagerScript;
    [SerializeField] SelectStationery SelectStationeryScript;

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
            Script1.StationeryName = SelectStationeryScript.StationeryName;

            UserInfoObject.PrintUserInfo();
            Scene2ManagerScript.DisableAllPanels();
        }
        else
        {
            Debug.LogError(request.error);
            SelectStationeryScript.StationeryName = null;
            Scene2ManagerScript.DisableInvisiblePanel();
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
                Debug.Log("User info received");

                Script1.SetUserInfo(ServerResponseObject.User);

                StartCoroutine(GetStationeryName(Script1.StationeryID));

                LoginScript.LoadHomePage();

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



    //Getting StationeryName after login.
    [System.Serializable]
    public class GetStationeryNameResponse
    {
        public bool     success;
        public string   StationeryName;
    }
    IEnumerator GetStationeryName(int stationeryID)
    {
        Debug.Log("1");
        using (UnityWebRequest request = UnityWebRequest.Get("http://localhost:3000/get-stationery-name/" + stationeryID))
        {
            Debug.Log("2");
            yield return request.SendWebRequest();

            Debug.Log("3");
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("4");
                Debug.LogError("GetStationeryName Error: " + request.error);
                yield break;
            }

            Debug.Log("5");
            GetStationeryNameResponse response =
                JsonUtility.FromJson<GetStationeryNameResponse>(request.downloadHandler.text);

            Debug.Log("6");
            if (response.success)
            {
                Debug.Log("7");
                Debug.Log("Stationery Name: " + response.StationeryName);

                Script1.StationeryName = response.StationeryName;
            }
            else
            {
                Debug.Log("8");
                Debug.LogError("Server Error: " + request.downloadHandler.text);
            }
        }
    }












    //Getting all the registers stationery's name and ID from server, and displaying in in the Stationery dropdown.
    public GetStationeriesResponse StationeryResponse = new GetStationeriesResponse();
    public void GetStationeries()
    {
        StartCoroutine(Coroutine_GetStationeries());
    }
    IEnumerator Coroutine_GetStationeries()
    {
        UnityWebRequest request = new UnityWebRequest(
                                                        "http://localhost:3000/stationeries",
                                                        "GET"
                                                     );

        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            yield break;
        }

        Debug.Log(request.downloadHandler.text);
        
        StationeryResponse =
            JsonUtility.FromJson<GetStationeriesResponse>(
                                                             request.downloadHandler.text
                                                         );

        if (!StationeryResponse.Success)
        {
            Debug.Log("Failed to fetch stationeries.");
            yield break;
        }

        if (StationeryResponse.Stationeries == null 
            ||
            StationeryResponse.Stationeries.Length == 0)
        {
            Debug.Log("No stationeries found.");
            yield break;
        }

        PopulateDropdown();

        Scene2ManagerScript.DisableAllPanels();
        Scene2ManagerScript.EnableSetStationeryNamePanel();
    }
    void PopulateDropdown()
    {
        TMP_Dropdown dropdown = SelectStationeryScript.StationeryNameDropDown;

        dropdown.ClearOptions();

        foreach (StationeryNameSummary stationery in StationeryResponse.Stationeries)
        {
            dropdown.options.Add(
                new TMP_Dropdown.OptionData(stationery.StationeryName)
            );
        }

        dropdown.RefreshShownValue();
    }
}