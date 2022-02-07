using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class raycast : MonoBehaviour
{
    private GameObject raycastedObj;

    //initialising variables
    [SerializeField] private float rayLength = 10f;
    [SerializeField] private LayerMask layerMaskInteract;
    [SerializeField] private Image uiCrosshair;
    [SerializeField] private Image grabCrosshair;

    public bool gotKey;
    [SerializeField] GameObject Door;


    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        if(Physics.Raycast(transform.position, fwd, out hit, rayLength, layerMaskInteract.value)){
            //The objective book needs to be taged with Key
            if(hit.collider.CompareTag("Key")){
                raycastedObj = hit.collider.gameObject;
                CrosshairActive();
                if(Input.GetKeyDown("e")){
                    //resetting the state of the raycast
                    raycastedObj.SetActive(false);
                    gotKey = true;
                }
            }if(hit.collider.CompareTag("Door") && gotKey == true){
                raycastedObj = hit.collider.gameObject;
                CrosshairActive();
                if(Input.GetKeyDown("e")){
                    //resetting the state of the raycast
                    Door.transform.Rotate(Vector3.up, -90);
                    gotKey = false;
                }
            }
        }else{
            CrosshairNormal();                                                                                                                                           
        }
    }

    //When the crosshair goes over an objective book
    void CrosshairActive()
    {
        uiCrosshair.enabled = false;
	    grabCrosshair.enabled = true;
    }

    //When the crosshair goes away from the objective book
    void CrosshairNormal()
    {
        uiCrosshair.enabled = true;
	    grabCrosshair.enabled = false;
    }

}
