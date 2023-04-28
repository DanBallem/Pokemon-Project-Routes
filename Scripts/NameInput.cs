using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NameInput : MonoBehaviour
{
    public string nameInput;
    public GameObject inputField;
    public GameObject textDisplay;
    public string nameStorage;
    public GameObject NameInputText;
    public GameObject WelcomeText;
    public void StoreName()
    {
        nameInput = inputField.GetComponent<Text>().text;
        textDisplay.GetComponent<Text>().text = "Welcome " + nameInput + " to Pokémon: Project Routes!";
        nameStorage = nameInput;
        NameInputText.SetActive(false);
        WelcomeText.SetActive(true);
    }
}
