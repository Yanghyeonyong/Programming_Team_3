using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void Load(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }


    public void Load(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
   
}
