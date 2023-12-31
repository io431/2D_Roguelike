using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public EnemyData MonsterData;
    public int hp;
    public int atk;
    public int def;
    public int exp;

    public Tilemap tilemap;
    public float moveDistance = 1f;
    public delegate void PlayerMoveHandler();
   

    public EnemyMovement enemy;
    private Animator animator;

    private bool isMoving = false;
    private Vector3 targetPosition;
    private int counter = 0;

    public Vector3Int targetCell;

    private void Start()
    {
        animator = GetComponent<Animator>();

       
}

    private void Update()
    {
        if (isMoving) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            enemy.TryMove();
            return;
        }

       
        // 変更点：GetAxisRawからGetKeyに変更
        bool moveUp = Input.GetKey(KeyCode.UpArrow);
        bool moveDown = Input.GetKey(KeyCode.DownArrow);
        bool moveRight = Input.GetKey(KeyCode.RightArrow);
        bool moveLeft = Input.GetKey(KeyCode.LeftArrow);

        // 新規に追加：斜め移動のためのキー入力を検出
        bool moveUpRight = moveUp && moveRight|| Input.GetKey(KeyCode.E);
        bool moveUpLeft = moveUp && moveLeft|| Input.GetKey(KeyCode.Q);
        bool moveDownRight = moveDown && moveRight|| Input.GetKey(KeyCode.C);
        bool moveDownLeft = moveDown && moveLeft|| Input.GetKey(KeyCode.Z);

        if (!(moveUp || moveDown || moveRight || moveLeft || moveUpRight || moveUpLeft || moveDownRight || moveDownLeft))
        {
            //無限に攻撃コマンドが実行されない為の処理
            counter = 0;
            return;
        }

        counter++;

        Vector3Int currentCell = tilemap.WorldToCell(transform.position);

        // 斜め移動のための新しいターゲットセルの計算
        if (moveUpRight) targetCell = currentCell + new Vector3Int(1, 1, 0);
        else if (moveUpLeft) targetCell = currentCell + new Vector3Int(-1, 1, 0);
        else if (moveDownRight) targetCell = currentCell + new Vector3Int(1, -1, 0);
        else if (moveDownLeft) targetCell = currentCell + new Vector3Int(-1, -1, 0);
        else if (moveUp) targetCell = currentCell + Vector3Int.up;
        else if (moveDown) targetCell = currentCell + Vector3Int.down;
        else if (moveRight) targetCell = currentCell + Vector3Int.right;
        else if (moveLeft) targetCell = currentCell + Vector3Int.left;
        else return;

        // アニメーションの方向を設定する
        // ここでは斜め移動に対応するアニメーションが存在しないと仮定
        if (moveUp || moveUpRight || moveUpLeft) animator.SetInteger("Direction", 2);
        else if (moveDown || moveDownRight || moveDownLeft) animator.SetInteger("Direction", 0);
        else if (moveRight || moveDownRight || moveUpRight) animator.SetInteger("Direction", 3);
        else if (moveLeft || moveDownLeft || moveUpLeft) animator.SetInteger("Direction", 1);

        //壁は進まずに向きだけ変える
        if (!IsWalkableTile(targetCell)) return;



        if (IsEnemyTile(targetCell) && counter == 1)
        {
            AttackEnemy(targetCell);
            return;
        }
        else if(!IsEnemyTile(targetCell))
        {
            //カウンターが0ならば長押しでそのまま進む
            targetPosition = tilemap.GetCellCenterWorld(targetCell);
            StartCoroutine(MovePlayer());
            enemy.TryMove();
        }
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
                enemy.AttackPlayer();
                break;
            }
        }
    }
}