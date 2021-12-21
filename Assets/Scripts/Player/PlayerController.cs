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
    [SerializeField] private GameObject exclamationMark;
    [SerializeField] private AudioClip battleTheme;

    [SerializeField] private AudioClip walk_1_SFX;
    [SerializeField] private AudioClip walk_2_SFX;
    [SerializeField] private AudioClip encounterSFX;


    public event Action OnEncountered;

    private bool isMoving;
    public static bool canMove = true;
    private Vector2 input;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void HandleUpdate()
    {
        if (Time.timeScale == 0) return;
        if (!canMove)
        {
            animator.SetBool("isMoving", false);
            return;
        }

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
        StartCoroutine(CheckForPortal());
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, nonWalkableLayer) != null)
            return false;
        else
            return true;
    }

    private IEnumerator CheckForPortal()
    {
        Collider2D collider = Physics2D.OverlapCircle(player.position, 0.2f, portalLayer);
        if (collider != null)
        {
            canMove = false;
            collider.GetComponent<Portals>().PlayEnterSFX();

            GameManager.Instance.SetActiveOptionButton(false);
            yield return Fader.Instance.FadeIn(0.5f);
            collider.GetComponent<Portals>().PlayEnterBGM();
            player.localPosition = collider.GetComponent<Portals>().EndPos.localPosition;
            yield return Fader.Instance.FadeOut(0.5f);
            GameManager.Instance.SetActiveOptionButton(true);

            canMove = true;
        }


    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(player.position, 0.2f, longGrassLayer) != null)
        {
            if (UnityEngine.Random.Range(0, 100) <= 10)
            {
                animator.SetBool("isMoving", false);
                StartCoroutine(OnEncounteredAnimation());
            }
        }
    }

    private IEnumerator OnEncounteredAnimation()
    {
        canMove = false;
        GameManager.Instance.SetActiveOptionButton(false);
        SoundManager.Instance.PlaySFX(encounterSFX);
        exclamationMark.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        SoundManager.Instance.PlayBGM(battleTheme);
        for (int i = 0; i < 2; i++)
        {
            yield return Fader.Instance.FadeIn(0.15f);
            yield return Fader.Instance.FadeOut(0.15f);
        }
        yield return Fader.Instance.FadeIn(0.15f);
        yield return new WaitForSeconds(1f);
        OnEncountered();
        StartCoroutine(Fader.Instance.FadeOut(0.5f));
        exclamationMark.SetActive(false);
        canMove = true;
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
                    animator.SetBool("isMoving", false);
                    interactable.Interact(facingDir);
                    
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

    public void PlayWalk1()
    {
        SoundManager.Instance.PlaySFX(walk_1_SFX);
    }

    public void PlayWalk2()
    {
        SoundManager.Instance.PlaySFX(walk_2_SFX);
    }
}
