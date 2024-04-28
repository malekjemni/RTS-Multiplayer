using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] PossibleSpots;
    public GameObject Coin;

    public GameObject Item;


    public static ItemSpawner Instance;


    private void Awake()
    {
        Instance = this;
        int randomIndex = Random.Range(0, PossibleSpots.Length);


        Debug.Log(PossibleSpots[randomIndex].transform.parent.name);
 
        PossibleSpots[randomIndex].transform.parent.GetComponent<BoxHandler>().isGolden = true;
        Debug.Log(PossibleSpots[randomIndex].transform.parent.GetComponent<BoxHandler>().isGolden);
        Item= Instantiate(Coin, PossibleSpots[randomIndex].transform.position, Quaternion.identity);
    }
}
