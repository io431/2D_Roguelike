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

    private void Start()
    {
        player.OnMoveFinished += TryMove;
    }

    private void OnDestroy()
    {
        player.OnMoveFinished -= TryMove;
    }

    private void TryMove()
    {
        // 敵が移動中なら何もしない
        if (isMoving) return;

        // プレイヤーの位置と敵の位置をタイルマップ上のセル座標に変換
        Vector3Int playerCell = tilemap.WorldToCell(player.transform.position);
        Vector3Int enemyCell = tilemap.WorldToCell(transform.position);

        // 横方向に移動を試みる
        if (playerCell.x != enemyCell.x)
        {
            Vector3Int targetCell = enemyCell + new Vector3Int((int)Mathf.Sign(playerCell.x - enemyCell.x), 0, 0);

            // 移動先がプレイヤーのセルなら攻撃、そうでないならその方向に移動する
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

        // 横方向に移動できない場合、縦方向に移動を試みる
        if (playerCell.y != enemyCell.y)
        {
            Vector3Int targetCell = enemyCell + new Vector3Int(0, (int)Mathf.Sign(playerCell.y - enemyCell.y), 0);

            // 移動先がプレイヤーのセルなら攻撃、そうでないならその方向に移動する
            if (targetCell == playerCell)
            {
                AttackPlayer();
            }
            else if (player.IsWalkableTile(targetCell))
            {
                StartCoroutine(MoveToCell(targetCell));
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
