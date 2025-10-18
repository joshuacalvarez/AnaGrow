using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader :MonoBehaviour 
{
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneByInex(int index)
    {
        SceneManager.LoadScene(index);
    }
    
}