using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;

    [SerializeField] private Slider loadingBar;

    public void LoadNewLevel(string levelToLoad)
    {
        loadingScreen.SetActive(true);
        StartCoroutine(LoadLevelAsync(levelToLoad));
    }

    private IEnumerator LoadLevelAsync(string levelToLoad)
    {
        AsyncOperation load = SceneManager.LoadSceneAsync(levelToLoad);

        float progressBar;
        while (!load.isDone)
        {
            progressBar = Mathf.Clamp01(load.progress / 0.9f);
            loadingBar.value = progressBar;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
    }
}
