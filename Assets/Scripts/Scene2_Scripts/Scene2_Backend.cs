using JetBrains.Annotations;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


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

            PhoneNumberAvailabilityResponse ServerResponseObject = JsonUtility.FromJson<PhoneNumberAvailabilityResponse>(ServerResponseJSON);

            if(ServerResponseObject.Success == true)
            {
                Scene2ManagerScript.DisableAllPanels();
                Scene2ManagerScript.EnablePasswordPanel();
            }
            else
            {
                Scene2ManagerScript.DisableAllPanels();
                Scene2ManagerScript.EnableSetPhoneNumberPanel();
                Scene2ManagerScript.EnableErrorPanel();

                Scene2ManagerScript.ErrorText.text = "Phone number already taken.";
            }
        }
        else
        {
            Scene2ManagerScript.DisableAllPanels();
            Scene2ManagerScript.EnableSetPhoneNumberPanel();
            Scene2ManagerScript.EnableErrorPanel();

            Scene2ManagerScript.ErrorText.text = "Error: " + request.error + "\nRestart the app and try again.";
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

            UserInfoRecieved UserInfoRecievedObject = JsonUtility.FromJson<UserInfoRecieved>(ServerResponseJSON);

            UserInfoObject.UserID = UserInfoRecievedObject.UserID;

            Script1.SetUserInfo(UserInfoObject);
            Script1.StationeryName = SelectStationeryScript.StationeryName;

            SceneManager.LoadScene("Scene3_Home");
        }
        else
        {
            Scene2ManagerScript.DisableAllPanels();
            Scene2ManagerScript.EnableSetProfileInfoPanel();
            Scene2ManagerScript.EnableErrorPanel();

            Scene2ManagerScript.ErrorText.text = "Error: " + request.error + "\nRestart the app and try again.";

            SelectStationeryScript.StationeryName = null;
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

        UnityWebRequest request = new UnityWebRequest
        (
            "http://localhost:3000/userLogin",
            "POST"
        );

        request.uploadHandler = new UploadHandlerRaw(LoginInfoRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader
        (
            "Content-Type",
            "application/json"
        );

        yield return request.SendWebRequest();

        string ServerResponse = request.downloadHandler.text;

        //Debug.Log("Raw server response =\n" + ServerResponse);

        LoginVerificationResponseFromServer ServerResponseObject =
            JsonUtility.FromJson<LoginVerificationResponseFromServer>(ServerResponse);

        if (request.responseCode == 200)
        {
            Debug.Log("User info received");

            Script1.SetUserInfo(ServerResponseObject.User);

            yield return StartCoroutine(GetStationeryName(Script1.StationeryID));

            LoginScript.LoadHomePage();

            Debug.Log("Unity - Successfully logged in.");
        }
        else
        {
            Scene2ManagerScript.EnableErrorPanel();

            Scene2ManagerScript.DisableAllPanels();
            Scene2ManagerScript.EnableLoginPanel();
  
            if (ServerResponseObject != null)
            {
                Scene2ManagerScript.ErrorText.text = ServerResponseObject.Message;
            }
            else
            {
                Scene2ManagerScript.ErrorText.text = "An unknown error occurred.\nRestart the app and try again.";
            }

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
        using (UnityWebRequest request = UnityWebRequest.Get("http://localhost:3000/get-stationery-name/" + stationeryID))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("GetStationeryName Error: " + request.error);

                Scene2ManagerScript.ErrorText.text = "An error occured, restart the app and try again.";
                Scene2ManagerScript.EnableErrorPanel();

                Scene2ManagerScript.DisableAllPanels();
                Scene2ManagerScript.EnableLoginPanel();

                yield break;
            }

            GetStationeryNameResponse response =
                JsonUtility.FromJson<GetStationeryNameResponse>(request.downloadHandler.text);

            if (response.success)
            {
                Debug.Log("Stationery Name: " + response.StationeryName);

                Script1.StationeryName = response.StationeryName;
            }
            else
            {
                Debug.LogError("Server Error: " + request.downloadHandler.text);

                Scene2ManagerScript.ErrorText.text = "An error occured, restart the app and try again.";
                Scene2ManagerScript.EnableErrorPanel();

                Scene2ManagerScript.DisableAllPanels();
                Scene2ManagerScript.EnableLoginPanel();
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
            Scene2ManagerScript.DisableAllPanels();
            Scene2ManagerScript.EnablePasswordPanel();
            Scene2ManagerScript.EnableErrorPanel();

            Scene2ManagerScript.ErrorText.text = "Error: Unsuccessful Web Request\nRestart the app and try again.";

            yield break;
        }
        
        StationeryResponse =
            JsonUtility.FromJson<GetStationeriesResponse>(
                                                             request.downloadHandler.text
                                                         );

        if (!StationeryResponse.Success) 
        {
            Scene2ManagerScript.DisableAllPanels();
            Scene2ManagerScript.EnablePasswordPanel();
            Scene2ManagerScript.EnableErrorPanel();

            Scene2ManagerScript.ErrorText.text = "Error: Failed to fetch stationeries\nRestart the app and try again.";

            yield break;
        }

        if (StationeryResponse.Stationeries == null 
            ||
            StationeryResponse.Stationeries.Length == 0)      
        {
            Scene2ManagerScript.DisableAllPanels();
            Scene2ManagerScript.EnablePasswordPanel();
            Scene2ManagerScript.EnableErrorPanel();

            Scene2ManagerScript.ErrorText.text = "Error: No stationeries found\nRestart the app and try again.";

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