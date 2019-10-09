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
    void Start()
    {
        animator = GetComponent<Animator>();
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

        transform.position = transform.position + movement * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "SmallBall")
        {
            if (!chompSound.isPlaying)
            {
                chompSound.Play();
            }
            if (collisionParticle.gameObject.activeSelf == false)
            {
                collisionParticle.gameObject.SetActive(true);
            }
            //collisionParticle.transform.position = gameObject.transform.position;
            //collisionParticle.Play();
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "Ghost")
        {
            if (!deathSound.isPlaying)
            {
                deathSound.Play();
            }
            //if (collisionParticle.gameObject.activeSelf == false) 
            //{
            //    collisionParticle.gameObject.SetActive(true);
            //}
            collisionParticle.transform.position = gameObject.transform.position;
            collisionParticle.Play();
            Destroy(this.gameObject);
            //MessageBox.Show("Thua nha co ho", "Thua", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            Debug.Log("OnTriggerEnter2DOnTriggerEnter2DOnTriggerEnter2DOnTriggerEnter2DOnTriggerEnter2D");
        }

    }


    
}
