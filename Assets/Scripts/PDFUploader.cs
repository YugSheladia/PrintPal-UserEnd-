using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;



[System.Serializable]
public class UploadResponse2
{
    public bool Success;
    public int PrintID;
    public string PDFName;
    public string OriginalFileName;
    public int PageCount;
    public long FileSize;
    public string UploadedAt;
}



[System.Serializable]
public class PrintInfoResponse
{
    public bool Success;
    public int PrintID;
    public string PDFName;
    public long FileSize;
    public int PageCount;
    public string UploadedAt;
}





public class PDFUploader : MonoBehaviour
{
    [SerializeField] private string ServerURL = "http://localhost:3001";

    [SerializeField] private TMP_InputField PrintIDText;




    public void SelectPDF()
    {
        NativeFilePicker.PickFile(
            (path) =>
            {
                if (string.IsNullOrEmpty(path))
                {
                    Debug.Log("No file selected.");
                    return;
                }

                Debug.Log("Selected PDF:");

                Debug.Log(path);

                StartCoroutine(UploadPDF(path));
            },
            new string[]
            {
            "application/pdf"
            });
    }
    IEnumerator UploadPDF(string filePath)
    {
        //The actual contents of your file.

        byte[] pdfBytes = File.ReadAllBytes(filePath);

        Debug.Log("File Size: " + pdfBytes.Length + " bytes");

        WWWForm form = new WWWForm();

        form.AddBinaryData(
            "PDF",
            pdfBytes,
            Path.GetFileName(filePath),
            "application/pdf"
        );

        using (UnityWebRequest request =
               UnityWebRequest.Post("http://localhost:3001/upload/print", form))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);

                UploadResponse2 response = JsonUtility.FromJson<UploadResponse2>(request.downloadHandler.text);

                Debug.Log(response.Success); 
                Debug.Log(response.PrintID);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
            }
        }
    }


    

    public void ClickedGetPrintDetails()
    {
        int PrintID = int.Parse(PrintIDText.text.ToString());
        GetPrintInfo(PrintID);
    }    
    public void GetPrintInfo(int printID)
    {
        StartCoroutine(Coroutine_GetPrintInfo(printID));
    }
    IEnumerator Coroutine_GetPrintInfo(int printID)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(ServerURL + "/print/" + printID))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
                yield break;
            }

            PrintInfoResponse response =
                JsonUtility.FromJson<PrintInfoResponse>(request.downloadHandler.text);

            Debug.Log("Success: " + response.Success);
            Debug.Log("PrintID: " + response.PrintID);
            Debug.Log("PDF Name: " + response.PDFName);
            Debug.Log("File Size: " + response.FileSize + " bytes");
            Debug.Log("Page Count: " + response.PageCount);
            Debug.Log("Uploaded At: " + response.UploadedAt);
        }
    }




    public void ClickedDownloadAndOpenPDF()
    {
        int PrintID = int.Parse(PrintIDText.text.ToString());
        DownloadPDF(PrintID);
    }
    public void DownloadPDF(int printID)
    {
        StartCoroutine(Coroutine_DownloadPDF(printID));
    }
    IEnumerator Coroutine_DownloadPDF(int printID)
    {
        string savePath = Path.Combine(
            Application.persistentDataPath,
            printID + ".pdf"
        );

        using (UnityWebRequest request =
            UnityWebRequest.Get(ServerURL + "/download/" + printID))
        {
            request.downloadHandler = new DownloadHandlerFile(savePath);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log("Saved to: " + savePath);

                Application.OpenURL("file://" + savePath);
            }
        }
    }
}