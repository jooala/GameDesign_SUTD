using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float normalOffset = 5.0f;
    public float chaseOffset = 10.0f;
    public float enemyPatrolTime = 2.0f;
    public float jumpForce = 10f;
    public float attackForce = 10f;
    [Header("Score")]
    public int scoreAmount = 1;

    [Header("References")]
    public Transform jumpTarget;
    public GameObject lampObject;
    public Animator goombaAnimator;
    public GameManager gameManager;


    private Vector2 velocity;
    private Rigidbody2D enemyBody;
    private Vector2 lastSawMarioposition;
    private SpriteRenderer lamp;
    private Transform lampLight;
    private Coroutine explodeCoroutine;

    private float originalX;
    private Vector3 initialPosition;
    private int moveRight = -1;
    private bool walking = true;
    private bool isJumping = false;
    private float maxChaseX;
    private float minChaseX;

    private float explosionDelay = 1f;
    [SerializeField]
    private bool isFacingRight = false;

    private enum GoombaState
    {
        Normal,
        Chase
    }

    private GoombaState currentState = GoombaState.Normal;
    private float chaseTimer = 0f;

    private void Start()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        originalX = transform.position.x;
        initialPosition = transform.position;
        lampLight = lampObject.transform.Find("light");
        ComputeVelocity();
        CalculateChaseBounds();
    }

    private void ComputeVelocity()
    {
        if (currentState == GoombaState.Normal)
        {
            velocity = new Vector2((moveRight) * normalOffset / enemyPatrolTime, 0);
        }
        else if (currentState == GoombaState.Chase)
        {
            velocity = new Vector2((moveRight) * chaseOffset / enemyPatrolTime, 0);
        }
    }

    private void MoveGoomba()
    {
        enemyBody.MovePosition(enemyBody.position + velocity * Time.fixedDeltaTime);
    }

    private void Update()
    {
        switch (currentState)
        {
            case GoombaState.Normal:
                if (Mathf.Abs(enemyBody.position.x - originalX) < normalOffset)
                {
                    if (walking)
                    {
                        MoveGoomba();
                    }
                }
                else
                {
                    if (walking)
                    {
                        moveRight *= -1;
                        ComputeVelocity();
                        MoveGoomba();
                    }
                }
                FlipGoomba(moveRight > 0);
                break;

            case GoombaState.Chase:
                GoombaChase();
                if (Mathf.Abs(enemyBody.position.x - originalX) >= chaseOffset)
                {
                    moveRight *= -1;
                    ComputeVelocity();
                }
                MoveGoomba();
                break;
        }

        if (currentState == GoombaState.Chase)
        {
            chaseTimer += Time.deltaTime;
            if (enemyBody.position.x > maxChaseX || enemyBody.position.x < minChaseX)
            {
                ReturnToNormalState();
            }
        }
    }

    public void GameRestart()
    {
        ResetEnemy();
        ResetLamp();
    }

    private void ResetEnemy()
    {
        transform.localPosition = initialPosition;
        moveRight = -1;
        isFacingRight = false;
        walking = true;
        goombaAnimator.SetTrigger("gameOver");
        originalX = initialPosition.x;
        GetComponent<EdgeCollider2D>().enabled = true;
        enemyBody.bodyType = RigidbodyType2D.Dynamic;
        enemyBody.simulated = true;
        gameObject.SetActive(true);
        ComputeVelocity();
        currentState = GoombaState.Normal;
    }

    private void ResetLamp()
    {
        if (lampObject != null)
        {
            Transform childTransform = lampObject.transform;
            childTransform.SetParent(gameObject.transform);
            lampObject.SetActive(false);
            childTransform.localPosition = new Vector3(-0.09f, -0.26f, 0f);
            childTransform.localRotation = Quaternion.identity;
            lampObject.GetComponent<Rigidbody2D>().simulated = false;
            PolygonCollider2D childLight = lampLight.GetComponent<PolygonCollider2D>();
            childLight.enabled = true;
            Transform secondChildTransform = childTransform.GetChild(1);
            secondChildTransform.gameObject.SetActive(true);
            lampObject.SetActive(true);
        }
    }

    private void GoombaAlerted()
    {
        StartCoroutine(AlertedAndChase());
    }

    private IEnumerator AlertedAndChase()
    {
        enemyBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        walking = false;
        goombaAnimator.SetBool("isChasing", true);
        yield return new WaitForSeconds(0.6f);
        currentState = GoombaState.Chase;
    }

    public void GoombaDead()
    {
        goombaAnimator.Play("goomba-dead");
        gameManager.GoombaStomped(scoreAmount);
        GetComponent<EdgeCollider2D>().enabled = false;
        enemyBody.bodyType = RigidbodyType2D.Kinematic;
        enemyBody.simulated = false;
        DropLampOnGround();
    }

    public void DropLampOnGround()
    {
        if (lampObject != null)
        {
            Transform childTransform = lampObject.transform;
            childTransform.SetParent(null);
            lampObject.SetActive(true);
            lampObject.GetComponent<Rigidbody2D>().simulated = true;
            PolygonCollider2D childLight = lampLight.GetComponent<PolygonCollider2D>();
            Transform secondChildTransform = childTransform.GetChild(1);
            secondChildTransform.gameObject.SetActive(false);
            childLight.enabled = false;
        }
    }

    private void GoombaAttack()
    {
        if (!isJumping)
        {
            walking = false;
            isJumping = true;
            Vector2 jumpDirection = (jumpTarget.position - transform.position);
            enemyBody.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
            isJumping = false;
            if (explodeCoroutine != null)
            {
                StopCoroutine(explodeCoroutine);
            }
            explodeCoroutine = StartCoroutine(ExplodeAfterDelay());
        }
    }

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(explosionDelay);
        ExplodeGoomba();
    }

    private void ExplodeGoomba()
    {
        gameObject.SetActive(false);
    }

    private void GoombaChase()
    {
        currentState = GoombaState.Chase;
        Vector2 directionToMario = (jumpTarget.position - transform.position).normalized;
        float chaseSpeed = 5f;
        velocity = new Vector2(directionToMario.x * chaseSpeed, 0);
        lastSawMarioposition = transform.position;

        if (Mathf.Abs(enemyBody.position.x - originalX) >= chaseOffset)
        {
            ReturnToNormalState();
        }
        if (velocity.x > 0)
        {
            FlipGoomba(true);
        }
        else if (velocity.x < 0)
        {
            FlipGoomba(false);
        }
    }
    private void FlipGoomba(bool isFacingRight)
    {
        isFacingRight = !isFacingRight;
        Vector3 newScale = transform.localScale;
        newScale.x = isFacingRight ? -1 : 1;
        transform.localScale = newScale;
    }
    private void ReturnToNormalState()
    {
        transform.position = lastSawMarioposition;
        originalX = transform.position.x;
        normalOffset = 4F;
        enemyPatrolTime = 2;
        ComputeVelocity();
        currentState = GoombaState.Normal;
        walking = true;
        chaseTimer = 0f;
    }

    private void CalculateChaseBounds()
    {
        maxChaseX = originalX + chaseOffset;
        minChaseX = originalX - chaseOffset;
    }
}
