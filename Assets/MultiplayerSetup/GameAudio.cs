using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudio : MonoBehaviour
{
    private FXManager fxManager;

    private void Start()
    {
        fxManager = FXManager.instance;
        fxManager.PlayThemeProceedSound();
    }

  
}
