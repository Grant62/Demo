using System.Collections;
using Features.Dungeon.Application;
using Features.Dungeon.Domain;
using UnityEngine;

namespace Features.Dungeon.Infrastructure
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private LayerMask _obstacleLayer;
        [SerializeField] private float _moveDuration = 0.12f;
        [SerializeField] private FogOfWarRenderer _fogOfWar;
        [SerializeField] private int _visionRange = 2;

        private bool _isMoving;

        private void Start()
        {
            GameObject entrance = GameObject.FindGameObjectWithTag("Entrance");
            if (entrance != null)
            {
                transform.position = entrance.transform.position;
            }

            FogOfWarService.SetVisionRange(_visionRange);

            if (_fogOfWar != null)
            {
                _fogOfWar.RevealAt(transform.position);
            }
        }

        private void Update()
        {
            if (_isMoving)
                return;

            Vector2 dir = GetInputDirection();
            if (dir == Vector2.zero)
                return;

            TryMove(dir);
        }

        private static Vector2 GetInputDirection()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                return Vector2.right;
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                return Vector2.left;
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                return Vector2.up;
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                return Vector2.down;

            return Vector2.zero;
        }

        private void TryMove(Vector2 dir)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position, dir, 1.0f, _obstacleLayer
            );

            if (!hit)
            {
                StartCoroutine(SmoothMove(dir));
            }
        }

        private IEnumerator SmoothMove(Vector2 dir)
        {
            _isMoving = true;

            Vector3 from = transform.position;
            Vector3 to = from + (Vector3)dir;

            float elapsed = 0f;
            while (elapsed < _moveDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _moveDuration;
                transform.position = Vector3.Lerp(from, to, t);
                yield return null;
            }

            transform.position = to;
            _isMoving = false;

            if (_fogOfWar != null)
            {
                _fogOfWar.RevealAt(transform.position);
            }

            CheckCellInteraction();
        }

        private void CheckCellInteraction()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);
            foreach (Collider2D hit in hits)
            {
                if (hit.TryGetComponent(out IDungeonInteractable interactable)
                    && interactable.CanInteract)
                {
                    interactable.Interact();
                    return;
                }
            }
        }
    }
}
