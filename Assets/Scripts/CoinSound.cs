using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSound : MonoBehaviour
{
    public AudioSource coinCollectedSound;
    void PlayCoinCollectedSound()
    {
        coinCollectedSound.PlayOneShot(coinCollectedSound.clip);
    }

    IEnumerator DisableCoin()
    {
        yield return new WaitForSeconds(0.3F);
        gameObject.SetActive(false);

    }
}
