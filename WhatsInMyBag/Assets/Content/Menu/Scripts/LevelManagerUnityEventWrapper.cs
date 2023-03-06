using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManagerUnityEventWrapper : MonoBehaviour
{
    public void SwitchScene(string scene)
    {
        LevelManager.Instance.SwitchScene(scene);
    }
}
