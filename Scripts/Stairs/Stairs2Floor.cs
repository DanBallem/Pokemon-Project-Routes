using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs2Floor : MonoBehaviour
{
    public GameObject floorOne;
    public GameObject floorTwo;
    private bool onFloor1 = false;
    private void Start()
    {
        floorOne.SetActive(false);
        floorTwo.SetActive(true);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
    if (onFloor1 == false)
    {
        floorOne.SetActive(true);
        floorTwo.SetActive(false);
        onFloor1 = true;
    }
    else if (onFloor1 == true)
    {
        floorOne.SetActive(false);
        floorTwo.SetActive(true);
        onFloor1 = false;
    }
    }
}