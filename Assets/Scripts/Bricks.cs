using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bricks : MonoBehaviour
{
    public Animator boxAnimator;
    public Animator coinAnimator;
    public AudioSource coinBounceAudio;
    public bool hasCoin;

    [System.NonSerialized]
    public bool collected = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collided with bricks!");
            boxAnimator.Play("bricks-spring");
            coinBounceAudio.PlayOneShot(coinBounceAudio.clip);
            if (!collected && hasCoin)
            {
                collected = true;
                coinAnimator.Play("coinbounce");
            }
        }
    }

}
