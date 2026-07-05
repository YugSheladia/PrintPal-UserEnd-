using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


public class UpdatedUserInfo
{
    public string PhoneNumber;
    public string Name;
    public string UPINumber;
    public string Email;
    public string Username;
}


public class UserInfoWrapper
{
    public bool Success;
    public UserInfo UserInfo;
}



public class Scene4_Backend : MonoBehaviour
{
    [SerializeField] ViewProfileInfo ViewProfileInfoScript;
    [SerializeField] UpdateProfileInfo UpdateProfileInfoScript;

    UpdatedUserInfo UpdatedUserInfoObject;
    public void StartCoroutine_UpdateUserInfoInServer(UpdatedUserInfo UpdatedUserInfoObject)
    {
        this.UpdatedUserInfoObject = UpdatedUserInfoObject;

        StartCoroutine(UpdateUserInfoInServer());
    }
    IEnumerator UpdateUserInfoInServer()
    {
        string UserInfoObjectJSON = JsonUtility.ToJson(UpdatedUserInfoObject);

        byte[] UserInfoObjectRaw = System.Text.Encoding.UTF8.GetBytes(UserInfoObjectJSON);

        UnityWebRequest request = new UnityWebRequest(
                                                        "http://localhost:3000/update-user",
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

            Script1.UserActualName          = UpdatedUserInfoObject.Name;
            Script1.UPILinkedPhoneNumber    = UpdatedUserInfoObject.UPINumber;
            Script1.UserEmail               = UpdatedUserInfoObject.Email;
            Script1.UserName                = UpdatedUserInfoObject.Username;

            ViewProfileInfoScript.UpdateProfileInfo();
        }
        else
        {
            Debug.LogError(request.error);
        }
    }



    UserInfoWrapper UserInfoWrapperObject;
    public void StartCoroutine_GetUserInfoFromServer()
    {
        StartCoroutine(GetUserInfoFromServer());
    }
    IEnumerator GetUserInfoFromServer()
    {
        UnityWebRequest request = UnityWebRequest.Get("http://localhost:3000/userInfo/" + Script1.PhoneNumber);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string ServerResponseJSON = request.downloadHandler.text;

            Debug.Log("Raw Server response = \n" + ServerResponseJSON);

            UserInfoWrapperObject = JsonUtility.FromJson<UserInfoWrapper>(ServerResponseJSON);

            if (UserInfoWrapperObject.Success)
            {
                Debug.Log("User info received");

                Script1.SetUserInfo(UserInfoWrapperObject.UserInfo);

                ViewProfileInfoScript.SetUI();
                UpdateProfileInfoScript.SetUI();
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
