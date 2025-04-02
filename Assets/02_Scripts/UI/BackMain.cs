using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackMain : MonoBehaviour
{
    public Button backButton;

    private void Start()
    {
        if(backButton != null)
        {
            backButton.onClick.AddListener(ChangeMainScene);
        }
    }
    public void ChangeMainScene()
    {
        SceneManager.LoadScene("MainMenu");
        Cursor.visible = true;
    }
}
