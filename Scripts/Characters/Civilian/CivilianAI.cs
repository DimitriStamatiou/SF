using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CivilianAI : MonoBehaviour
{
    //declaring the Finite State Machine
    public enum FSMState
    {
        None,
        Wandering,
        Alert,
        Spooked,
    }

    //declaring some other essential variables
    public FSMState curState;

    GameObject Guards;

    protected Transform playerTransform;
    [SerializeField] Transform LKP;

    public GameObject[] waypointList;
    private NavMeshAgent nav;

    public int waypointNext = 0;
    public int stopping = 5;
    public int seeingRad = 10;

    float timer = 7;

    // Start is called before the first frame update
    void Start()
    {
        // Setting the starting state
        curState = FSMState.Wandering;

        // Initializing the navmesh agent and setting the waypoint
        nav = GetComponent<NavMeshAgent>();
        nav.SetDestination(waypointList[waypointNext].transform.position);

        // Defining the player
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        playerTransform = objPlayer.transform;
        if (!playerTransform)
        {
            print("Player not found, add one with Tag Player");
        }

        // initializing the guards to make them effect the player's health
        Guards = GameObject.FindWithTag("Guard");
    }

    // Update is called once per frame
    void Update()
    {
        //this is the Finite State Machine
        switch (curState)
        {
            case FSMState.Wandering: UpdateWanderingState(); break;
            case FSMState.Alert: UpdateAlertState(); break;
            case FSMState.Spooked: UpdateSpookedState(); break;
        }
    }

    // Wandering Code
    protected void UpdateWanderingState(){
        nav.isStopped = false;

        // If the Civilian has reached the destination incriment waypointNext by 1
        if(Vector3.Distance(transform.position, waypointList[waypointNext].transform.position) <= stopping){
            waypointNext += 1;
            // If waypointNext is greater than the length of the waypointlist reset waypointNext to 0
            if(waypointNext >= waypointList.Length){
                waypointNext = 0;
            }
            // Go to the next waypoint
            nav.SetDestination(waypointList[waypointNext].transform.position);
        }

        if((Vector3.Distance(Guards.transform.position, playerTransform.position) <= seeingRad) && (Vector3.Distance(transform.position, playerTransform.position) <= seeingRad)){
            curState = FSMState.Alert;
        }else if((Vector3.Distance(Guards.transform.position, playerTransform.position) <= seeingRad) && (Vector3.Distance(transform.position, playerTransform.position) > seeingRad)){
            curState = FSMState.Alert;
        }
    }
    // Alert Code
    protected void UpdateAlertState(){
        nav.isStopped = true;

        if(Vector3.Distance(transform.position, playerTransform.position) <= seeingRad){
            timer = 7;
            LKP.position = playerTransform.position;
        }else if(Vector3.Distance(transform.position, playerTransform.position) > seeingRad){
            timer -= Time.deltaTime;
            if(timer <= 0){
                timer = 5;
                curState = FSMState.Spooked;
            }
        }
    }
    //Spooked Code
    protected void UpdateSpookedState(){
        timer -= Time.deltaTime;
        if(timer <= 0){
            curState = FSMState.Wandering;
        }
        if(Vector3.Distance(transform.position, playerTransform.position) <= seeingRad){
            curState = FSMState.Alert;
        }
    }

    //some quality of life things for the editor
    void OnDrawGizmos (){
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, stopping);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, seeingRad);
    }
}
