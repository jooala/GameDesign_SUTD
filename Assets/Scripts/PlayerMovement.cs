using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PlayerMovement : MonoBehaviour
{
    public float speed = 10;
    private Rigidbody2D marioBody;
    public float maxSpeed = 20;
    // Start is called before the first frame update
    public float upSpeed = 10;
    private bool onGroundState = true;
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;
    public GameManager gameManager;
    public Transform gameCamera;
    public Animator marioAnimator;
    public Animator[] boxAnimators;
    public QuestionBox[] questionBoxes;
    public CoinSound[] coins;
    public Bricks[] bricks;
    public AudioSource marioAudio;
    public AudioSource marioDeath;
    public float deathImpulse;

    private bool moving = false;
    private bool jumpedState = false;

    // state
    [System.NonSerialized]
    public bool alive = true;
    void Start()
    {
        // Set to be 30 FPS
        Application.targetFrameRate = 30;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        marioAnimator.SetBool("onGround", onGroundState);
    }

    // Update is called once per frame
    void Update()
    {
        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.velocity.x));
    }

    int collisionLayerMask = (1 << 3) | (1 << 6) | (1 << 7);
    void OnCollisionEnter2D(Collision2D col)
    {
        if (((collisionLayerMask & (1 << col.transform.gameObject.layer)) > 0) & !onGroundState)
        {
            onGroundState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);
        }
    }

    void GameOverScene()
    {
        gameManager.GameOver();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            marioAnimator.Play("mario-die");
            marioDeath.PlayOneShot(marioDeath.clip);
            alive = false;
        }
    }

    void FixedUpdate()
    {
        if (alive && moving)
        {
            Move(faceRightState == true ? 1 : -1);
        }
    }

    public void RestartButtonCallback(int input)
    {
        GameRestart();
        Time.timeScale = 1.0f;
    }

    public void GameRestart()
    {
        marioBody.transform.position = new Vector3(-7.46f, -3.44f, 0.0f);
        faceRightState = true;
        marioSprite.flipX = false;
        gameCamera.position = new Vector3(-1.92f, 0, -10);

        foreach (Animator ba in boxAnimators)
        {
            ba.SetTrigger("gameOver");
        }

        foreach (QuestionBox qa in questionBoxes)
        {
            qa.collected = false;
        }

        foreach (Bricks bc in bricks)
        {
            bc.collected = false;
        }

        foreach (CoinSound co in coins)
        {
            co.gameObject.SetActive(true);
        }
        marioAnimator.SetTrigger("gameOver");
        alive = true;
    }

    void Move(int value)
    {
        Vector2 movement = new Vector2(value, 0);
        if (marioBody.velocity.magnitude < maxSpeed)
            marioBody.AddForce(movement * speed);
    }

    public void Jump()
    {
        if (alive && onGroundState)
        {
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;
            jumpedState = true;
            marioAnimator.SetBool("onGround", onGroundState);

        }
    }

    public void MoveCheck(int value)
    {
        if (value == 0)
        {
            moving = false;
        }
        else
        {
            FlipMarioSprite(value);
            moving = true;
            Move(value);
        }
    }

    void FlipMarioSprite(int value)
    {
        if (value == -1 && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
            if (marioBody.velocity.x > 0.05f)
                marioAnimator.SetTrigger("onSkid");

        }

        else if (value == 1 && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
            if (marioBody.velocity.x < -0.05f)
                marioAnimator.SetTrigger("onSkid");
        }
    }

    public void JumpHold()
    {
        if (alive && jumpedState)
        {
            marioBody.AddForce(Vector2.up * upSpeed * 30, ForceMode2D.Force);
            jumpedState = false;

        }
    }


    void PlayJumpSound()
    {
        marioAudio.PlayOneShot(marioAudio.clip);
    }

    void PlayDeathImpulse()
    {
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }
}