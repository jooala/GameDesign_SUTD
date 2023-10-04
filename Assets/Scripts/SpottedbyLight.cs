using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpottedbyLight : MonoBehaviour
{
    public AudioSource goombaAudio;
    public Animator alertAnimator;
    public Animator goombaAnimator;

    private bool triggerOnCooldown = false;
    private float cooldownDuration = 5.0f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !triggerOnCooldown)
        {
            alertAnimator.Play("alert-spring");
            goombaAudio.PlayOneShot(goombaAudio.clip);
            goombaAnimator.Play("goomba-alerted");

            triggerOnCooldown = true;
            StartCoroutine(TriggerCooldown());
        }
    }
    private IEnumerator TriggerCooldown()
    {
        yield return new WaitForSeconds(cooldownDuration);
        triggerOnCooldown = false;
    }

    public void RestartGame()
    {
        triggerOnCooldown = false;
    }
}
