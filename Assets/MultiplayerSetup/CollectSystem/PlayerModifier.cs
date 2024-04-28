using System.Collections;
using UnityEngine;

public class PlayerModifier : MonoBehaviour
{
    public float speedDuration;
    public float jumpDuration;
    public float speed;
    public float sprintSpeed;
    public float jump;
    public float stunDuration;

    private float initialSpeed;
    private float initialJump;
    private float initialsprintSpeed;

    private ThirdPersonController player;

    void Start()
    {
        player = GetComponent<ThirdPersonController>();
        initialSpeed = player.MoveSpeed;
        initialJump = player.JumpHeight;
        initialsprintSpeed = player.SprintSpeed;

    }
    public void AddSpeed()
    {
       StartCoroutine(SpeedEffectDuration());
    }
    public void AddJump()
    {
        StartCoroutine(JumpEffectDuration());
    }
    public void Stun()
    {
       StartCoroutine(StunDuration());
    }

    IEnumerator SpeedEffectDuration()
    {
        player.MoveSpeed = speed;
        player.SprintSpeed = sprintSpeed;
        yield return new WaitForSeconds(speedDuration);
        player.MoveSpeed = initialSpeed;
        player.SprintSpeed = initialsprintSpeed;
    }

    IEnumerator JumpEffectDuration()
    {      
        player.JumpHeight = jump;
        yield return new WaitForSeconds(jumpDuration);
        player.JumpHeight = initialJump;
    }

    IEnumerator StunDuration()
    {
        player.MoveSpeed = 0f;
        player.SprintSpeed = 0f;
        yield return new WaitForSeconds(stunDuration);
        player.MoveSpeed = initialSpeed;
        player.SprintSpeed = initialsprintSpeed;
    }
}
