using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomEncounter : MonoBehaviour
{
    public string sceneToLoad;
    private int encounterChanceDigit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EncounterSquare"))
        {
            encounterChanceDigit = Random.Range(1, 10);
            if (encounterChanceDigit == 1)
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}
