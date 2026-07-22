using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;



//Used for updating user info through ProfileMenu -> View Profile -> Edit.
[System.Serializable]
public class UpdatedUserInfo
{
    public string PhoneNumber;
    public string Name;
    public string UPINumber;
    public string Email;
    public string Username;
}

[System.Serializable]
public class UserInfoWrapper
{
    public bool Success;
    public UserInfo UserInfo;
}





//Used for updating stationery info through ProfileMenu -> Stationery.
[System.Serializable]
public class UpdateStationeryPreferenceRequest
{
    public int UserID;
    public int StationeryPreference;
}

[System.Serializable]
public class UpdateStationeryPreferenceResponse
{
    public bool Success;
    public string Message;
}





public class Scene4_Backend : MonoBehaviour
{
    [SerializeField] Scene4Manager Scene4ManagerScript;

    [SerializeField] ViewProfileInfo ViewProfileInfoScript;
    [SerializeField] UpdateProfileInfo UpdateProfileInfoScript;
    [SerializeField] UpdateStationery UpdateStationeryScript;





    //Used for updating user info through ProfileMenu -> View Profile -> Edit.
    UpdatedUserInfo UpdatedUserInfoObject;

    public void UpdateUserInfoInServer
    (
        UpdatedUserInfo UpdatedUserInfoObject,
        System.Action<bool> OnComplete
    )
    {
        this.UpdatedUserInfoObject = UpdatedUserInfoObject;

        StartCoroutine
        (
            Coroutine_UpdateUserInfoInServer(OnComplete)
        );
    }

    IEnumerator Coroutine_UpdateUserInfoInServer
    (
        System.Action<bool> OnComplete
    )
    {
        string UserInfoObjectJSON = JsonUtility.ToJson(UpdatedUserInfoObject);

        byte[] UserInfoObjectRaw =
            Encoding.UTF8.GetBytes(UserInfoObjectJSON);

        UnityWebRequest request = new UnityWebRequest
        (
            "http://localhost:3000/update-user",
            "POST"
        );

        request.uploadHandler = new UploadHandlerRaw(UserInfoObjectRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader
        (
            "Content-Type",
            "application/json"
        );

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string ServerResponseJSON = request.downloadHandler.text;

            Debug.Log("Raw Server Response : \n" + ServerResponseJSON);

            OnComplete?.Invoke(true);
        }
        else
        {
            Debug.LogError(request.error);

            OnComplete?.Invoke(false);
        }
    }






    //Used for displaying user info in Profile Menu -> View Profile
    UserInfoWrapper UserInfoWrapperObject;

    public void GetUserInfoFromServer(System.Action<bool> OnComplete)
    {
        StartCoroutine
        (
            Coroutine_GetUserInfoFromServer(OnComplete)
        );
    }

    IEnumerator Coroutine_GetUserInfoFromServer(System.Action<bool> OnComplete)
    {
        UnityWebRequest request = UnityWebRequest.Get
        (
            "http://localhost:3000/userInfo/" + Script1.PhoneNumber
        );

        yield return request.SendWebRequest();




        if (request.result == UnityWebRequest.Result.Success)
        {
            string ServerResponseJSON = request.downloadHandler.text;

            Debug.Log("Raw Server response = \n" + ServerResponseJSON);

            UserInfoWrapperObject = JsonUtility.FromJson<UserInfoWrapper>
            (
                ServerResponseJSON
            );

            if (UserInfoWrapperObject.Success)
            {
                Debug.Log("User info received");

                Script1.SetUserInfo(UserInfoWrapperObject.UserInfo);

                OnComplete?.Invoke(true);
            }
            else
            {
                Debug.LogError("User not found");

                Scene4ManagerScript.MessageText.text = "User not found.\nRestart the app and try again.";

                OnComplete?.Invoke(false);
            }
        }
        else
        {
            Debug.LogError(request.error);

            Scene4ManagerScript.MessageText.text = "User not found.\nRestart the app and try again.";

            OnComplete?.Invoke(false);
        }
    }








