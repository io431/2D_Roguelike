using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public int hp = 30;
    public Tilemap tilemap;
    public float moveDistance = 1f;
    public delegate void PlayerMoveHandler();
    public event PlayerMoveHandler OnMoveFinished;

    private Animator animator;

    private bool isMoving = false;
    private Vector3 targetPosition;
    private int counter = 0;
    private void Awake()
    {
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

        // 変更点：GetAxisRawからGetKeyDownに変更
        bool moveUp = Input.GetKey(KeyCode.UpArrow);
        bool moveDown = Input.GetKey(KeyCode.DownArrow);
        bool moveRight = Input.GetKey(KeyCode.RightArrow);
        bool moveLeft = Input.GetKey(KeyCode.LeftArrow);

        if (!(moveUp || moveDown || moveRight || moveLeft))
        {
            counter = 0;
            return;
        }

        counter++;


        Vector3Int currentCell = tilemap.WorldToCell(transform.position);
        Vector3Int targetCell;

        if (moveUp) targetCell = currentCell + Vector3Int.up;
        else if (moveDown) targetCell = currentCell + Vector3Int.down;
        else if (moveRight) targetCell = currentCell + Vector3Int.right;
        else if (moveLeft) targetCell = currentCell + Vector3Int.left;
        else return;

        if (moveUp) animator.SetInteger("Direction", 2);
        else if (moveDown) animator.SetInteger("Direction", 0);
        else if (moveRight) animator.SetInteger("Direction", 3);
        else if (moveLeft) animator.SetInteger("Direction", 1);




        if (!IsWalkableTile(targetCell)) return;

        if (IsEnemyTile(targetCell)&&counter==1)
        {
            AttackEnemy(targetCell);
            return;
        }

        targetPosition = tilemap.GetCellCenterWorld(targetCell);

       
        StartCoroutine(MovePlayer());
    }
    public bool IsWalkableTile(Vector3Int cellPosition)
    {
        TileBase tile = tilemap.GetTile(cellPosition);
        return (tile != null && !tile.name.Contains("Unwalkable"));
    }

    private IEnumerator MovePlayer()
    {
        isMoving = true;

        while (transform.position != targetPosition)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveDistance * Time.deltaTime);
            yield return null;
        }

        isMoving = false;
        OnMoveFinished?.Invoke();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("item"))
        {
            Debug.Log("アイテムを獲得!");
            Destroy(other.gameObject);
        }
    }

    public bool IsEnemyTile(Vector3Int cellPosition)
    {
        // セル内のすべてのコライダーを取得
        Collider2D[] colliders = Physics2D.OverlapCircleAll(tilemap.GetCellCenterWorld(cellPosition), 0.1f);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.CompareTag("Enemy"))
            {
                return true;
            }
        }
        return false;
    }

    public void AttackEnemy(Vector3Int cellPosition)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(tilemap.GetCellCenterWorld(cellPosition), 0.1f);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.CompareTag("Enemy"))
            {
               
                Debug.Log("敵を攻撃!");
                
                break;
            }
        }
    }
}