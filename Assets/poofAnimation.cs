using GameCreator.Runtime.Characters;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class poofAnimation : MonoBehaviour
{
    public static poofAnimation Instance;
    public GameObject poof;
    public TextMeshProUGUI BoostText;
    public GameObject[] powerupsIcons;
    public GameObject[] Players;

    private void Awake()
    {
        Instance = this;
    }




    public void MisteryEffetc()
    {
        int rand=Random.Range(1, 4);
        if(rand==1)
        {
            BoostText.text = "Movement Speed !";
            Players[0].GetComponent<Character>().Motion.LinearSpeed = 10f;
            Debug.Log("powerup1");
        }
        else if (rand == 2)
        {
            BoostText.text = "Players Stunned !";
            Debug.Log("powerup2");
            Players[1].GetComponent<Character>().enabled =false;
        }
        else  {
            BoostText.text = "Players Has Double Jump!";
            Players[0].GetComponent<Character>().Motion.AirJumps = 1;
            Debug.Log("powerup3");
        }
        powerupsIcons[rand-1].SetActive(true);
        StartCoroutine("BoostTextOff");
        StartCoroutine(IconOff(rand));
        SliderController.Instance.StartTimer();
    }

    IEnumerator BoostTextOff()
    {
        BoostText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        BoostText.gameObject.SetActive(false);
    }


    public void  pooftrigger()
    {
        StartCoroutine("PoofOn");
    }

    IEnumerator PoofOn()
    {
        poof.SetActive(true);
        yield return new WaitForSeconds(2f);
        poof.SetActive(false);

    }

    IEnumerator IconOff(int rand)
    {
        yield return new WaitForSeconds(20f);
        powerupsIcons[rand-1].SetActive(false);
        Players[0].GetComponent<Character>().Motion.LinearSpeed = 4f;
        Players[1].GetComponent<Character>().enabled = true;
        Players[0].GetComponent<Character>().Motion.AirJumps = 0;

    }

}