    //Used for displaying the names of all registered stationeries.
    public GetStationeriesResponse StationeryResponse;

    public void GetStationeries(System.Action<bool> OnComplete)
    {
        StartCoroutine
        (
            Coroutine_GetStationeries(OnComplete)
        );
    }

    IEnumerator Coroutine_GetStationeries(System.Action<bool> OnComplete)
    {
        UnityWebRequest request = new UnityWebRequest
        (
            "http://localhost:3000/stationeries",
            "GET"
        );

        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();




        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);

            Scene4ManagerScript.MessageText.text = "An error occured.\nRestart the app and try again.";

            OnComplete?.Invoke(false);
            yield break;
        }



        Debug.Log(request.downloadHandler.text);

        StationeryResponse = JsonUtility.FromJson<GetStationeriesResponse>
        (
            request.downloadHandler.text
        );



        if (!StationeryResponse.Success)
        {
            Debug.Log("Failed to fetch stationeries.");

            Scene4ManagerScript.MessageText.text =
                "Failed to fetch stationeries.\nRestart the app and try again.";

            OnComplete?.Invoke(false);
            yield break;
        }

        if
        (StationeryResponse.Stationeries == null
        ||
        StationeryResponse.Stationeries.Length == 0)
        {
            Debug.Log("No stationeries found.");

            Scene4ManagerScript.MessageText.text =
                "No stationery found.\nRestart the app and try again.";

            OnComplete?.Invoke(false);
            yield break;
        }

        PopulateDropdown();

        OnComplete?.Invoke(true);
    }

    void PopulateDropdown()
    {
        Debug.Log("Stationery names = ");

        TMP_Dropdown dropdown = UpdateStationeryScript.StationeryNameDropDown;

        dropdown.ClearOptions();

        foreach (StationeryNameSummary stationery in StationeryResponse.Stationeries)
        {
            Debug.Log("Stationery = " + StationeryResponse.Stationeries);
            dropdown.options.Add
            (
                new TMP_Dropdown.OptionData(stationery.StationeryName)
            );
        }

        dropdown.RefreshShownValue();
    }









    //Used for updating stationery info through ProfileMenu -> Stationery.
    UpdateStationeryPreferenceRequest Request =
    new UpdateStationeryPreferenceRequest();

    public void UpdateStationeryPreference
    (
        int StationeryID,
        System.Action<bool> OnComplete
    )
    {
        StartCoroutine
        (
            Coroutine_UpdateStationeryPreference
            (
                StationeryID,
                OnComplete
            )
        );
    }

    IEnumerator Coroutine_UpdateStationeryPreference
    (
        int StationeryID,
        System.Action<bool> OnComplete
    )
    {
        Debug.Log("Chosen StationeryID = " + StationeryID);
        Debug.Log("User ID = " + Script1.UserID);

        Request.StationeryPreference = StationeryID;
        Request.UserID = Script1.UserID;

        string json = JsonUtility.ToJson(Request);

        byte[] raw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest
        (
            "http://localhost:3000/update-stationeryPreference",
            "POST"
        );

        request.uploadHandler = new UploadHandlerRaw(raw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader
        (
            "Content-Type",
            "application/json"
        );

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);

            Scene4ManagerScript.MessageText.text = "An error occured.\nRestart the app and try again.";

            OnComplete?.Invoke(false);
            yield break;
        }

        UpdateStationeryPreferenceResponse response = JsonUtility.FromJson<UpdateStationeryPreferenceResponse>
        (
            request.downloadHandler.text
        );

        if (response.Success)
        {
            Debug.Log(response.Message);

            Script1.StationeryID = StationeryID;
            Script1.StationeryName = UpdateStationeryScript.StationeryName;

            Scene4ManagerScript.MessageText.text = "Stationery updated.";

            OnComplete?.Invoke(true);
        }
        else
        {
            Debug.LogError(response.Message);

            Scene4ManagerScript.MessageText.text = "An error occured.\nRestart the app and try again.";

            UpdateStationeryScript.StationeryName = null;

            OnComplete?.Invoke(false);
        }
    }
}
