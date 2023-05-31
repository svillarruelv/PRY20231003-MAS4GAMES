using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float _health = 100; //Health

    public enum PlayerState { 
        ALIVE, 
        HURT,
        DEAD};

    private int id = 0; //Player ID

    private PlayerState _currentState = PlayerState.ALIVE; //Player State

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //string log = "Player's health: " + _health.ToString();
        //Debug.Log(log);
    }
    
    public void TakeDamage(float Health) {
        string log = "Damage Health parameter: " + Health.ToString();
        Debug.Log(log);
        _health = Health; 

        if (_health > 0)
        {
            Debug.Log("Player hit!");
            _currentState = PlayerState.HURT;

            //_animator.SetTrigger("Hurt");
        } else
        {
            Debug.Log("Player Death");
            _currentState = PlayerState.DEAD;
            //this.TriggerDeath();
        }

        _currentState = PlayerState.ALIVE;
    }
}
