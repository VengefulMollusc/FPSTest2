using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

	[SerializeField]
	private float speed = 5.0f;
	[SerializeField]
	private float lookSensitivity = 3.0f;

	[SerializeField]
	private float thrusterForce = 1000.0f;

	[Header("Joint Options")]
	[SerializeField]
	private float jointSpring = 20f;
	[SerializeField]
	private float jointMaxForce = 40f;

	// component caching
	private PlayerMotor motor;
	private ConfigurableJoint joint;
	private Animator animator;

	void Start(){
		motor = GetComponent<PlayerMotor> ();
		joint = GetComponent<ConfigurableJoint> ();
		animator = GetComponent<Animator> ();

		SetJointSettings (jointSpring);
	}

	void Update (){
		// calculate movement as 3d vector
		float xMov = Input.GetAxis("Horizontal");
		float zMov = Input.GetAxis("Vertical");

		// multiply local directions by current movement values
		Vector3 movHorizontal = transform.right * xMov;
		Vector3 movVertical = transform.forward * zMov;

		// final movement vector
		Vector3 velocity = (movHorizontal + movVertical).normalized * speed; // add .normalized?

		// animate movement
		animator.SetFloat("ForwardVelocity", zMov);

		// apply movement
		motor.Move(velocity);

		// calculate rotation as 3d vector: for turning on y axis
		float yRot = Input.GetAxisRaw("Mouse X");

		Vector3 rotation = new Vector3 (0.0f, yRot, 0.0f) * lookSensitivity;

		// apply rotation
		motor.Rotate(rotation);

		// calculate camera rotation as 3d vector: for turning on x axis
		float xRot = Input.GetAxisRaw("Mouse Y");

		float cameraRotationX = xRot * lookSensitivity;

		// apply rotation
		motor.RotateCamera(cameraRotationX);

		// calculate and apply thruster force
		Vector3 _thrusterForce = Vector3.zero;
		if (Input.GetButton ("Jump")) {
			_thrusterForce = Vector3.up * thrusterForce;
			SetJointSettings (0f);
		} else {
			SetJointSettings (jointSpring);
		}

		motor.ApplyThruster (_thrusterForce);
	}

	private void SetJointSettings(float _jointSpring){
		joint.yDrive = new JointDrive {positionSpring = _jointSpring, 
			maximumForce = jointMaxForce};
	}
}
