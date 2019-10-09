using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    enum GhostType
    {
        Green, // move random at junction
        Pink, // move clock-wise around
        Blue, // run away
        Red, // chase
    }

    enum GhostMode
    {
        Normal,
        Scatter,
        Frightened
    }

    public Waypoint startingPoint;

    [SerializeField]
    private float speed;

    [SerializeField]
    private GhostType type;


    public GameObject[] waypoints;
    private Waypoint currentWaypoint, previousWaypoint, targetWaypoint;
    private Vector2 direction, nextDirection;
    private GameObject pacman;
    int cur = 0;
    public Animator animator;

    float scatterDuration = 7;


    private Vector2 headingDir;
    private Vector2 scan;
    void Start()
    {
        pacman = GameObject.FindGameObjectWithTag("Pacman");
        //Waypoint startingWaypoint = waypoints[0].GetComponent<Waypoint>();

        Vector2 pacmanPos = pacman.transform.position;
        targetWaypoint = startingPoint;
        direction = Vector2.up;
        previousWaypoint = startingPoint;
        //if (startingWaypoint != null)
        //{
        //    MoveToWaypoint(startingWaypoint);
        //    currentWaypoint = startingWaypoint;
        //    previousWaypoint = currentWaypoint;
        //}
        animator.SetBool("Frightened", true);
    }

    // Update is called once per frame
    void Update()
    {
        switch (type)
        {
            case GhostType.Pink:
                ChaseMove();
                break;
            case GhostType.Blue:
                ChaseMove();
                break;
            case GhostType.Red:
                ChaseMove();
                break;
            case GhostType.Green:
                //RandomMove();
                ChaseMove();
                break;
        }
    }


    private void FixedUpdate()
    {
    }

    public void RandomMove()
    {
        Waypoint nextWaypoint = waypoints[cur].GetComponent<Waypoint>();
        if (nextWaypoint.isTouched != true)
        {
            Debug.Log("move to: " + waypoints[cur].transform.position);
            MoveToWaypoint(nextWaypoint);
        }
        // Waypoint reached, select next one
        else cur = (cur + 1) % waypoints.Length;
    }

    public void ChaseMove()
    {

        if (targetWaypoint != currentWaypoint && targetWaypoint != null)
        {
            //Debug.Log("ChaseMove 11111");
            if (nextDirection == direction *-1)
            {
                //Debug.Log("ChaseMove 2222");
                direction *= -1;
                Waypoint temp = targetWaypoint;
                targetWaypoint = previousWaypoint;
                previousWaypoint = temp;
            }

            if (isOverShot())
            {
                currentWaypoint = targetWaypoint;
                transform.localPosition = currentWaypoint.transform.position;
                targetWaypoint = ChooseNextWaypoint();
                previousWaypoint = currentWaypoint;
                currentWaypoint = null;
            } else
            {
                MoveToWaypoint(targetWaypoint);
            }
        }
    }

    public void MoveToWaypoint(Waypoint waypoint)
    {
        //Debug.Log("MoveToWaypoint");
        headingDir = waypoint.transform.position;
        transform.position = Vector3.MoveTowards(transform.position, headingDir, speed * Time.deltaTime);
    }

    //void ChangePosition (Vector2 d)
    //{
    //    if (d != direction)
    //    {
    //        nextDirection = d;
    //    }

    //    if (currentWaypoint != null)
    //    {
    //        Waypoint moveToWaypoint = CanMove(d);
    //        if (moveToWaypoint != null)
    //        {
    //            direction = d;
    //            targetWaypoint = moveToWaypoint;
    //            previousWaypoint = currentWaypoint;
    //            currentWaypoint = null;
    //        }
    //    }
    //}

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
        //Debug.Log("OnCollisionEnter2D OnCollisionEnter2D");
        if (collision.gameObject.tag == "Waypoint")
        {
            collision.gameObject.GetComponent<Waypoint>().Touch();
        }

    }
    
    float LengthFromWaypoint(Vector2 targetPosition)
    {
        Vector2 vec = targetPosition - (Vector2)previousWaypoint.transform.position;
        return vec.magnitude;
    }

    bool isOverShot()
    {
        float waypointToTarget = LengthFromWaypoint(targetWaypoint.transform.position);
        float nodeToSelf = LengthFromWaypoint(transform.position);
        //Debug.Log("waypointToTarget == " + waypointToTarget + " || nodeToSelf == " + nodeToSelf);
        return nodeToSelf >= waypointToTarget;
    }

    float GetDistance (Vector2 posA, Vector2 posB)
    {
        float dx = posA.x - posB.x;
        float dy = posA.y - posB.y;
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    Waypoint ChooseNextWaypoint()
    {
        //Debug.Log("ChooseNextWaypoint ChooseNextWaypoint");
        Vector2 pacmanPos = pacman.transform.position;
        Vector2 targetPosition = pacmanPos;

        Waypoint moveToWaypoint = null;
        ArrayList foundWaypoints = new ArrayList();
        Vector2[] foundWaypointDirection = new Vector2[4];
        int wpCount = 0;
        //Debug.Log("2222 ChooseNextWaypoint  " + currentWaypoint.neighbors.Length);
        for (int i = 0; i < currentWaypoint.neighbors.Length; i++)
        {
            if(currentWaypoint.validDirections[i] != direction * (-1))
            {
                foundWaypoints.Add(currentWaypoint.neighbors[i]);
                foundWaypointDirection[wpCount] = currentWaypoint.validDirections[i];
                wpCount++;
            }
        }

        if(foundWaypoints.Count == 1)
        {
            //Debug.Log("333 ChooseNextWaypoint  ");
            moveToWaypoint = (Waypoint)foundWaypoints[0];
            direction = foundWaypointDirection[0];
        }

        if (foundWaypoints.Count > 1)
        {
            if (type == GhostType.Red) // Chase Pacman
            {
                //Debug.Log("4444 ChooseNextWaypoint  ");
                float leastDis = 10000f;
                for (int i = 0; i < foundWaypoints.Count; i++)
                {
                    if (foundWaypointDirection[i] != Vector2.zero)
                    {
                        float distance = GetDistance(((Waypoint)foundWaypoints[i]).transform.position, targetPosition);
                        //Debug.Log("555 ChooseNextWaypoint distance === " + distance);
                        if (distance < leastDis)
                        {
                            //Debug.Log("666 ChooseNextWaypoint  ");
                            leastDis = distance;
                            moveToWaypoint = (Waypoint)foundWaypoints[i];
                            direction = foundWaypointDirection[i];
                        }
                    }
                }
            }

            if (type == GhostType.Green || type == GhostType.Pink) // Random turn
            { 
                Debug.Log("555 available  ==" + foundWaypoints.Count);
                var random = Random.Range(0, foundWaypoints.Count);
                Debug.Log("666 ChooseNextWaypoint  random ==" + random);
                if (foundWaypointDirection[random] != Vector2.zero)
                {
                    moveToWaypoint = (Waypoint)foundWaypoints[random];
                    direction = foundWaypointDirection[random];
                }
            }

            if (type == GhostType.Blue) // run away
            {
                //Debug.Log("4444 ChooseNextWaypoint  ");
                float farthest = 0;
                for (int i = 0; i < foundWaypoints.Count; i++)
                {
                    if (foundWaypointDirection[i] != Vector2.zero)
                    {
                        float distance = GetDistance(((Waypoint)foundWaypoints[i]).transform.position, targetPosition);
                        //Debug.Log("555 ChooseNextWaypoint distance === " + distance);
                        if (distance > farthest)
                        {
                            //Debug.Log("666 ChooseNextWaypoint  ");
                            farthest = distance;
                            moveToWaypoint = (Waypoint)foundWaypoints[i];
                            direction = foundWaypointDirection[i];
                        }
                    }
                }
            }

            //if (type == GhostType.Pink) // turn clock-wise TODO: not working
            //{
            //    for (int i = 0; i < foundWaypoints.Count; i++)
            //    {
            //        if (foundWaypointDirection[i].x > 0)
            //        {
            //            //Debug.Log("555 ChooseNextWaypoint distance === " + distance);
            //            moveToWaypoint = (Waypoint)foundWaypoints[i];
            //            direction = foundWaypointDirection[i];
            //            return moveToWaypoint;
            //        } else if (foundWaypointDirection[i].y > 0)
            //        {
            //            moveToWaypoint = (Waypoint)foundWaypoints[i];
            //            direction = foundWaypointDirection[i];
            //            return moveToWaypoint;
            //        } else if (foundWaypointDirection[i].y < 0)
            //        {
            //            moveToWaypoint = (Waypoint)foundWaypoints[i];
            //            direction = foundWaypointDirection[i];
            //            return moveToWaypoint;
            //        } else
            //        {
            //            moveToWaypoint = (Waypoint)foundWaypoints[i];
            //            direction = foundWaypointDirection[i];
            //            return moveToWaypoint;
            //        }
            //    }
            //}
        }
        return moveToWaypoint;
    }
}
