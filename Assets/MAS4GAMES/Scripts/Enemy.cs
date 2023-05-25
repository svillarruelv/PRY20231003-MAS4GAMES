using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    private enum EnemyState{
        Walking,
        Ragdoll
    }

    [SerializeField]
    private Camera _camera;
    
    private Rigidbody[] _ragdollRigidbodies;
    private EnemyState _currentState = EnemyState.Walking;
    private Animator _animator;
    private CharacterController _characterController;
    
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
}
