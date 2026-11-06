using System;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node cameFrom;
    public List<Node> connections;

    public float gScore;
    public float hScore;
    public bool isCorner;

    void Start()
    {
        //if isCorner is true must be added manually
        if (!isCorner)
        {
            connections.Add(FindNearestLeftNode(this.transform.position));
            connections.Add(FindNearestRightNode(this.transform.position));   
        }
    }

    public float FScore()
    {
        return gScore + hScore;
    }

    public Node FindNearestLeftNode(Vector3 position)
    {
        Node nearestNode = null;
        float shortestDistance = float.MaxValue;

        foreach (Node node in AStarManager.instance.NodesInScene())
        {
            Vector3 nodePosition = node.transform.position;

            // Ignore nodes that aren't to the left
            if (nodePosition.x >= position.x){continue;}

            // Ignore vertical distance
            float distance = Vector2.Distance(new Vector2(position.y, position.x), new Vector2(nodePosition.y, nodePosition.x));

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestNode = node;
            }
        }

        return nearestNode;
    }

    public Node FindNearestRightNode(Vector3 position)
    {
        Node nearestNode = null;
        float shortestDistance = float.MaxValue;

        foreach (Node node in AStarManager.instance.NodesInScene())
        {
            Vector3 nodePosition = node.transform.position;

            // Ignore nodes that aren't to the left
            if (nodePosition.x <= position.x){continue;}

            // Ignore vertical distance
            float distance = Vector2.Distance(new Vector2(position.y, position.x), new Vector2(nodePosition.y, nodePosition.x));

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestNode = node;
            }
        }

        return nearestNode;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        if (connections.Count > 0)
        {
            for(int i = 0; i < connections.Count; i++)
            {
                Gizmos.DrawLine(transform.position, connections[i].transform.position);
            }
        }
    }
}
