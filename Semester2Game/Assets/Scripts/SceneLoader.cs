using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider loadingBar;

    public void DeactivateObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void LoadNewLevel(string levelToLoad)
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }

        StartCoroutine(LoadLevelAsync(levelToLoad));
    }

    public void ReloadLevel()
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }

        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        StartCoroutine(LoadLevelAsync(SceneManager.GetActiveScene().name));
    }

    private IEnumerator LoadLevelAsync(string levelToLoad)
    {
        AsyncOperation load = SceneManager.LoadSceneAsync(levelToLoad);

        if (loadingBar != null)
        {
            float progressBar;
            while (!load.isDone)
            {
                progressBar = Mathf.Clamp01(load.progress / 0.9f);
                loadingBar.value = progressBar;
                yield return null;
            }
        }
    }
}
