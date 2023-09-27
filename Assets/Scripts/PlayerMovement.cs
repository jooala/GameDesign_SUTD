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
    public TextMeshProUGUI scoreText;
    public GameObject enemies;
    public JumpOverGoomba jumpOverGoomba;
    public Canvas GameplayUI;
    public Canvas GameOverCanvas;
    public TextMeshProUGUI scoreTextEnd;
    public Transform gameCamera;
    public Animator marioAnimator;
    public Animator[] boxAnimators;
    public QuestionBox[] questionBoxes;
    public CoinSound[] coins;
    public Bricks[] bricks;
    public AudioSource marioAudio;
    public AudioClip marioDeath;
    public float deathImpulse;

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
        if (Input.GetKeyDown("a") && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
            if (marioBody.velocity.x > 0.1f)
                marioAnimator.SetTrigger("onSkid");
        }

        if (Input.GetKeyDown("d") && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
            if (marioBody.velocity.x < -0.1f)
                marioAnimator.SetTrigger("onSkid");
        }

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
        Time.timeScale = 0.0f;
        scoreTextEnd.text = scoreText.text;
        GameplayUI.gameObject.SetActive(false);
        GameOverCanvas.gameObject.SetActive(true);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            marioAnimator.Play("mario-die");
            marioAudio.PlayOneShot(marioDeath);
            alive = false;
        }
    }
    // FixedUpdate is called 50 times a second
    void FixedUpdate()
    {
        if (alive)
        {
            float moveHorizontal = Input.GetAxisRaw("Horizontal");

            if (Mathf.Abs(moveHorizontal) > 0)
            {
                Vector2 movement = new Vector2(moveHorizontal, 0);
                // check if it doesn't go beyond maxSpeed
                if (marioBody.velocity.magnitude < maxSpeed)
                    marioBody.AddForce(movement * speed);
            }

            // stop
            if (Input.GetKeyUp("a") || Input.GetKeyUp("d"))
            {
                // stop
                marioBody.velocity = Vector2.zero;
            }

            if (Input.GetKeyDown("space") && onGroundState)
            {
                marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
                onGroundState = false;

                marioAnimator.SetBool("onGround", onGroundState);
            }
        }
    }

    public void RestartButtonCallback(int input)
    {
        // reset everything
        ResetGame();
        // resume time
        Time.timeScale = 1.0f;
    }

    private void ResetGame()
    {
        // reset position
        GameOverCanvas.gameObject.SetActive(false);
        GameplayUI.gameObject.SetActive(true);
        marioBody.transform.position = new Vector3(-7.46f, -3.44f, 0.0f);
        // reset sprite direction
        faceRightState = true;
        marioSprite.flipX = false;
        // reset score
        scoreText.text = "Score: 0";
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

        // reset Goomba
        foreach (Transform eachChild in enemies.transform)
        {
            eachChild.transform.localPosition = eachChild.GetComponent<EnemyMovement>().startPosition;
        }

        // reset boxes
        jumpOverGoomba.score = 0;

    }


    void PlayJumpSound()
    {
        // play jump sound
        marioAudio.PlayOneShot(marioAudio.clip);
    }

    void PlayDeathImpulse()
    {
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }
}