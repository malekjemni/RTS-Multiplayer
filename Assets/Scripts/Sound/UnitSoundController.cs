using UnityEngine;
using UnscriptedLogic;

public class UnitSoundController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private FXPair[] footstepsFXs;

    public void PlayFootStep()
    {
        FXManager.instance.PlayFXPair(RandomLogic.FromArray(footstepsFXs), transform.position);
    }
}
