using UnityEngine;
namespace Pandora.Scripts.Enemy
{
    public class SecondBossAI : MonoBehaviour
    {
        // Components
        private Vector3 direction;
        private GameObject target;
        private string parentName;
        private Vector3 myPos;
        private Animator parentAnimation;

        //Status
        private float speed;
        private float timer;
        private int waitingTime;

        public float attackRange = 2f;
        Vector3 attackRangePos;

        private void Start()
        {
            timer = 0.0f;
            waitingTime = 2;
            parentName = transform.parent.name;
            attackRangePos = GameObject.Find(parentName).transform.Find("AttackRange").transform.localPosition;
            myPos = transform.position;
            parentAnimation = transform.parent.GetComponent<Animator>();
            speed = transform.parent.GetComponent<SecondBossController>()._enemyStatus.Speed; //������ ������ ���ǵ�� ����
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer > 0.5)
                transform.parent.transform.Find("AttackRange").gameObject.SetActive(false);
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            float distance = 0.0f;
            //�÷��̾� �ĺ�
            if (collision.gameObject.tag.Equals("Player"))
            {
                target = collision.gameObject;
            }
            //�÷��̾���� �Ÿ� ����
            if (target != null)
            {
                //�ڽ��� �θ�[���� obj�� �÷��̾���� �Ÿ�
                distance = Vector2.Distance(transform.parent.position, target.transform.position);
            }
            if (timer > waitingTime && target == collision.gameObject)
            {
                setDirection();
                selectBehavior(distance);
            }

        }
        private void setDirection()
        {
            direction = target.transform.position - transform.parent.position; //�ٶ󺸴� ����
            direction.Normalize(); //����ȭ

            //���� ��ȯ
            if (direction.x < 0) //������ �ٶ󺸴� ���
            {
                transform.parent.GetComponent<Animator>().SetFloat("Direction", -1);
                GameObject.Find(parentName).transform.Find("AttackRange").transform.localPosition = new Vector3(-attackRangePos.x, attackRangePos.y, 0);
            }
            else
            {
                transform.parent.GetComponent<Animator>().SetFloat("Direction", 1);
                GameObject.Find(parentName).transform.Find("AttackRange").transform.localPosition = new Vector3(attackRangePos.x, attackRangePos.y, 0);
            }
        }
        private void selectBehavior(float distance)
        {
            if (distance > attackRange) //�Ÿ��� ���� ���� �ۿ� ���� ��
            {
                transform.parent.position += direction * speed * Time.deltaTime; //�÷��̾� �������� �̵�
                parentAnimation.SetBool("isFollow", true); //�ִϸ��̼��� �÷��̾� ���󰡴� �ɷ� ����
            }
            else //���� ���� ���� �ִٸ�
            {
                int type = Random.Range(0, 2); //�������� �����Կ� ���� ������ �޶���
                switch (type)
                {
                    case 0:
                        parentAnimation.SetTrigger("Attack1");
                        parentAnimation.SetBool("isFollow", false); //Idle ����
                        timer = 0;
                        break;
                    case 1:
                        parentAnimation.SetTrigger("Attack2");
                        parentAnimation.SetBool("isFollow", false);
                        timer = 0;
                        break;
                }
                GameObject.Find(parentName).transform.Find("AttackRange").gameObject.SetActive(true);
            }
        }


        private void OnTriggerExit2D(Collider2D collision)
        {
            if (target == collision.gameObject)
            {
                target = null;
            }
        }
    }
}