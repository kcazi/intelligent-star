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
        PATROL,
        ENGAGE
    }

    public StateMachine currentState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = StateMachine.PATROL;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case StateMachine.PATROL:
                Patrol();
                break;
            case StateMachine.ENGAGE:
                Engage();
                break;
        }
        
        bool playerSeen = Vector2.Distance(transform.position, player.transform.position) < 1.0f;

        if (!playerSeen && currentState != StateMachine.PATROL)
        {
            currentState = StateMachine.PATROL;
            path.Clear();
        } else if (playerSeen && currentState != StateMachine.ENGAGE)
        {
            currentState = StateMachine.ENGAGE;
            path.Clear();
        }

        CreatePath();
    }


    //Goal Node is nearest to player
    void Patrol()
    {
        if(path.Count == 0)
        {
            path = AStarManager.instance.GeneratePath(currentNode, AStarManager.instance.FindNearestNode(player.transform.position));
        }
    }

    //If Enemy is close enough to player will just walk to them
    void Engage()
    {
        
    }

    void CreatePath()
    {
        if(path.Count > 0)
        {
            int x = 0;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(path[x].transform.position.x, path[x].transform.position.y, -2), speed * Time.deltaTime);

            if(Vector2.Distance(transform.position, path[x].transform.position) < 0.1f)
            {
                currentNode = path[x];
                path.RemoveAt(x);
            }
        }
    }
}