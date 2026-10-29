using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu_Manager : MonoBehaviour
{
    public string SceneSelectorGameField = "SampleScene";
    public void PlayGame()
    {  
        SceneManager.LoadScene(SceneSelectorGameField);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
