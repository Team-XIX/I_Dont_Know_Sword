using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{
    public Button targetButton;
    public AudioSource audioSource;
    public  AudioClip clickSound;

    private void Start()
    {
        if (targetButton != null)
            targetButton.onClick.AddListener(() => StartCoroutine(ChangeScene()));
    }
    IEnumerator ChangeScene()
    {
        if(audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
            yield return new WaitForSeconds(clickSound.length);
        }

        SceneManager.LoadScene("UIScene");
    }
}
