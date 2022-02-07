using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlayerScript : MonoBehaviour{
	// controller variables
	[SerializeField] int speed = 10;
	[SerializeField] int Sspeed = 20;
    [SerializeField] int cspeed = 5;
	Vector3 velocity;
	[SerializeField] float gravity = 9.81f;

    public CharacterController controller;
	private float health = 100.0f;

    float timer = 3;

    bool attacking;
    GuardAI guard;
    GameObject Guards;
    [SerializeField] GameObject cam;

	// UI variables
	[SerializeField] TextMeshProUGUI heal;

	void Start(){
        // initializing the guards to make them effect the player's health
        Guards = GameObject.FindWithTag("Guard");
        guard = Guards.GetComponent<GuardAI>();
	}

	void FixedUpdate(){
		Move();
		Grav();
		UI();
        Health();
	}

	void Move(){
        // getting the input
		float x = Input.GetAxis("Horizontal");
		float z = Input.GetAxis("Vertical");

		// move variable
		Vector3 move = transform.right * x + transform.forward * z;

		// making the player to sprint
		if(Input.GetKey(KeyCode.LeftShift)){
			controller.Move(move * Sspeed * Time.deltaTime);
		}else if(Input.GetKey(KeyCode.LeftControl)){// get the player to crouch
            controller.Move(move * cspeed * Time.deltaTime);
            cam.transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);
        }else{// making the player walk
            controller.Move(move * speed * Time.deltaTime);
            cam.transform.position = new Vector3(transform.position.x, 2.24f, transform.position.z);
        }
    }

	void Grav(){
		velocity.y -= gravity * Time.deltaTime;
		controller.Move(velocity * Time.deltaTime);
	}

	void UI(){
		heal.text = health.ToString();
	}

	void Health(){
        attacking = guard.Attacking;
        if(attacking == true){
            timer -= Time.deltaTime;
            if(timer <= 0){
                health -= 10;
                timer = 3;
            }
        }else{
            timer -= Time.deltaTime;
            if(timer <= 0 && health < 100){
                health += 1.3f * Time.deltaTime;
            }else if(health > 100){
                health = 100;
            }
        }
    }
}
