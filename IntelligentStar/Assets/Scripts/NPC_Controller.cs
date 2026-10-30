using System.Collections.Generic;
using UnityEngine;

public class NPC_Controller : MonoBehaviour
{
    
    public Node currentNode;
    public List<Node> path = new List<Node>();

    public playerMovement player;
    private float speed = 3f;

    public enum StateMachine
    {
        Patrol,
        Engage,
        Evade
    }

    public StateMachine currentState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Patrol()
    {
        
    }

    void Engage()
    {
        
    }

    void Evade()
    {
        
    }

}