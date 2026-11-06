using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class NPC_Controller : MonoBehaviour
{
    
    public Node currentNode;
    public List<Node> path = new List<Node>();

    public playerMovement player;
    public float speed = 3f;

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
        currentNode = AStarManager.instance.FindNearestNode(this.transform.position);
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
        
        #region player seen interactions
        bool playerSeen = Vector2.Distance(transform.position, player.transform.position) < 1.5f;

        if (!playerSeen && currentState != StateMachine.PATROL)
        {
            currentState = StateMachine.PATROL;
            currentNode = AStarManager.instance.FindNearestNode(this.transform.position);
            path.Clear();
        } else if (playerSeen && currentState != StateMachine.ENGAGE)
        {
            currentState = StateMachine.ENGAGE;
            path.Clear();
        }
        #endregion

        CreatePath();
    }


    //Goal Node is nearest to player
    private Node previousTargetNode;
    void Patrol()
    {
        Node newTargetNode = AStarManager.instance.FindNearestNode(player.transform.position);

        if(path.Count == 0)
        {
            previousTargetNode = newTargetNode;
            
            path.Clear();
            
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
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(path[0].transform.position.x, path[0].transform.position.y, -2), speed * Time.deltaTime);

            if(Vector2.Distance(transform.position, path[0].transform.position) < 0.1f)
            {
                currentNode = path[0];
                path.RemoveAt(0);
            }
        }
    }
}