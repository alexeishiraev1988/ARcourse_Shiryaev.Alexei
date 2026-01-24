using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private Object sceneToLoad; 

    public void LoadScene()
    {
        if (sceneToLoad != null)
        {
            // Получаем имя сцены из ассета
            string sceneName = sceneToLoad.name;
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Сцена не назначена! Перетащи сцену в инспекторе.");
        }
    }
}
