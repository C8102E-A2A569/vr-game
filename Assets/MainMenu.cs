using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Имя сцены для загрузки при нажатии 'Играть'")]
    public string gameSceneName = "SampleScene"; // Укажи имя своей сцены

    // Вызывается кнопкой "Играть"
    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    // Вызывается кнопкой "Выход"
    public void QuitGame()
    {
        Debug.Log("Игра закрывается...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
