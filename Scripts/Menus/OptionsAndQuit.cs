using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class OptionsAndQuit : MonoBehaviour
{
    public void LoadScene(string Options)
    {
        SceneManager.LoadScene(Options);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}