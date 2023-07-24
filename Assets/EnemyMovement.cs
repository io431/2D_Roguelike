using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyMovement : MonoBehaviour
{
    public Tilemap tilemap;
    public float moveDistance = 1f;
    public PlayerMovement player;

    private bool isMoving = false;
    private Vector3 targetPosition;

    public AudioClip attackSound;  // 攻撃音
    private AudioSource audioSource;
 
   



    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player.OnMoveFinished += TryMove;
    }

    private void OnDestroy()
    {
        player.OnMoveFinished -= TryMove;
    }

    private void TryMove()
    {
        if (isMoving) return;

        Vector3Int playerCell = tilemap.WorldToCell(player.transform.position);
        Vector3Int enemyCell = tilemap.WorldToCell(transform.position);
        Vector3Int diff = playerCell - enemyCell;

        // 斜め移動を試みる
        if (Mathf.Abs(diff.x) >= 1 && Mathf.Abs(diff.y) >= 1)
        {
            Vector3Int direction = new Vector3Int((int)Mathf.Sign(diff.x), (int)Mathf.Sign(diff.y), 0);
            Vector3Int targetCell = enemyCell + direction;
            if (targetCell == playerCell)
            {
                AttackPlayer();
                return;
            }
            else if (player.IsWalkableTile(targetCell))
            {
                StartCoroutine(MoveToCell(targetCell));
                return;
            }
        }

        // 斜めに移動できなければ、横または縦に移動を試みる
        if (diff.x != 0)
        {
            Vector3Int targetCell = enemyCell + new Vector3Int((int)Mathf.Sign(diff.x), 0, 0);
            if (targetCell == playerCell)
            {
                AttackPlayer();
                return;
            }
            else if (player.IsWalkableTile(targetCell))
            {
                StartCoroutine(MoveToCell(targetCell));
                return;
            }
        }

        if (diff.y != 0)
        {
            Vector3Int targetCell = enemyCell + new Vector3Int(0, (int)Mathf.Sign(diff.y), 0);
            if (targetCell == playerCell)
            {
                AttackPlayer();
                return;
            }
            else if (player.IsWalkableTile(targetCell))
            {
                StartCoroutine(MoveToCell(targetCell));
                return;
            }
        }
    }
    private void EnemyTurn()
    {
        TryMove();
    }
    private void AttackPlayer()
    {

        // プレイヤーを攻撃する処理

        Debug.Log("攻撃されたよ");
        audioSource.PlayOneShot(attackSound);
     
    }

    private IEnumerator MoveToCell(Vector3Int targetCell)
    {
        isMoving = true;
        targetPosition = tilemap.GetCellCenterWorld(targetCell);

        // 移動アニメーションなどの処理を追加する場合はここに記述

        // 移動
        while (transform.position != targetPosition)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveDistance * Time.deltaTime);
            yield return null;
        }

        isMoving = false;
    }
}