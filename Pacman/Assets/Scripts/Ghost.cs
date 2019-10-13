using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : ItemCollector
{
    public enum GhostMode
    {
        Normal, // Ghost at Normal mode will move with their move mode
        Scatter, // scattered ghost will move back to their starting point
        Frightened // frightened ghosts will move away from nearest pacman, speed are halved
    }
    public enum MoveMode
    {
        Chase, // chase pacman or move to targeting position
        RunAway, // run away from pacmans
        Random, // random turn
        Path // move following a pre-define path (use for Pink ghost and ghost in Menu Scene)
    }

    public Waypoint startingPoint; // starting position

    // Ghost will have their base speed set by default which is 1.8f
    // their actual speed is adjusted based on their mode and applied effects (increased or reduced by 0.5 in 5s)
    [SerializeField]
    private float speed = 1.8f;
    [SerializeField]
    private MoveMode moveMode = MoveMode.Random;
    public GhostMode mode = GhostMode.Normal;
    // predefined waypoints using for Pink ghost
    public GameObject[] waypoints;
    private Waypoint currentWaypoint, previousWaypoint, targetWaypoint;
    private Vector2 direction, nextDirection;
    int cur = 0; // current position of pre-defined waypoints index
    public Animator animator;
    // default duration for frightened mode
    float scaredDuration = 7;
    DateTime scaredStartingTime;
    // default duration for scattered mode
    float scatterDuration = 7;
    DateTime scatterStartingTime;

    private Vector2 headingDir;
    private Vector2 scan;
    GameData gameData;
    [SerializeField]
    private GameObject[] oponents;
    void Start()
    {
        mode = GhostMode.Normal;
        targetWaypoint = startingPoint;
        direction = Vector2.up;
        previousWaypoint = startingPoint;
        gameData = GameObject.Find("GameData").GetComponent<GameData>();
    }

    // in each frame, ghost will move, update their mode and animation accordingly
    override public void Update()
    {
        base.Update();
        Move();
        CheckMode(); // check and reset ghost mode
        ChangeAnim();
    }

    // Ghosts move following waypoints that placed along the map
    // They will choose next targeted waypoint based on their mode or moving habbit
    public void Move()
    {
        if (targetWaypoint != currentWaypoint && targetWaypoint != null)
        {
            if (IsOverShot()) // if targeted waypoint is passed, go back 
            {
                currentWaypoint = targetWaypoint;
                transform.position = currentWaypoint.transform.position;
                // Choose next waypoint as target to move to
                targetWaypoint = ChooseNextWaypoint();
                // save previous passed waypoint to calculate distance to next target
                previousWaypoint = currentWaypoint;
                currentWaypoint = null;
            } else //else move to target waypoint
            {
                MoveToWaypoint(targetWaypoint);
            }
        }
    }

    // Method to move tranform to target waypoint position
    public void MoveToWaypoint(Waypoint waypoint)
    {
        
        headingDir = waypoint.transform.position;
        transform.position = Vector3.MoveTowards(transform.position, headingDir, GetSpeed() * Time.deltaTime);
    }
    
    // calculate distance from target position to previous waypoint, support overshot checking 
    float LengthFromWaypoint(Vector2 targetPosition)
    {
        Vector2 vec = targetPosition - (Vector2)previousWaypoint.transform.position;
        return vec.magnitude;
    }

    // if current vector position is > than target waypoint position, it is overshot (or passed) 
    bool IsOverShot()
    {
        float waypointToTarget = LengthFromWaypoint(targetWaypoint.transform.position);
        float nodeToSelf = LengthFromWaypoint(transform.position);
        return nodeToSelf >= waypointToTarget;
    }

    // get distance between two positions 
    float GetDistance (Vector2 posA, Vector2 posB)
    {
        float dx = posA.x - posB.x;
        float dy = posA.y - posB.y;
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    // Method to choose next waypoint to move to
    Waypoint ChooseNextWaypoint()
    {
        // default target is pacman position
        Vector2 pacmanPos = startingPoint.transform.position;
        // In battle mode, 2 pacmans is present, then need to choose the nearest pacman
        if (gameData.allPacmans.Count > 1)
        {
            Vector2 pacman1Pos = ((Pacman)gameData.allPacmans[0]).transform.position;
            Vector2 pacman2Pos = ((Pacman)gameData.allPacmans[1]).transform.position;
            // calculate distance between current position to pacman 1 and pacman 2
            float distanceToPacman1 = GetDistance(transform.position, pacman1Pos);
            float distanceToPacman2 = GetDistance(transform.position, pacman2Pos);
            // choose the shorter distance as target
            if (distanceToPacman2 < distanceToPacman1)
            {
                pacmanPos = pacman2Pos;
            }
            else pacmanPos = pacman1Pos;
        }
        else if(gameData.allPacmans.Count == 1)
        {
            // if only 1 pacman, then choose its position as target
             pacmanPos = ((Pacman)gameData.allPacmans[0]).transform.position;
        }
        
        Vector2 targetPosition = pacmanPos;
        if (mode == GhostMode.Scatter)
        { 
            // in scatter mode, ghosts move back to their starting point
            targetPosition = startingPoint.transform.position;
        }
        if (mode == GhostMode.Normal)
        {
            targetPosition = pacmanPos; 
        }
        if (moveMode == MoveMode.Path)
        {
            // if ghosts move on a pre-defined path, get the relevant index from the waypoints array as target position 
            if (waypoints.Length >= 1)
            {
                targetPosition = waypoints[cur].transform.position;
                // Waypoint reached, select next one
                cur = (cur + 1) % waypoints.Length;
            }
        }

        // Every waypoint has other surrounding waypoints called neighbors 
        Waypoint moveToWaypoint = null;
        ArrayList foundWaypoints = new ArrayList();
        Vector2[] foundWaypointDirection = new Vector2[4];
        int wpCount = 0;
        // finding available waypoints to move from current waypoint
        for (int i = 0; i < currentWaypoint.neighbors.Length; i++)
        {
            // ignore the waypoint that the ghost come from, add other neighbor waypoints to available waypoints
            // if their is only 1 neighbor waypoint, then use it
            if(currentWaypoint.validDirections[i] != direction * (-1) || currentWaypoint.neighbors.Length == 1)
            {
                foundWaypoints.Add(currentWaypoint.neighbors[i]);
                foundWaypointDirection[wpCount] = currentWaypoint.validDirections[i];
                wpCount++;
            }
        }

        // if only 1 available waypoint found, move to that way point
        if(foundWaypoints.Count == 1)
        {
            moveToWaypoint = (Waypoint)foundWaypoints[0];
            direction = foundWaypointDirection[0];
        }

        // if more than 1 available waypoint found, choose next waypoint based on the ghost's characteristic
        if (foundWaypoints.Count > 1)
        {
            // if the ghost is in Runaway move mode or frightened, they run away from nearest Pacman
            // it means they will find the farthest available waypoint from pacman to move to
            if (moveMode == MoveMode.RunAway || mode == GhostMode.Frightened) // run away
            {
                float farthest = 0;
                // loop though all available waypoints to find the farthest one by comparing their distance from the target position
                for (int i = 0; i < foundWaypoints.Count; i++)
                {
                    if (foundWaypointDirection[i] != Vector2.zero)
                    {
                        float distance = GetDistance(((Waypoint)foundWaypoints[i]).transform.position, targetPosition);
                        if (distance > farthest)
                        {
                            farthest = distance;
                            moveToWaypoint = (Waypoint)foundWaypoints[i];
                            direction = foundWaypointDirection[i];
                        }
                    }
                }
            // if move mode is chase or follow path, meaning they will move to the nearest available waypoint from their target 
            } else if (moveMode == MoveMode.Chase || moveMode == MoveMode.Path || mode == GhostMode.Scatter) // Chase target position or go back to base
            {
                float leastDis = 10000f;
                for (int i = 0; i < foundWaypoints.Count; i++)
                {
                    if (foundWaypointDirection[i] != Vector2.zero)
                    {
                        float distance = GetDistance(((Waypoint)foundWaypoints[i]).transform.position, targetPosition);
                        if (distance < leastDis)
                        {
                            leastDis = distance;
                            moveToWaypoint = (Waypoint)foundWaypoints[i];
                            direction = foundWaypointDirection[i];
                        }
                    }
                }
            } else if (moveMode == MoveMode.Random) // Random turn
            { 
                // random choose 1 waypoint to move to from all available waypoints
                var random = UnityEngine.Random.Range(0, foundWaypoints.Count);
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

    // if ghosts become scatter, they will move to their starting position, it is the element at index 0 in their waypoints list
    public void BecomeScatter()
    {
        mode = GhostMode.Scatter;
        scatterStartingTime = DateTime.Now;
        cur = 0;
    }

    private void CheckMode()
    {
        // if ghost is frightened, check time to change them back to normal and reset multiplier
        if (mode == GhostMode.Frightened)
        {
            TimeSpan time = DateTime.Now - scaredStartingTime;
            if (time.Seconds >= scaredDuration)
            {
                gameData.ResetMultiplier();
                mode = GhostMode.Normal;
            }
        }

        // // if ghost is scattered, check time to change them back to normal
        if (mode == GhostMode.Scatter)
        {
            TimeSpan time = DateTime.Now - scatterStartingTime;
            if (time.Seconds >= scatterDuration)
            {
                mode = GhostMode.Normal;
            }
        }
    }
    
    // change ghost animation based on their state
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

        animator.SetFloat("DirX", direction.x);
        animator.SetFloat("DirY", direction.y);
    }

    // calculate actual speed of the ghost based on their mode and applied effect
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

    // In innovative mode, ghosts can pick up fruits and apply some special effects
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
        UnityEngine.Object.Destroy(item.gameObject);
    }
}
