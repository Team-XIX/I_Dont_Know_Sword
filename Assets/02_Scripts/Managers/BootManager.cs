using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootManager : MonoBehaviour
{
    [SerializeField] private float delayTime = 0.2f; // 대기 시간(초)
    [SerializeField] private string nextSceneName = "MainMenu"; // 다음 씬 이름

    void Start()
    {
        StartCoroutine(LoadNextSceneAfterDelay());
    }

    private IEnumerator LoadNextSceneAfterDelay()
    {
        // 지정된 시간만큼 대기
        yield return new WaitForSeconds(delayTime);

        // 다음 씬으로 전환
        SceneManager.LoadScene(nextSceneName);
    }
}