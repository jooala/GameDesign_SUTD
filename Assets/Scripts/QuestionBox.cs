using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionBox : MonoBehaviour
{
    public Animator boxAnimator;
    public Animator coinAnimator;
    public AudioSource coinBounceAudio;

    [System.NonSerialized]
    public bool collected = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !collected)
        {
            Debug.Log("Collided with questionbox!");
            collected = true;
            boxAnimator.Play("questionbox-spring");
            coinAnimator.Play("coinbounce");
            coinBounceAudio.PlayOneShot(coinBounceAudio.clip);

        }
    }
}
