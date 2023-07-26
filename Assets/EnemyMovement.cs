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

    public AudioClip attackSound;  // �U����
    private AudioSource audioSource;
 
   



    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
       
    }



    public void TryMove()
    {
        if (isMoving) return;

        
        Vector3Int enemyCell = tilemap.WorldToCell(transform.position);
        Vector3Int diff = player.targetCell - enemyCell;

        // �΂߈ړ������݂�
        if (Mathf.Abs(diff.x) >= 1 && Mathf.Abs(diff.y) >= 1)
        {
            Vector3Int direction = new Vector3Int((int)Mathf.Sign(diff.x), (int)Mathf.Sign(diff.y), 0);
            Vector3Int targetCell = enemyCell + direction;
            if (targetCell == player.targetCell)
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

        // �΂߂Ɉړ��ł��Ȃ���΁A���܂��͏c�Ɉړ������݂�
        if (diff.x != 0)
        {
            Vector3Int targetCell = enemyCell + new Vector3Int((int)Mathf.Sign(diff.x), 0, 0);
            if (targetCell == player.targetCell)
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
            if (targetCell == player.targetCell)
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
    {//�폜���邩��
        TryMove();
    }
    public void AttackPlayer()
    {

     
        // �v���C���[���U�����鏈��

        Debug.Log("�U�����ꂽ��");
        audioSource.PlayOneShot(attackSound);
    }

    private IEnumerator MoveToCell(Vector3Int targetCell)
    {
        isMoving = true;
        targetPosition = tilemap.GetCellCenterWorld(targetCell);

        // �ړ��A�j���[�V�����Ȃǂ̏�����ǉ�����ꍇ�͂����ɋL�q

        // �ړ�
        while (transform.position != targetPosition)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveDistance * Time.deltaTime);
            yield return null;
        }

        isMoving = false;
    }
}