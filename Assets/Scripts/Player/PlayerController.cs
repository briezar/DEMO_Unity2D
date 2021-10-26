using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private LayerMask nonWalkableLayer;
    [SerializeField] private LayerMask longGrassLayer;
    [SerializeField] private LayerMask portalLayer;

    [SerializeField] private Transform player;

    public event Action OnEncountered;

    private bool isMoving;
    private Vector2 input;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void HandleUpdate()
    {
        Vector2 prevFacingDir = new Vector2(animator.GetFloat("input.x"), animator.GetFloat("input.y"));

        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                animator.SetFloat("input.x", input.x);
                animator.SetFloat("input.y", input.y);

                Vector3 targetPos = player.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                Vector2 newFacingDir = new Vector2(input.x, input.y);

                if (IsWalkable(targetPos) && (prevFacingDir == newFacingDir))
                    StartCoroutine(Move(targetPos));
                else
                    StartCoroutine(FaceNewDirection(0.1f));

            }
        }

        animator.SetBool("isMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Interact();
        }

    }

    private IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - player.position).sqrMagnitude > Mathf.Epsilon)
        {
            player.position = Vector3.MoveTowards(player.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        player.position = targetPos;

        isMoving = false;

        CheckForEncounters();
        CheckForPortal();
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, nonWalkableLayer) != null)
            return false;
        else
            return true;
    }

    private void CheckForPortal()
    {
        Collider2D collider = Physics2D.OverlapCircle(player.position, 0.2f, portalLayer);
        if (collider != null)
        {
            player.localPosition = Portals.Instance.EndPos.localPosition;
            //player.localPosition = new Vector3(player.localPosition.x - 0.5f, player.localPosition.y);
        }


    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(player.position, 0.2f, longGrassLayer) != null)
        {
            if (UnityEngine.Random.Range(0, 100) <= 10)
            {
                animator.SetBool("isMoving", false);
                OnEncountered();
            }
        }
    }

    private void Interact()
    {
        var facingDir = new Vector3(animator.GetFloat("input.x"), animator.GetFloat("input.y"));
        var interactPos = player.position + facingDir;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(interactPos, 0.2f, nonWalkableLayer);
        if (colliders != null)
        {
            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent<IInteractable>(out var interactable))
                {
                    interactable.Interact();
                    break;
                }
            }
        }
    
    }

    IEnumerator FaceNewDirection(float time)
    {
        isMoving = true;
        yield return new WaitForSeconds(time);
        isMoving = false;

    }
}
