using UnityEngine;
using TMPro;
using System.Collections;

public class TextBlinker : MonoBehaviour
{
    public TextMeshProUGUI textMesh;


    public static TextBlinker Instance;

    private void Awake()
    {
        Instance = this;
    }



    public void StartUI()
    { Debug.Log("1");
        StartCoroutine("BlinkText");
    }
    IEnumerator BlinkText()
    {

     
           
            yield return new WaitForSeconds(2f); // Wait for 0.5 seconds
        textMesh.enabled = true;

        // Turn text off
        textMesh.enabled = false;
            yield return new WaitForSeconds(1f); // Wait for another 0.5 seconds

            // Blink again
            textMesh.enabled = true;
            yield return new WaitForSeconds(3f); // Wait for 0.5 seconds

            // Turn text off
            textMesh.enabled = false;
         
        
    }
}
