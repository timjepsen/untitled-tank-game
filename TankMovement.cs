using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public float m_Speed = 12f;                 // How fast the tank moves forward and back.
    public float m_TurnSpeed = 180f;            // How fast the tank turns in degrees per second.
    //public AudioSource m_MovementAudio;         // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.
    //public AudioClip m_EngineIdling;            // Audio to play when the tank isn't moving.
    //public AudioClip m_EngineDriving;           // Audio to play when the tank is moving.
    //public float m_PitchRange = 0.2f;           // The amount by which the pitch of the engine noises can vary.


    private string[] m_MovementAxisNames;          // The name of the input axis for moving forward and back.
    private string[] m_TurnAxisNames;              // The name of the input axis for turning.
    private Rigidbody m_Rigidbody;              // Reference used to move the tank.
    private TankManager m_TankManager;
    private float m_MovementInputValue;         // The current value of the movement input.
    private float m_TurnInputValue;             // The current value of the turn input.
    private float m_OriginalPitch;              // The pitch of the audio source at the start of the scene.
    private bool m_AbleToMove = true;
    [SerializeField] private float m_TotalFuel;
    [SerializeField] private float m_CurrentFuel;
    [SerializeField] private float m_MaxRollAmount; //how far does the tank need to have turned over before we reset its rotation?

    private void Awake ()
    {
        m_Rigidbody = GetComponent<Rigidbody> ();
        m_TankManager = GetComponent<TankManager>();
        
        
        //uncomment and the axis names are based on player number. 
        m_MovementAxisNames = new string[] { "Vertical controller","Vertical keyboard" };// + m_PlayerNumber; for multiple controllers
        m_TurnAxisNames = new string[] {"Horizontal controller","Horizontal keyboard"};// + m_PlayerNumber;
    }
    public void MakeActive(bool active) {
        if (active) {
            m_CurrentFuel = m_TotalFuel;
        }
    }
    private void OnEnable ()
    {
        // When the tank is turned on, make sure it's not kinematic.
        m_Rigidbody.isKinematic = false;

        // Also reset the input values.
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }
    private void OnDisable ()
    {
        //When the tank is turned off, set it to kinematic so it stops moving.
        m_Rigidbody.isKinematic = true;
    }
    private void Update ()
    {
        // Store the value of both input axes.
        m_MovementInputValue = Input.GetAxis (m_MovementAxisNames[0]) + Input.GetAxis (m_MovementAxisNames[1]);
        m_TurnInputValue = Input.GetAxis (m_TurnAxisNames[0]) + Input.GetAxis (m_TurnAxisNames[1]);//refactor - should generalise to any number of axis names

        MakeSureDoesntRoll();
        //EngineAudio ();
    }

    private void FixedUpdate ()
    {
        // Adjust the rigidbodies position and orientation in FixedUpdate.
        if (m_TankManager.GetActive()  && m_TankManager.m_TankShooting.GetShotRemaining()){
                Turn();
            if (m_CurrentFuel > 0.0f && m_AbleToMove) {
                Move ();
            }

            if (m_CurrentFuel > 0f) {
                m_CurrentFuel = m_CurrentFuel - Mathf.Abs (m_MovementInputValue);//TODO: modify this so fuel also depends on velocity. 
                m_TankManager.m_UIManager.SetFuelAmount(m_CurrentFuel / m_TotalFuel);
            }
        }
    }

    private void Move ()
    {
        // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

        // Apply this movement to the rigidbody's position.
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }

    private void Turn ()
    {
        // Determine the number of degrees to be turned based on the input, speed and time between frames.
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

        // Make this into a rotation in the y axis.
        Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);

        // Apply this rotation to the rigidbody's rotation.
        m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation);
    }

    public void SetAbleToMove(bool AbleToMove) {
        m_TankManager.m_TankUIManager.SetCanMove(AbleToMove);
        m_AbleToMove = AbleToMove;
    }
    public bool GetAbleToMove() {
        return m_AbleToMove;
    }
    public void MakeSureDoesntRoll() {
        if (Vector3.Dot(transform.up, Vector3.up) < m_MaxRollAmount) {
            transform.position = transform.position + 3.0f * Vector3.up;
            Vector3 facingDirection = Vector3.ProjectOnPlane(transform.forward, Vector3.up);//project the object's forward direction onto the horizontal plane
            transform.LookAt(transform.position + facingDirection);
            m_Rigidbody.angularVelocity = Vector3.zero;
            m_Rigidbody.velocity = Vector3.zero;
        }
    }
}