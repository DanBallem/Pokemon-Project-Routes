using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignInteraction : MonoBehaviour
{
    public GameObject UI;
    private void OnCollisionEnter2D(Collision2D collision)
    {
            UI.gameObject.SetActive(true);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        UI.gameObject.SetActive(false);
    }
}