using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    private CapsuleCollider myCollider;

    private void Awake()
    {
        myCollider = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        HandleMove();
        HandleInteract();
    }

    private void HandleMove()
    {
        Vector3 moveDir = GetMoveDirection();
        isWalking = moveDir.magnitude > 0;

        float moveDistance = moveSpeed * Time.deltaTime;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, lastMoveDir, Time.deltaTime * rotateSpeed);

        float moveIndicatorDistance = 1.5f;
        moveIndicator.transform.position = transform.position + moveIndicatorDistance * moveDir;

        Vector3 lowestColliderPoint = myCollider.transform.position;
        Vector3 highestColliderPoint = myCollider.transform.position + Vector3.up * myCollider.height;
        float colliderRadius = myCollider.radius;
        bool canMove = !Physics.CapsuleCast(lowestColliderPoint, highestColliderPoint, colliderRadius, moveDir, moveDistance);

        if (!canMove)
        {
            //Try to move only on X
            Vector3 moveDirX = new(moveDir.x, 0, 0);
            bool canMoveX = !Physics.CapsuleCast(lowestColliderPoint, highestColliderPoint, colliderRadius, moveDirX, moveDistance);
            if (canMoveX)
            {
                Move(moveDirX.normalized, moveDistance);
            }
            else
            {
                //Try to move only on Z
                Vector3 moveDirZ = new(0, 0, moveDir.z);
                bool canMoveZ = !Physics.CapsuleCast(lowestColliderPoint, highestColliderPoint, colliderRadius, moveDirZ, moveDistance);
                if (canMoveZ)
                {
                    Move(moveDirZ.normalized, moveDistance);
                }
                else
                {
                    //Can't move in this direction, so try to move only on oposite X direction
                    //float opositeX = -moveDir.x;
                    //float velocityMultiplier = 2f;
                    //Vector3 moveDirOpositeX = new Vector3(opositeX, 0, 0).normalized;
                    //bool canMoveOpositeX = !Physics.CapsuleCast(lowestColliderPoint, highestColliderPoint, colliderRadius, moveDirOpositeX, moveDistance);
                    //if (canMoveOpositeX)
                    //{
                    //    Move(moveDirOpositeX * velocityMultiplier, moveDistance);
                    //}
                }
            }
        }
        else
        {
            Move(moveDir, moveDistance);
        }
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

    private void Move(Vector3 moveDirection, float moveDistance)
    {
        transform.position += moveDistance * moveDirection;
        lastMoveDir = moveDirection;
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
