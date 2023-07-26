using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public int hp = 30;
    public Tilemap tilemap;
    public float moveDistance = 1f;
    public delegate void PlayerMoveHandler();
   

    public EnemyMovement enemy;
    private Animator animator;

    private bool isMoving = false;
    private Vector3 targetPosition;
    private int counter = 0;

    public Vector3Int targetCell;

    private void Awake()
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

       
        // �ύX�_�FGetAxisRaw����GetKey�ɕύX
        bool moveUp = Input.GetKey(KeyCode.UpArrow);
        bool moveDown = Input.GetKey(KeyCode.DownArrow);
        bool moveRight = Input.GetKey(KeyCode.RightArrow);
        bool moveLeft = Input.GetKey(KeyCode.LeftArrow);

        // �V�K�ɒǉ��F�΂߈ړ��̂��߂̃L�[���͂����o
        bool moveUpRight = moveUp && moveRight|| Input.GetKey(KeyCode.E);
        bool moveUpLeft = moveUp && moveLeft|| Input.GetKey(KeyCode.Q);
        bool moveDownRight = moveDown && moveRight|| Input.GetKey(KeyCode.C);
        bool moveDownLeft = moveDown && moveLeft|| Input.GetKey(KeyCode.Z);

        if (!(moveUp || moveDown || moveRight || moveLeft || moveUpRight || moveUpLeft || moveDownRight || moveDownLeft))
        {
            //�����ɍU���R�}���h�����s����Ȃ��ׂ̏���
            counter = 0;
            return;
        }

        counter++;

        Vector3Int currentCell = tilemap.WorldToCell(transform.position);

        // �΂߈ړ��̂��߂̐V�����^�[�Q�b�g�Z���̌v�Z
        if (moveUpRight) targetCell = currentCell + new Vector3Int(1, 1, 0);
        else if (moveUpLeft) targetCell = currentCell + new Vector3Int(-1, 1, 0);
        else if (moveDownRight) targetCell = currentCell + new Vector3Int(1, -1, 0);
        else if (moveDownLeft) targetCell = currentCell + new Vector3Int(-1, -1, 0);
        else if (moveUp) targetCell = currentCell + Vector3Int.up;
        else if (moveDown) targetCell = currentCell + Vector3Int.down;
        else if (moveRight) targetCell = currentCell + Vector3Int.right;
        else if (moveLeft) targetCell = currentCell + Vector3Int.left;
        else return;

        // �A�j���[�V�����̕�����ݒ肷��
        // �����ł͎΂߈ړ��ɑΉ�����A�j���[�V���������݂��Ȃ��Ɖ���
        if (moveUp || moveUpRight || moveUpLeft) animator.SetInteger("Direction", 2);
        else if (moveDown || moveDownRight || moveDownLeft) animator.SetInteger("Direction", 0);
        else if (moveRight || moveDownRight || moveUpRight) animator.SetInteger("Direction", 3);
        else if (moveLeft || moveDownLeft || moveUpLeft) animator.SetInteger("Direction", 1);

        //�ǂ͐i�܂��Ɍ��������ς���
        if (!IsWalkableTile(targetCell)) return;



        if (IsEnemyTile(targetCell) && counter == 1)
        {
            AttackEnemy(targetCell);
            return;
        }
        else if(!IsEnemyTile(targetCell))
        {
            //�J�E���^�[��0�Ȃ�Β������ł��̂܂ܐi��
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
            Debug.Log("�A�C�e�����l��!");
            Destroy(other.gameObject);
        }
    }

    public bool IsEnemyTile(Vector3Int cellPosition)
    {
        // �Z�����̂��ׂẴR���C�_�[���擾
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
               
                Debug.Log("�G���U��!");
                enemy.AttackPlayer();
                break;
            }
        }
    }
}