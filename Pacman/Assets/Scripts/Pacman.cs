using System.Collections;
using System.Collections.Generic;
using System.Windows;
using UnityEngine;

public class Pacman : MonoBehaviour
{
    public float speed;
    public Animator animator;
    Vector2 dest = Vector2.zero;
    [SerializeField]
    private AudioSource chompSound;
    [SerializeField]
    private AudioSource deathSound;
    [SerializeField]
    private ParticleSystem collisionParticle;
    // Start is called before the first frame update
    MainManager manager;
    void Start()
    {
        animator = GetComponent<Animator>();
        manager = GameObject.Find("Manager").GetComponent<MainManager>();
    }

    // Update is called once per frame
    void Update()
    {


        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);
        animator.SetFloat("DirX", movement.x);
        animator.SetFloat("DirY", movement.y);
        animator.SetFloat("PreX", movement.x);
        animator.SetFloat("PreY", movement.y);
        animator.SetFloat("Magnitude", movement.magnitude);

        if (Mathf.Abs(movement.x) + Mathf.Abs(movement.y) >= 2f)
        {
            movement.x *= (float)System.Math.Sqrt(0.5);
            movement.y *= (float)System.Math.Sqrt(0.5);
        }

        //int layer = 1 << 9;
        //var lowerPosition = new Vector2(transform.position.x, transform.position.y-0.1f);
        //var upperPosition = new Vector2(transform.position.x, transform.position.y + 0.1f);


        //var direction = transform.TransformDirection(movement);
        //if (movement.x == 0)
        //{
        //    lowerPosition = new Vector2(transform.position.x - 0.1f, transform.position.y);
        //    upperPosition = new Vector2(transform.position.x + 0.1f, transform.position.y);
        //}

        //Debug.Log("movement == " + transform.TransformDirection(movement));

        //Debug.DrawRay(transform.position, direction, Color.red);
        //Debug.DrawRay(upperPosition, direction, Color.red);
        //Debug.DrawRay(lowerPosition, direction, Color.red);

        //RaycastHit2D hit1 = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, layer);
        //RaycastHit2D hit2 = Physics2D.Raycast(upperPosition, direction, Mathf.Infinity, layer);
        //RaycastHit2D hit3 = Physics2D.Raycast(lowerPosition, direction, Mathf.Infinity, layer);
        //RaycastHit2D[] hits = { hit1, hit2, hit3};
        //bool isBlocked = false;
        //foreach (var hit in hits)
        //{
        //    if (hit)
        //    {
        //        if (hit.collider)
        //        {
        //            if (hit.distance <= 0.14)
        //            {
        //                isBlocked = true;
        //            }
        //        }

        //    }
        //}
        //if (!isBlocked)
            transform.position = transform.position + movement * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Waypoint")
        {
            Waypoint waypoint = collision.gameObject.GetComponent<Waypoint>();
            // teleport to target portal
            if(waypoint.isPortal)
            {
                Vector2 targetPortalPosition = waypoint.targetPortal.GetComponent<Transform>().position;
                transform.position = targetPortalPosition;
            }
        }

        if (collision.gameObject.tag == "SmallBall")
        {
            if (!chompSound.isPlaying)
            {
                chompSound.Play();
            }
            manager.ConsumeSmallBall();
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "BigBall")
        {
            GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");
            foreach (GameObject ghost in ghosts)
            {
                ghost.GetComponent<Ghost>().BecomeScared();
            }
            manager.ConsumeBigBall();
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "Fruit")
        {
            if (!chompSound.isPlaying)
            {
                chompSound.Play();
            }
            Fruit fruit = (Fruit)collision.gameObject.GetComponent<Fruit>();
            manager.ConsumeFruit(fruit);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "Ghost")
        {
            Ghost ghost = collision.gameObject.GetComponent<Ghost>();
            if (ghost.mode == Ghost.GhostMode.Frightened)
            {
                manager.ConsumeGhost();
                ghost.BecomeScatter();
            } else if(ghost.mode != Ghost.GhostMode.Scatter)
            {
                if (!deathSound.isPlaying)
                {
                    deathSound.Play();
                }
                if (collisionParticle.gameObject.activeSelf == false)
                {
                    collisionParticle.gameObject.SetActive(true);
                }
                collisionParticle.transform.position = gameObject.transform.position;
                collisionParticle.Play();
                //Destroy(this.gameObject);
                gameObject.SetActive(false);
            }
        }

    }


    
}
