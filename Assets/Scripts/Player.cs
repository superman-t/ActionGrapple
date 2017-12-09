using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public float jumpHeight = 4;
	public float timeToJumpApex = .4f;
	float accelerationTimeGrounded = .1f;
	public float moveSpeed = 6;


	float logicJumpHeight = 0;
	float originGravity;
	float gravity;
	float jumpVelocity;
	Vector3 velocity;
	float velocityXSmoothing;
	float velocityYSmoothing;
	Vector2 input;
	bool inputJump; 
	bool inputAttack;
	bool isJumping = false;

	Animator animator;
	SpriteRenderer render;

	void Awake(){
		animator = GetComponent<Animator> ();
		render = GetComponent<SpriteRenderer> ();
	}

	void Start() {
		originGravity = gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		logicJumpHeight = 0;
		print ("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);
	}

	void Update() {
		input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		inputJump = Input.GetKeyDown (KeyCode.Space);


		Attack ();
		Jump ();
		Move ();

		transform.Translate(velocity * Time.deltaTime);
		if (!isJumping && !inputAttack) {
			animator.SetBool ("jump", false);
		}
	}

	void Jump() {
		gravity = logicJumpHeight <= 0 ? 0 : originGravity;
		if (logicJumpHeight <= 0) {
			logicJumpHeight = 0;
		}

		if (inputJump) {
			isJumping = true;
			animator.SetBool ("jump", true);
			if (logicJumpHeight <= 0) {
				velocity.y = jumpVelocity;
				gravity =  originGravity;
			}
		}

		if (isJumping) {
			velocity.y += gravity * Time.deltaTime;
			float h = velocity.y * Time.deltaTime;
			logicJumpHeight += h;


			if (logicJumpHeight <= 0) {
				//fix logic position y
				transform.Translate (new Vector3 (0, h - logicJumpHeight, 0));
				velocity.y = 0;
				isJumping = false;
			}
		}

		inputJump = false;
	}

	void Attack(){
		bool box = false;
		bool kick = false;
		if (Input.GetKeyDown (KeyCode.Z)) {
			animator.SetBool ("box", true);
			box = true;
		} else {
			animator.SetBool ("box", false);
			box = false;
		}

		if (Input.GetKeyDown (KeyCode.X)) {
			animator.SetBool ("kick", true);
			kick = true;
		} else {
			animator.SetBool ("kick", false);
			kick = false;
		}
		inputAttack = box || kick;
	}

	void Move(){
		float targetVelocityX = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTimeGrounded);

		if (!isJumping) {
			float targetVelocityY = input.y * moveSpeed;
			velocity.y = Mathf.SmoothDamp (velocity.y, targetVelocityY, ref velocityYSmoothing, accelerationTimeGrounded);
		}

		if (input.x != 0 || input.y != 0) {
			animator.SetBool ("walk", true);
			render.flipX = input.x < 0;
		} else {
			animator.SetBool ("walk", false);
		}
			
	}

	void FixedUpdate(){
		
	}
}
