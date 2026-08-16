using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading_Landing_Script : MonoBehaviour
{
    [SerializeField] private GameObject BlankPanel;

    [SerializeField] private GameObject LoadingScreenPanel;

    [SerializeField] private float Delay = 5f;

    private void Start()
    {
        LoadingScreenPanel.SetActive(true);
        DisableBlankPanel();
        StartCoroutine(LoadNextPanelAfterDelay());
    }

    public void EnableBlankPanel()
    {
        BlankPanel.SetActive(true);
    }
    public void DisableBlankPanel()
    {
        BlankPanel?.SetActive(false);
    }


    IEnumerator LoadNextPanelAfterDelay()
    {
        yield return new WaitForSeconds(Delay);

        SceneManager.LoadScene("Scene2_SetupProfileInfo");
    }
}
