using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardAI : MonoBehaviour
{
    //declaring the Finite State Machine
    public enum FSMState
    {
        None,
        Patrol,
        Attack,
        Suspicious,
        Investigating,
    }

    //declaring some other essential variables
    public FSMState curState;

    protected Transform playerTransform;
    // Last known position (of the player)
    [SerializeField] Transform LKP;

    public GameObject[] waypointList;
    private NavMeshAgent nav;

    public int waypointNext = 0;
    public int stopping = 5;
    public int seeingRad = 10;
    public float timer = 5;
    bool Investigated = false;
    public bool Attacking;

    void Start()
    {
        Attacking = false;
        //starting the navmesh agent
        nav = GetComponent<NavMeshAgent>();

        nav.SetDestination(waypointList[waypointNext].transform.position);

        curState = FSMState.Patrol;

        //searching for the player
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        playerTransform = objPlayer.transform;

        if (!playerTransform)
        {
            print("Player not found, add one with Tag Player");
        }
    }

    void Update()
    {
        //this is the Finite State Machine
        switch (curState)
        {
            case FSMState.Patrol: UpdatePatrolState(); break;
            case FSMState.Attack: UpdateAttackState(); break;
            case FSMState.Suspicious: UpdateSusState(); break;
            case FSMState.Investigating: UpdateInvState(); break;
        }
    }

    // Patrol code
    protected void UpdatePatrolState(){
        nav.isStopped = false;

        // If the guard just investigated an area
        if(Investigated == true){
            // Resetting investigation check and return to patroling
            Investigated = false;
            nav.SetDestination(waypointList[waypointNext].transform.position);
        }

        // making the guard move from waypoint to waypoint periodically
        if(timer > 0){
            // if the distance between the guard and the waypoint is closer than the stopping distance, make it stop for 5 seconds
            if(Vector3.Distance(transform.position, waypointList[waypointNext].transform.position) <= stopping){
                nav.isStopped = true;
                timer -= Time.deltaTime;
            }
        // if the time is up, get the guard to move to the next waypoint
        }else if(timer <= 0){
            waypointNext += 1;
            nav.isStopped = false;
            timer = 5;
            if(waypointNext >= waypointList.Length){
                waypointNext = 0;
            }
            // set the destination to the next waypoint
            nav.SetDestination(waypointList[waypointNext].transform.position);
        }
        // if the player enters the seeing radius of the guard, the guard will become suspicious
        if(Vector3.Distance(transform.position, playerTransform.position) <= seeingRad){
            curState = FSMState.Suspicious;
        }
    }
    // Attack code
    protected void UpdateAttackState(){
        // if the guard can't see the player anymore
        if(Vector3.Distance(transform.position, playerTransform.position) >= seeingRad){
            //set the last known position and investigate it
            LKP.position = playerTransform.position;
            Attacking = false;
            curState = FSMState.Investigating;
        }else{
            Attacking = true;
            nav.SetDestination(playerTransform.position);
        }
    }
    //Suspicious code
    protected void UpdateSusState(){
        // Stop the guard dead in their tracks and start the timer
        nav.isStopped = true;
        timer -= Time.deltaTime;
        // If the guard is within the seeing radius of the player and the timer is 0, go investigate
        if(Vector3.Distance(transform.position, playerTransform.position) <= seeingRad){
            if(timer <= 0){
                LKP.position = playerTransform.position;
                // Reset the timer
                timer = 3;
                curState = FSMState.Investigating;
            }
        }
        if(Vector3.Distance(transform.position, playerTransform.position) > seeingRad){
            curState = FSMState.Patrol;
        }
    }
    //Investigating code
    protected void UpdateInvState(){
        // Restart the navmesh & go to the last known position of the player
        nav.isStopped = false;
        nav.SetDestination(LKP.position);
        // If the player is within the stopping distance, attack the player
        if(Vector3.Distance(transform.position, playerTransform.position) <= stopping){
            curState = FSMState.Attack;
        }else if(Vector3.Distance(transform.position, LKP.position) <= stopping){// if the guard doesn't see the player within 3 seconds
            timer -= Time.deltaTime;
            if(timer <= 0){// Reset the timer and go back to patroling the area
                timer = 5;
                Investigated = true;
                curState = FSMState.Patrol;
            }
        }
    }
    //some quality of life things for the editor
    void OnDrawGizmos (){
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stopping);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, seeingRad);
    }
}
