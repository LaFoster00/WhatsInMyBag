using System;
using System.Collections;
using Bolt;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] private LevelManagerData data;
    [SerializeField] private string loadingScreen;

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).parent = null;
        }
        
        if (Instance && Instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }
        
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    public void SwitchScene(int scene)
    {
        data.loadingProgress = 0;
        SceneManager.LoadScene(loadingScreen);
        StartCoroutine(UpdateLoadingIndicator(() => SceneManager.LoadSceneAsync(scene)));
    }

    public void SwitchScene(string scene)
    {
        data.loadingProgress = 0;
        SceneManager.LoadScene(loadingScreen);
        StartCoroutine(UpdateLoadingIndicator(() => SceneManager.LoadSceneAsync(scene)));
    }

    public void ResetScene()
    {
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SwitchScene(activeSceneIndex);
    }

    private IEnumerator UpdateLoadingIndicator(Func<AsyncOperation> loadSceneFunction)
    {
        yield return new WaitForSecondsRealtime(1.0f);
        var loadSceneAsync = loadSceneFunction.Invoke();
        while (!Mathf.Approximately(loadSceneAsync.progress, 1.0f))
        {
            data.loadingProgress = loadSceneAsync.progress;
            yield return null;
        }
    }
}