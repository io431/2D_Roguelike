using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public Tilemap tilemap;
    public float moveDistance = 1f;
    public delegate void PlayerMoveHandler();
    public event PlayerMoveHandler OnMoveFinished;

    // Animatorコンポーネントの参照を追加
    private Animator animator;

    private bool isMoving = false;
    private Vector3 targetPosition;

    private void Awake()
    {
        // Animatorコンポーネントを取得
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isMoving) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnMoveFinished?.Invoke();
            return;
        }

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (horizontalInput == 0 && verticalInput == 0) return;

        Vector3Int currentCell = tilemap.WorldToCell(transform.position);
        Vector3Int targetCell = currentCell + new Vector3Int(Mathf.RoundToInt(horizontalInput), Mathf.RoundToInt(verticalInput), 0);

        if (!IsWalkableTile(targetCell)) return;

        targetPosition = tilemap.GetCellCenterWorld(targetCell);

        // 入力に基づいてAnimatorの方向パラメーターを更新
        if (verticalInput > 0) animator.SetInteger("Direction", 2);      // 上
        else if (verticalInput < 0) animator.SetInteger("Direction", 0); // 下
        else if (horizontalInput > 0) animator.SetInteger("Direction", 3); // 右
        else if (horizontalInput < 0) animator.SetInteger("Direction", 1); // 左

        StartCoroutine(MovePlayer());
    }
    public bool IsWalkableTile(Vector3Int cellPosition)
    {
        // タイルマップ上の位置からタイルを取得
        TileBase tile = tilemap.GetTile(cellPosition);

        // タイルがnullでないかつ、歩けないタイルであればfalseを返す
        return (tile != null && !tile.name.Contains("Unwalkable")); // "Unwalkable"は歩けないタイルの名前に応じて変更してください
    }

    private IEnumerator MovePlayer()
    {
        isMoving = true;

        // 移動アニメーションなどの処理

        // 移動
        while (transform.position != targetPosition)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveDistance * Time.deltaTime);
            yield return null;
        }

        isMoving = false;
        OnMoveFinished?.Invoke();
    }
}
