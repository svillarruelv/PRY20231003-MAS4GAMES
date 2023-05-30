using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    private enum EnemyState{
        Walking,
        Ragdoll,
        Dead
    }

    [SerializeField]
    private Camera _camera; //Variable de c�mara del jugador
    
    private Rigidbody[] _ragdollRigidbodies;
    private EnemyState _currentState = EnemyState.Walking;
    private Animator _animator; //Variable para llamar al animador de Unity
    private CharacterController _characterController;

    //Variables para el MAS
    private bool isDead = false;
    private float _health = 100; //Salud/Vida del jugador
    private int id; //ID del enemigo
    public float AttackRange = 3f; //Rango de ataque
    
    void Awake()
    {
        //Get every ragdoll component in the model
        _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        DisableRagdoll();
    }

    // Update is called once per frame
    void Update()
    {
        switch(_currentState){
            case EnemyState.Walking:
                WalkingBehaviour();
                break;
            case EnemyState.Ragdoll:
                RagdollBehaviour();
                break;
        }
    }

    //Death Function (Activates trigger)
    public void TriggerDeath()
    {
        _currentState = EnemyState.Dead;
        _animator.SetBool("isDead", true); //Activar el trigger de la animaci�n para la muerte

        //EnableRagdoll(); //Volverlo Ragdoll
        Debug.Log("MUERE"); 

        GetComponent<Collider>().enabled = false; //Desactivar las colisiones (colider)
        this.enabled = false; //Desactivar el script


    }

    public void TakeDamage(float Health)
    {
        _health = Health;

        if (_health > 0)
        {
            //Animaci�n que recibe da�o
            Debug.Log("HIT!");
            _animator.SetTrigger("Hurt");
        } else
        {
            this.TriggerDeath();
        }
    }

    private void DisableRagdoll()
    {
        foreach (var rigidBody in _ragdollRigidbodies){
            rigidBody.isKinematic = true;
        }

        _animator.enabled = true;
        _characterController.enabled = true;
    }

    private void EnableRagdoll()
    {
        foreach (var rigidBody in _ragdollRigidbodies){
            rigidBody.isKinematic = false;
        }

        _animator.enabled = false;
        _characterController.enabled = false;

        Collider collider = GetComponent<Collider>();
        Debug.Log(collider.enabled); 
    }

    private void WalkingBehaviour()
    {
        Vector3 direction = _camera.transform.position - transform.position;
        direction.y = 0;
        direction.Normalize();

        Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 100 * Time.deltaTime);


        if(Input.GetKeyDown(KeyCode.Space)){
            EnableRagdoll();
            _currentState = EnemyState.Ragdoll;
        }
    }

    private void RagdollBehaviour()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            DisableRagdoll();
            _currentState = EnemyState.Walking;
        }
    }

    //Attack function
    public void Attack() 
    {
        //Validate attack range from player


        //Set animation trigger
         Debug.Log("PLAYER ATTACKED!");


    }
}
