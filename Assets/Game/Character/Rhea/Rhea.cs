using System;
using UnityEngine;

public class Rhea : Character<Rhea, RheaState, RheaStateInput>
{
    [SerializeField]
    private float dashSpeed = 10;
    [SerializeField]
    private float dashDuration = 0.2f;
    [SerializeField]
    private GameObject shieldPrefab = null;

    [SerializeField]
    private float beamInterval = 5;
    [SerializeField]
    private GameObject beamPrefab = null;

    override protected void Init()
    {
        input.anim = GetComponent<Animator>();
        input.cc2d = GetComponent<CircleCollider2D>();
        input.rb = GetComponent<Rigidbody2D>();
        input.cc = GetComponent<SharedCharacterController>();
        input.sr = GetComponent<SpriteRenderer>();
        input.shieldPrefab = shieldPrefab;
        input.beamPrefab = beamPrefab;
    }

    override protected void SetInitialState()
    {
        ChangeState<RheaIdleState>();
    }

    override protected void UpdateInput()
    {
        input.dashSpeed = dashSpeed;
        input.dashDuration = dashDuration;
        input.beamInterval = beamInterval;
    }
}

public abstract class RheaState : CharacterState<Rhea, RheaState, RheaStateInput>
{
    protected int prevHorizontalMovement;
    protected int prevVerticalMovement;

    // Logic used in multiple states
    protected void HandleMoveAnimation(RheaStateInput input)
    {
        int horizontalMovement = 0;
        if (InputMap.Instance.GetInput(input.cc.playerNumber, ActionType.RIGHT))
        {
            horizontalMovement++;
        }
        if (InputMap.Instance.GetInput(input.cc.playerNumber, ActionType.LEFT))
        {
            horizontalMovement--;
        }

        int verticalMovement = 0;
        if (InputMap.Instance.GetInput(input.cc.playerNumber, ActionType.UP))
        {
            verticalMovement++;
        }
        if (InputMap.Instance.GetInput(input.cc.playerNumber, ActionType.DOWN))
        {
            verticalMovement--;
        }

        bool moving = horizontalMovement != 0 || verticalMovement != 0;
        bool prevMoving = prevHorizontalMovement != 0 || prevVerticalMovement != 0;
        if (prevHorizontalMovement != horizontalMovement && horizontalMovement != 0)
        {
            input.anim.Play("Move");
            input.sr.flipX = horizontalMovement == 1;
        }
        else if (!prevMoving && verticalMovement != 0)
        {
            input.anim.Play("Move");
        }
        else if (!moving && prevMoving)
        {
            input.anim.Play("Idle");
        }
        prevHorizontalMovement = horizontalMovement;
        prevVerticalMovement = verticalMovement;
    }
}

public class RheaStateInput : CharacterStateInput
{
    public float dashSpeed;
    public float dashDuration;
    public GameObject shieldPrefab;

    public float beamInterval;
    public GameObject beamPrefab;

    public Animator anim;
    public CircleCollider2D cc2d;
    public Rigidbody2D rb;
    public SharedCharacterController cc;
    public SpriteRenderer sr;
}