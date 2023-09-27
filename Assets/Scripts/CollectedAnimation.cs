using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectedAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator boxAnimator;

    void PlayCollectedAnimation()
    {
        if (gameObject.name == "QuestionBox")
        {
            boxAnimator.Play("questionbox-collected");
        }
    }
}
