using UnityEngine;
using System.Collections;
using Cinemachine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public int health = 100;
    public int maxHealth = 100;
    public bool showSpeed = false;
    public bool isCasting = false;
    public SpriteRenderer sr;
    [Header("move settings")]
    public float moveSpeed = 5f;
    public float groundFriction = 20f;
    public float maxInputSpeed = 8f;
    private float horizontalInput;
    private Rigidbody2D rb;

    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public bool isGrounded;

    public bool canCast = true;
    public float jumpForce = 10f;
    public bool canInputMove = true;
    public float upKnockback = 3f;

    public static PlayerController instance;

    [Header("camera things")]
    public CinemachineVirtualCamera vcam;
    public float normalSize = 15f;
    public float zoomedSize = 13f;
    public float zoomInSpeed = 8f;
    public float zoomOutSpeed = 5f;
    public GloveSpells gloveSpells;
    public ManaResource manaResource;
    
    [Header("experience")]
    public int currentXp = 0;
    public int currentLevel = 1;
    public int xpForNextLevel = 6;
    
    [Header("ledges")]
    public float dropThroughTime = 0.25f;
    private bool isDroppingThrough = false;
    private Collider2D playerCol;

    [Header("death and dying")]
    public float slowDuration = 2f;
    public GameObject deathPanel;
    public bool canTakeDamage = true;
    
    [Header("i-frames")]
    public float invincibilityDuration = 0.4f;
    private bool isInvincible = false;
    
    private void Awake()
    {
        
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        playerCol = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        if (manaResource == null)
            manaResource = GetComponent<ManaResource>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !UIManager.instance.gameIsPaused)
        {
            UIManager.instance.PauseSequence();
        }
        if (Input.GetKeyDown(KeyCode.S) && isGrounded && !isDroppingThrough)
        {
            StartCoroutine(DropThroughLedges());
        }
        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("great optimization good job cheve very smart");
            currentInteractable.Interact();
        }
        //Debug.Log(heldSeconds);
        // -1 = left, +1 = right
        horizontalInput = Input.GetAxisRaw("Horizontal");

        
        float horizontalSpeed = Mathf.Abs(rb.linearVelocity.x);
        if (showSpeed)
            Debug.Log(horizontalSpeed);

        if (Input.GetKeyDown("space") && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
 
        if (gloveSpells != null)
            gloveSpells.HandleInput();
        {
            bool isHolding = false;
            if (manaResource != null)
                isHolding = manaResource.isHolding;

            float targetSize = isHolding ? zoomedSize : normalSize;

            // choose speed based on direction (returning vs zooming in)
            float speed = (targetSize > vcam.m_Lens.OrthographicSize) ? zoomOutSpeed : zoomInSpeed;

            vcam.m_Lens.OrthographicSize = Mathf.MoveTowards(
                vcam.m_Lens.OrthographicSize,
                targetSize,
                Time.unscaledDeltaTime * speed
            );
        }
    }

    public void GainExperience(int experience)
    {
        currentXp += experience;
        Debug.Log("new experience: " + experience);
        Debug.Log("new experienceForNextLevel: " + xpForNextLevel);
        
        if (currentXp >= xpForNextLevel)
        {
            Level();
        }
    }
    public void Level()
    {
        currentLevel++;
        currentXp -= xpForNextLevel;
        xpForNextLevel += 6 + (currentLevel - 1) * 3;
        UIManager.instance.LevelUpSequence();
    }

    private void FixedUpdate()
    {
        float vx = rb.linearVelocity.x;

        if (horizontalInput != 0)
        {

            bool pushingRightAllowed = horizontalInput > 0f && vx < maxInputSpeed;
            bool pushingLeftAllowed = horizontalInput < 0f && vx > -maxInputSpeed;

            if ((pushingRightAllowed || pushingLeftAllowed) && canInputMove)
            {
                rb.AddForce(Vector2.right * horizontalInput * moveSpeed, ForceMode2D.Force);
            }

        }
        else if (isGrounded)
        {
            Vector2 velocity = rb.linearVelocity;
            velocity.x = Mathf.MoveTowards(
                velocity.x,
                0f,
                groundFriction * Time.fixedDeltaTime
            );
            rb.linearVelocity = velocity;
        }

        isGrounded = Physics2D.OverlapCircle(
        groundCheck.position,
        groundCheckRadius,
        groundLayer);

    }
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
    
    private Interactable currentInteractable;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyBullet hit = collision.GetComponent<EnemyBullet>();
        if (hit != null)
        {
            Debug.Log("Player hit by something");
            TakeHit(hit.damage, hit.knockbackForce, collision.transform.position);
            if (hit.stunTime > 0)
                StartCoroutine(StunCoroutine(hit.stunTime));
            Destroy(hit.gameObject, .1f);
        }
        currentInteractable = collision.gameObject.GetComponent<Interactable>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Portal>() == currentInteractable)
        {
            currentInteractable = null;
        }
    }
    public void TakeHit(int damage, float knockbackForce, Vector2 hitSourcePos)
    {
        if (isInvincible)
            return;
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
        StartCoroutine(InvincibilityCoroutine());
        Vector2 knockDir = ((Vector2)transform.position - hitSourcePos).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockDir * knockbackForce, ForceMode2D.Impulse);
        rb.AddForce(Vector2.up * upKnockback, ForceMode2D.Impulse);
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        float flashInterval = 0.08f;
        float timer = 0f;
        while (timer < invincibilityDuration)
        {
            sr.enabled = !sr.enabled;

            yield return new WaitForSecondsRealtime(flashInterval);
            timer += flashInterval;
        }

        sr.enabled = true;
        isInvincible = false;
    }

    public void Die()
    {
        canInputMove = false;
        canCast = false;
        UIManager.instance.DieSequence();
    }

    public void StunRoutineWrapper(float stunTime)
    {
        StartCoroutine(StunCoroutine(stunTime));
    }
    private IEnumerator StunCoroutine(float stunTime)
    {
        canInputMove = false;
        canCast = false;
        yield return new WaitForSeconds(stunTime);
        canInputMove = true;
        canCast = true;
    }

    private IEnumerator DropThroughLedges()
    {
        Collider2D ledgeCol = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius + 0.05f,
            LayerMask.GetMask("Ledges")
        );

        if (ledgeCol == null)
        {
            Debug.Log("noledge collider");
            yield break;
        }
        
        Debug.Log("ignoring collision between " + playerCol.name + " and " + ledgeCol.name);
        isDroppingThrough = true;

        Physics2D.IgnoreCollision(playerCol, ledgeCol, true);

        yield return new WaitForSeconds(dropThroughTime);

        Physics2D.IgnoreCollision(playerCol, ledgeCol, false);

        isDroppingThrough = false;
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        transform.position = Vector3.zero;
    }
    
    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}