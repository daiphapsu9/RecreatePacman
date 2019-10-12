using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : ItemCollector
{
    enum GhostType
    {
        Green, // move random at junction
        Pink, // move clock-wise around
        Blue, // run away
        Red, // chase
    }

    public enum GhostMode
    {
        Normal,
        Scatter,
        Frightened
    }
    public enum MoveMode
    {
        Chase,
        RunAway,
        Random,
        Path
    }

    public Waypoint startingPoint;

    [SerializeField]
    private float speed;

    [SerializeField]
    private GhostType type;
    [SerializeField]
    private MoveMode moveMode;
    public GhostMode mode;

    public GameObject[] waypoints;
    private Waypoint currentWaypoint, previousWaypoint, targetWaypoint;
    private Vector2 direction, nextDirection;
    private GameObject pacman;
    int cur = 0;
    public Animator animator;

    float scaredDuration = 7;
    DateTime scaredStartingTime;

    float scatterDuration = 7;
    DateTime scatterStartingTime;

    private Vector2 headingDir;
    private Vector2 scan;
    private Vector2 lookingDir;
    GameData gameData;
    void Start()
    {
        pacman = GameObject.FindGameObjectWithTag("Pacman");
        //Waypoint startingWaypoint = waypoints[0].GetComponent<Waypoint>();
        mode = GhostMode.Normal;
        Vector2 pacmanPos = pacman.transform.position;
        targetWaypoint = startingPoint;
        direction = Vector2.up;
        previousWaypoint = startingPoint;
        gameData = GameObject.Find("GameData").GetComponent<GameData>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        CheckMode(); // check and reset ghost mode
        ChangeAnim();
    }


    private void FixedUpdate()
    {
    }

    public void Move()
    {

        if (targetWaypoint != currentWaypoint && targetWaypoint != null)
        {
            //Debug.Log("Move 11111");
            if (nextDirection == direction *-1)
            {
                //Debug.Log("Move 2222");
                direction *= -1;
                Waypoint temp = targetWaypoint;
                targetWaypoint = previousWaypoint;
                previousWaypoint = temp;
            }

            if (isOverShot())
            {
                //Debug.Log("Move 333");
                currentWaypoint = targetWaypoint;
                transform.position = currentWaypoint.transform.position;
                targetWaypoint = ChooseNextWaypoint();
                previousWaypoint = currentWaypoint;
                currentWaypoint = null;
            } else
            {
                //Debug.Log("Move 444");
                MoveToWaypoint(targetWaypoint);
            }
        }
    }

    public void MoveToWaypoint(Waypoint waypoint)
    {
        //Debug.Log("MoveToWaypoint");
        headingDir = waypoint.transform.position;
        lookingDir = waypoint.transform.position - transform.position;
        transform.position = Vector3.MoveTowards(transform.position, headingDir, GetSpeed() * Time.deltaTime);
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
        if (mode == GhostMode.Scatter)
        {
            targetPosition = startingPoint.transform.position;
        }
        if (mode == GhostMode.Normal)
        {
            targetPosition = pacmanPos; 
        }
        if (moveMode == MoveMode.Path)
        {
            if (waypoints.Length >= 1)
            {
                //Waypoint nextWaypoint = waypoints[cur].GetComponent<Waypoint>();
                targetPosition = waypoints[cur].transform.position;
                // Waypoint reached, select next one
                cur = (cur + 1) % waypoints.Length;
            }
        }

        Waypoint moveToWaypoint = null;
        ArrayList foundWaypoints = new ArrayList();
        Vector2[] foundWaypointDirection = new Vector2[4];
        int wpCount = 0;

        for (int i = 0; i < currentWaypoint.neighbors.Length; i++)
        {
            if(currentWaypoint.validDirections[i] != direction * (-1) || currentWaypoint.neighbors.Length == 1)
            {
                foundWaypoints.Add(currentWaypoint.neighbors[i]);
                foundWaypointDirection[wpCount] = currentWaypoint.validDirections[i];
                wpCount++;
            }
            //Debug.Log("333 foundWaypointDirection  " + foundWaypointDirection.Length);
        }

        if(foundWaypoints.Count == 1)
        {
            moveToWaypoint = (Waypoint)foundWaypoints[0];
            direction = foundWaypointDirection[0];
        }

        if (foundWaypoints.Count > 1)
        {

            if (moveMode == MoveMode.RunAway || mode == GhostMode.Frightened) // run away
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
            } else if (moveMode == MoveMode.Chase || moveMode == MoveMode.Path || mode == GhostMode.Scatter) // Chase target position or go back to base
            {
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
            } else if (moveMode == MoveMode.Random) // Random turn
            { 
                //Debug.Log("555 available  ==" + foundWaypoints.Count);
                var random = UnityEngine.Random.Range(0, foundWaypoints.Count);
                //Debug.Log("666 ChooseNextWaypoint  random ==" + random);
                if (foundWaypointDirection[random] != Vector2.zero)
                {
                    moveToWaypoint = (Waypoint)foundWaypoints[random];
                    direction = foundWaypointDirection[random];
                }
            }
        }
        return moveToWaypoint;
    }

    public void BecomeScared()
    {
        mode = GhostMode.Frightened;
        scaredStartingTime = DateTime.Now;
    }

    public void BecomeScatter()
    {
        mode = GhostMode.Scatter;
        scatterStartingTime = DateTime.Now;
        cur = 0;
    }

    private void CheckMode()
    {
        if (mode == GhostMode.Frightened)
        {
            TimeSpan time = DateTime.Now - scaredStartingTime;
            if (time.Seconds >= scaredDuration)
            {
                gameData.ResetMultiplier();
                mode = GhostMode.Normal;
            }
        }

        if (mode == GhostMode.Scatter)
        {
            TimeSpan time = DateTime.Now - scatterStartingTime;
            if (time.Seconds >= scatterDuration)
            {
                mode = GhostMode.Normal;
            }
        }
    }

    private void ChangeAnim()
    {
        if (mode == GhostMode.Frightened && animator.GetBool("Frightened") != true)
        {
            animator.SetBool("Frightened", true);
        }

        if (mode == GhostMode.Normal && animator.GetBool("Frightened") != false)
        {
            animator.SetBool("Frightened", false);
        }

        if (mode == GhostMode.Scatter && animator.GetBool("Scatter") != true)
        {
            animator.SetBool("Scatter", true);
        }

        if (mode == GhostMode.Normal && animator.GetBool("Scatter") != false)
        {
            animator.SetBool("Scatter", false);
        }

        animator.SetFloat("DirX", lookingDir.x);
        animator.SetFloat("DirY", lookingDir.y);
    }

    float GetSpeed()
    {
        if (mode == GhostMode.Frightened)
        {
            return speed / 2;
        }
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

    public override void OnPickupItem(CollectableItem item)
    {
        if (gameData.currentMode == GameData.Mode.ClassicMode)
        {
            return;
        }
        if (item.effect != null)
        {
            if (item.effect.type == EffectType.IncreaseSpeed)
            {
                gameData.AddEffectToGhosts(item.effect);
            }
            else if (item.effect.type == EffectType.ReduceSpeed)
            {
                gameData.AddEffectToPacman(item.effect);
            }
        }
    }
}
