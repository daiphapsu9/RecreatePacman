using System.Collections;
using System.Collections.Generic;
using System.Windows;
using UnityEngine;

public class Pacman : ItemCollector
{
    public float speed;
    public Animator animator;
    [SerializeField]
    private AudioSource chompSound = null;
    [SerializeField]
    private AudioSource eatFruitSound = null;
    [SerializeField]
    public AudioSource deathSound = null;
    [SerializeField]
    public AudioSource eatGhostSound = null;
    [SerializeField]
    private ParticleSystem collisionParticle = null;
    [SerializeField]
    private bool isPlayer1 = true;
    [SerializeField]
    private GameObject[] oponents = null;
    // Start is called before the first frame update
    GameData gameData;

    void Start()
    {
        animator = GetComponent<Animator>();
        gameData = GameObject.Find("GameData").GetComponent<GameData>();
    }

    // Update is called once per frame
    override public void Update()
    {
        base.Update();
        Move();
    }

    // move and set appropriate animation
    void Move()
    {
        Vector3 movement;
        // In Classic and Innovative mode, user can user both A,S,D,W and arrow key to move
        if (gameData.currentMode != GameData.Mode.BattleMode)
        {
            // default input is A,S,W,D
            var inputMovement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);
            // if user us not using A,W,D,S then check arrows key for inputs
            if (inputMovement == Vector3.zero)
            {
                movement = new Vector3(Input.GetAxis("Horizontal2"), Input.GetAxis("Vertical2"), 0.0f);
            }
            else movement = inputMovement;
        }
        else
        {
            // in Battle mode, Player 1 will use A,S,D,W to move
            // player 2 will use arrow keys wo move
            if (isPlayer1) movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);
            else movement = new Vector3(Input.GetAxis("Horizontal2"), Input.GetAxis("Vertical2"), 0.0f);
        }
        animator.SetFloat("DirX", movement.x);
        animator.SetFloat("DirY", movement.y);
        animator.SetFloat("PreX", movement.x);
        animator.SetFloat("PreY", movement.y);
        animator.SetFloat("Magnitude", movement.magnitude);

        // if the sum of magnitude of movement is greater or equal 2, change it to sqr(0.5) ~= 0.7f
        if (Mathf.Abs(movement.x) + Mathf.Abs(movement.y) >= 2f)
        {
            movement.x *= (float)System.Math.Sqrt(0.5);
            movement.y *= (float)System.Math.Sqrt(0.5);
        }

        transform.position = transform.position + movement * GetSpeed() * Time.deltaTime;
    }

    // Handle on trigger event
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check if Pacman touches a portal
        if (collision.gameObject.tag == "Waypoint")
        {
            Waypoint waypoint = collision.gameObject.GetComponent<Waypoint>();
            // teleport to target portal
            if(waypoint.isPortal) // if pacman touches a portal, change position to destination portal
            {
                Vector2 targetPortalPosition = waypoint.targetPortal.GetComponent<Transform>().position;
                transform.position = targetPortalPosition;
            }
        }

        // consume balls when touch and play sound
        if (collision.gameObject.tag == "SmallBall")
        {
            if (!chompSound.isPlaying)
            {
                chompSound.Play();
            }
            gameData.ConsumeSmallBall(isPlayer1);
            Destroy(collision.gameObject);
        }

        // if pacman consume a big ball, all ghost will change into frightened mode
        if (collision.gameObject.tag == "BigBall")
        {
            GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");
            foreach (GameObject ghost in ghosts)
            {
                ghost.GetComponent<Ghost>().BecomeScared();
            }
            gameData.ConsumeBigBall(isPlayer1);
            Destroy(collision.gameObject);
        }

        // add points when consuming Fruits
        if (collision.gameObject.tag == "Fruit")
        {
            if (!eatFruitSound.isPlaying)
            {
                eatFruitSound.Play();
            }
            Fruit fruit = (Fruit)collision.gameObject.GetComponent<Fruit>();
            gameData.ConsumeFruit(fruit, isPlayer1);
        }

        // touch ghost
        if (collision.gameObject.tag == "Ghost")
        {
            Ghost ghost = collision.gameObject.GetComponent<Ghost>();
            // if the ghost is frightened, consume it to gain points and play sound
            if (ghost.mode == Ghost.GhostMode.Frightened)
            {
                if (!eatGhostSound.isPlaying)
                {
                    eatGhostSound.Play();
                }
                gameData.ConsumeGhost(isPlayer1);
                ghost.BecomeScatter();
            } else if(ghost.mode != Ghost.GhostMode.Scatter) // if the ghost is scattered, nothing happens, else pacman dies
            {
                if (!deathSound.isPlaying)
                {
                    deathSound.Play();
                }
                if (collisionParticle.gameObject.activeSelf == false)
                {
                    collisionParticle.gameObject.SetActive(true);
                }
                // play particle effect when pacman die
                collisionParticle.transform.position = gameObject.transform.position;
                collisionParticle.Play();
                gameObject.SetActive(false);
                gameData.isOver = true;
                // define winner in Battle mode if 1 player dies
                if (gameData.currentMode == GameData.Mode.BattleMode)
                {
                    gameData.gameResult = (isPlayer1) ? GameData.GameResult.Player2Win : GameData.GameResult.Player1Win;
                }
                else // in classic and innovative mode, play will lose
                {
                    gameData.gameResult = GameData.GameResult.Lose;
                }
            }
        }
    }

    // when pacman eat fruits (items may give special effect)
    public override void OnPickupItem(CollectableItem item)
    {
        // effect does apply in Classic mode
        if (gameData.currentMode == GameData.Mode.ClassicMode)
        {
            Object.Destroy(item.gameObject);
            return;
        }
        if (item.effect != null)
        {
            // if the fruit has special effect then apply it
            // if effect is a positive effect, apply to self 
            if (item.effect.type == EffectType.IncreaseSpeed)
            {
                appliedEffect = item.effect;
                appliedEffect.StartDurationCountDown();
            } else if (item.effect.type == EffectType.ReduceSpeed) // if effect is a negative effect, apply to oponents (Other pacman and ghosts) 
            {
                foreach (GameObject oponent in oponents)
                {
                    if (oponent.tag == "Pacman")
                    {
                        oponent.GetComponent<Pacman>().appliedEffect = item.effect;
                        oponent.GetComponent<Pacman>().appliedEffect.StartDurationCountDown();
                    }
                }
                gameData.AddEffectToGhosts(item.effect);
            }
        }
        Object.Destroy(item.gameObject);
    }

    float GetSpeed()
    {
        if (appliedEffect == null)
        {
            return speed;
        }
        switch (appliedEffect.type)
        {
            case EffectType.IncreaseSpeed:
                return speed + appliedEffect.value;
            case EffectType.ReduceSpeed:
                return speed - appliedEffect.value;
            case EffectType.Stun:
                animator.SetBool("Stun", true);
                return 0;
            default:
                return speed;
        }
    }
}
