using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class NPC_Controller : MonoBehaviour
{
    
    public Node currentNode;
    public List<Node> path = new List<Node>();

    public playerMovement player;
    public float speed = 2f;
    private Enemy enemy;

    public enum StateMachine
    {
        PATROL,
        ENGAGE
    }

    public enum JumpType
    {
        NOJUMP,
        UPJUMP,
        LEFTJUMP,
        RIGHTJUMP
    }
    
    [Header("Physics Settings")]
    public Rigidbody2D rb;
    private Collider2D col;
    private RaycastHit2D[] hitResults = new RaycastHit2D[5]; 
    public bool useWorldTagOnly = false; // toggle true for only "World" tagged objects to count as ground

    [Header("Jump Parameters")]
    public Vector2 upJumpForce = new Vector2(0f, 10f);
    public Vector2 leftJumpForce = new Vector2(-3f, 5f);
    public Vector2 rightJumpForce = new Vector2(3f, 5f);

    private JumpType currentJumpType = JumpType.NOJUMP;
    private bool isGrounded;
    private bool hasJumpedForCurrentNode = false;
    private float targetSatisfactionRadius = 0.4f;

    public StateMachine currentState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = StateMachine.PATROL;
        currentNode = AStarManager.instance.FindNearestNode(this.transform.position);

        if (rb == null) { rb = GetComponent<Rigidbody2D>(); }
        if (col == null) { col = GetComponent<Collider2D>(); }

        enemy = gameObject.GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        // kills all movement when enemy dies
        // if (enemy.isDead)
        // {
        //     Debug.Log("no move bc u die L<OL");
        //     return;
        // }

        //isGrounded = CheckIsGrounded();
        
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
            currentJumpType = JumpType.NOJUMP;
        } else if (playerSeen && currentState != StateMachine.ENGAGE)
        {
            currentState = StateMachine.ENGAGE;
            path.Clear();
            currentJumpType = JumpType.NOJUMP;
        }
        #endregion
    }

    void FixedUpdate() // physics calculations for moving npc done here, stick to tick rate
    {
        // kills all movement when enemy dies
        if (enemy.isDead)
        {
            Debug.Log("no move bc u die L<OL");
            return;
        }

        DoPath();
    }

    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (!useWorldTagOnly || collision.gameObject.CompareTag("World"))
    //     {
    //         isGrounded = true;
    //     }
    // }

    // private void OnCollisionExit2D(Collision2D collision)
    // {
    //     if (!useWorldTagOnly || collision.gameObject.CompareTag("World"))
    //     {
    //         isGrounded = false;
    //     }
    // }

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

            if (path.Count > 0)
            {
                EvaluateJumpType(currentNode, path[0]);
            }
        }
    }

    //If Enemy is close enough to player will just walk to them
    void Engage()
    {
        
    }

    void DoPath()
    {
        if (path.Count > 0)
        {
            Node targetNode = path[0];

            // 1. Trigger Jump Impulse once when entering a jump segment
            if (currentJumpType != JumpType.NOJUMP)
            {
                if (!hasJumpedForCurrentNode)
                {
                    DoJump(currentJumpType);
                    hasJumpedForCurrentNode = true;
                }
            }
            else // Walk normally when on flat ground or slope
            {
                float direction = Mathf.Sign(targetNode.transform.position.x - transform.position.x);
                rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
            }

            // 2. Separate X and Y proximity checks so vertical height doesn't block node removal
            float xDist = Mathf.Abs(transform.position.x - targetNode.transform.position.x);
            float yDist = Mathf.Abs(transform.position.y - targetNode.transform.position.y);

            if (xDist < targetSatisfactionRadius && yDist < 1.2f)
            {
                currentNode = path[0];
                path.RemoveAt(0);

                if (path.Count > 0)
                {
                    EvaluateJumpType(currentNode, path[0]);
                }
                else
                {
                    currentJumpType = JumpType.NOJUMP;
                }
            }
        }
        else
        {
            // Stop horizontal motion when path ends
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    void DoJump(JumpType j)
    {
        // Set linearVelocity directly for precise, repeatable jump trajectories
        switch (j)
        {
            case JumpType.UPJUMP:
                rb.linearVelocity = upJumpForce;
                break;
            case JumpType.LEFTJUMP:
                rb.linearVelocity = leftJumpForce;
                break;
            case JumpType.RIGHTJUMP:
                rb.linearVelocity = rightJumpForce;
                break;
        }
    }

    void EvaluateJumpType(Node source, Node destin)
    {
        hasJumpedForCurrentNode = false;

        // Reduced height difference requirement from 0.5f to 0.2f
        float yDiff = destin.transform.position.y - source.transform.position.y;
        float xDiff = destin.transform.position.x - source.transform.position.x;

        if (yDiff > 0.2f)
        {
            if (xDiff > 0.2f)
            {
                currentJumpType = JumpType.RIGHTJUMP;
            }
            else if (xDiff < -0.2f)
            {
                currentJumpType = JumpType.LEFTJUMP;
            }
            else
            {
                currentJumpType = JumpType.UPJUMP;
            }
        }
        else
        {
            currentJumpType = JumpType.NOJUMP;
        }

        Debug.Log($"Evaluating segment from {source.name} to {destin.name} | JumpType: {currentJumpType}");
    }
}