using UnityEngine;
using System.Collections;

using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Animator anim;
    public int health = 100;
    public int maxHealth = 100;
    public bool showSpeed = false;
    public bool isCasting = false;
    public bool wasCasting = false;
    public SpriteRenderer sr;
    [Header("move settings")]
    public float moveSpeed = 5f;
    public float maxInputSpeed = 8f;
    private float horizontalInput;
    private Rigidbody2D rb;

    public int currentDungeonLayer = 0;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public bool isGrounded;

    public bool canCast = true;
    public float jumpForce = 10f;
    public bool canInputMove = true;
    public float upKnockback = 3f;

    public static PlayerController instance;

    [Header("snappy movement")]
    public float acceleration = 80f;
    public float deceleration = 100f;
    public float turnAcceleration = 140f;

    [Header("camera things")]
    public Unity.Cinemachine.CinemachineVirtualCamera vcam;
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

    [Header("i-frames")]
    public float invincibilityDuration = 1f;
    private bool isInvincible = false;
    
    private PlayerControls controls;

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

        controls = new PlayerControls();
    }
    void Update()
    {
        if (controls.Player.Pause.WasPressedThisFrame() && !UIManager.instance.gameIsPaused)
        {
            UIManager.instance.PauseSequence();
        }
        if (controls.Player.DropThrough.WasPressedThisFrame() && isGrounded && !isDroppingThrough && canInputMove)
        {
            StartCoroutine(DropThroughLedges());
        }
        if (currentInteractable != null && controls.Player.Interact.WasPressedThisFrame())
        {
            Debug.Log("great optimization good job cheve very smart");
            currentInteractable.Interact();
        }
        //Debug.Log(heldSeconds);
        // -1 = left, +1 = right
        horizontalInput = controls.Player.Move.ReadValue<float>();
        Debug.Log(horizontalInput);
        anim.SetInteger("hInput", (int)rb.linearVelocity.magnitude);
        if (horizontalInput != 0)
        {
            sr.flipX = horizontalInput < 0;
        }

        float horizontalSpeed = Mathf.Abs(rb.linearVelocity.x);
        if (showSpeed)
            Debug.Log(horizontalSpeed);

        if (controls.Player.Jump.WasPressedThisFrame() && isGrounded)
        {
            anim.SetTrigger("jump");
            // set velocity directly so jump height is uniform regardless of mass
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
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
        Vector2 velocity = rb.linearVelocity;

        if (horizontalInput != 0 && canInputMove)
        {
            float targetSpeed = horizontalInput * maxInputSpeed;

            float accelToUse = acceleration;

            // Extra snappy when switching directions
            if (Mathf.Sign(velocity.x) != Mathf.Sign(horizontalInput) && Mathf.Abs(velocity.x) > 0.1f)
            {
                accelToUse = turnAcceleration;
            }

            velocity.x = Mathf.MoveTowards(
                velocity.x,
                targetSpeed,
                accelToUse * Time.fixedDeltaTime
            );

            rb.linearVelocity = velocity;
        }
        else if (isGrounded)
        {
            velocity.x = Mathf.MoveTowards(
                velocity.x,
                0f,
                deceleration * Time.fixedDeltaTime
            );

            rb.linearVelocity = velocity;
        }

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
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
            Debug.Log("player hit by something");
            TakeHit(hit.damage, hit.knockbackForce, collision.transform.position);
            anim.SetTrigger("hurt");
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
        //Vector2 knockDir = ((Vector2)transform.position - hitSourcePos).normalized;
        Vector2 knockDir = Vector2.left;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockDir * 15f, ForceMode2D.Impulse);
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
        isInvincible = true;
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
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        controls.Player.Disable();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        transform.position = Vector3.zero;
        vcam = FindFirstObjectByType<Unity.Cinemachine.CinemachineVirtualCamera>();
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}