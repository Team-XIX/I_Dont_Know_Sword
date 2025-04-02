using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{
    public Button targetButton;
    public Button quitButton;
    public AudioClip clickSound;

    private void Start()
    {
        if (targetButton != null)
            targetButton.onClick.AddListener(() => StartCoroutine(ChangeScene()));

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

    }
    IEnumerator ChangeScene()
    {
        if (clickSound != null)
        {
            AudioManager.Instance.PlaySFX(clickSound);
            yield return new WaitForSeconds(clickSound.length);
        }

        SceneManager.LoadScene("GameScene");
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}