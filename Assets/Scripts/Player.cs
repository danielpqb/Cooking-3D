using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject moveIndicator;

    private bool isWalking = false;
    private Vector3 lastMoveDir = Vector3.zero;

    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        HandleMove();
        HandleInteract();
    }

    private void HandleMove()
    {
        Vector3 moveDir = GetMoveDirection();

        isWalking = moveDir != Vector3.zero;

        if (isWalking)
        {
            lastMoveDir = moveDir;
            navMeshAgent.transform.forward = lastMoveDir;
        }

        float moveIndicatorDistance = 2f;
        moveIndicator.transform.position = transform.position + moveIndicatorDistance * moveDir;

        navMeshAgent.Move(moveSpeed * Time.deltaTime * moveDir);
    }

    private void HandleInteract()
    {
        float interactDistance = 10f;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, interactDistance))
        {
            if(hitInfo.transform.TryGetComponent(out ClearCounter clearCounter))
            {
                clearCounter.Interact();
            }
        } else
        {
            //Debug.Log("Não atingi!");
        }
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    public Vector3 GetMoveDirection()
    {
        Vector2 inputVector = playerInput.GetMoveVector();

        Vector3 moveDir = new(inputVector.x, 0f, inputVector.y);

        return moveDir;
    }
}
