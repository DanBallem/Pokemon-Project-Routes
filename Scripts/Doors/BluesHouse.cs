using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluesHouse : MonoBehaviour
{
    public GameObject player;
    public GameObject loadPoint;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        player.transform.position = loadPoint.transform.position;
    }
}
