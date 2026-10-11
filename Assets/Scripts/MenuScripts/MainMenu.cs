using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioSource gameAudioSource;
    public AudioClip buttonPressSound;
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void PlayButtonClickSound()
    {
        gameAudioSource.PlayOneShot(buttonPressSound);
    }
}