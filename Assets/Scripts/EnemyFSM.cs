using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyFSM : MonoBehaviour
{
    enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Return,
        Damaged,
        Die
    }

    EnemyState m_State;
    public float findDistance = 8f;
    public float attackDistance = 2f;
    public float moveSpeed = 5f;
    public float currentTime = 0;
    public float attackDelay = 2f;
    public float moveDistance = 20f;

    public int attackPower = 3;
    public int hp = 15;
    int maxHp = 15;
    public Slider hpSlider;

    Transform player;
    CharacterController cc;
    Vector3 originPos;
    Quaternion originRot;
    Animator anim;
    
    // Start is called before the first frame update
    void Start()
    {
        m_State = EnemyState.Idle;
        player = GameObject.Find("Player").transform;
        cc = GetComponent<CharacterController>();
        originPos = transform.position;
        originRot = transform.rotation;

        anim = transform.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_State)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Return:
                Return();
                break;
            case EnemyState.Damaged:
                //Damaged();
                break;
            case EnemyState.Die:
                //Die();
                break;
        }

        hpSlider.value = (float)hp / (float)maxHp;
    }

    void Idle()
    {
        if(Vector3.Distance(transform.position, player.position) < findDistance)
        {
            m_State = EnemyState.Move;
            print("���� ��ȯ: Idle -> Move");

            anim.SetTrigger("IdleToMove");
        }
    }

    void Move()
    {
        // �̵� ���� ���� ���� ��
        if (Vector3.Distance(transform.position, originPos) > moveDistance)
        {
            m_State = EnemyState.Return;
            print("���� ��ȯ: Move -> Return");
        }
        
        // �÷��̾ ���� ���� ���� ��
        else if (Vector3.Distance(transform.position, player.position)> attackDistance)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            cc.Move(dir * moveSpeed * Time.deltaTime);
            transform.forward = dir;
        }

        else
        {
            m_State = EnemyState.Attack;
            print("���� ��ȯ: Move -> Attack");

            // �ٷ� ����
            currentTime = attackDelay;

            anim.SetTrigger("MoveToAttackDelay");
        }
    }

    void Attack()
    {
        // �÷��̾ ���� ���� ���� ��
        if (Vector3.Distance(transform.position, player.position) < attackDistance)
        {
            // ���� ������
            currentTime += Time.deltaTime;
            if(currentTime > attackDelay)
            {
                player.GetComponent<PlayerMove>().DamageAction(attackPower);
                print("����");
                currentTime = 0;

                anim.SetTrigger("StartAttack");
            }
        }

        // ���߰�
        else
        {
            m_State = EnemyState.Move;
            print("���� ��ȯ: Attck -> Move");
            currentTime = 0;

            anim.SetTrigger("AttackToMove");
        }
    }

    void Return()
    {
        // �ʱ� ��ġ�� �̵�
        if(Vector3.Distance(transform.position, originPos) > 0.1f)
        {
            Vector3 dir = (originPos - transform.position).normalized;
            cc.Move(dir * moveSpeed * Time.deltaTime);

            transform.forward = dir;
        }

        else
        {
            transform.position = originPos;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            hp = maxHp;
            m_State = EnemyState.Idle;
            print("���� ��ȯ: Return -> Idle");

            anim.SetTrigger("MoveToIdle");
        }
    }

    public void HitEnemy(int hitPower)
    {
        if(m_State == EnemyState.Damaged || m_State == EnemyState.Die || m_State == EnemyState.Return)
        {
            return;
        }
        
        hp -= hitPower;

        if (hp > 0)
        {
            m_State = EnemyState.Damaged;
            print("���� ��ȯ: Any state -> Damaged");
            Damaged();
        }

        else
        {
            m_State = EnemyState.Die;
            print("���� ��ȯ: Any state -> Die");

            anim.SetTrigger("Die");
            Die();
        }
    }
    void Damaged()
    {
        StartCoroutine(DamageProcess());
    }

    IEnumerator DamageProcess()
    {
        yield return new WaitForSeconds(0.5f);

        m_State = EnemyState.Move;
        print("���� ��ȯ: Damaged -> Move");
    }

    void Die()
    {
        StopAllCoroutines();

        StartCoroutine(DieProcess());
    }

    IEnumerator DieProcess()
    {
        cc.enabled = false;

        yield return new WaitForSeconds(1.5f);
        print("�Ҹ�");
        Destroy(gameObject);

        ScoreManager.Instance.Score++;
    }
}
