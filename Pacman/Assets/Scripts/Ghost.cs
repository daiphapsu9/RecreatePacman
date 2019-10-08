using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{

    public GameObject[] waypoints;
    int cur = 0;
    enum GhostType
    {
        Green, // move random at junction
        Pink, // move clock-wise around
        Blue, // run away
        Red, // chase
        Frightened
    }

    [SerializeField]
    private float speed;

    [SerializeField]
    private GhostType type;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        switch (type)
        {
            case GhostType.Pink:
                break;
            case GhostType.Blue:
                break;
            case GhostType.Red:
                break;
            case GhostType.Green:
                RandomMove();
                break;
            case GhostType.Frightened:
                break;
        }
    }
    private Vector2 headingDir;
    private Vector2 scan;
    private bool detection = false;

    private void FixedUpdate()
    {
    }

    public void RandomMove()
    {
        Waypoint nextWaypoint = waypoints[cur].GetComponent<Waypoint>();
        if (nextWaypoint.isTouched != true)
        {
            Debug.Log("move to: " + waypoints[cur].transform.position);
            //Vector3 p = Vector3.MoveTowards(transform.position,
            //                                waypoints[cur].transform.position,
            //speed);
            //GetComponent<Rigidbody2D>().MovePosition(p);
            headingDir = waypoints[cur].transform.position;
            transform.position = Vector3.MoveTowards(transform.position, headingDir, speed * Time.deltaTime);
        }
        // Waypoint reached, select next one
        else cur = (cur + 1) % waypoints.Length;
    }
    void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.SetColors(color, color);
        lr.SetWidth(0.1f, 0.1f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("OnCollisionEnter2D OnCollisionEnter2D");
        if (collision.gameObject.tag == "Waypoint")
        {
            Debug.Log("Ghost Ghost");
            collision.gameObject.GetComponent<Waypoint>().Touch();
        }

    }
}
