using Pandora.Scripts.Effect;
using Pandora.Scripts.Player.Controller;
using Pandora.Scripts.System;
using Pandora.Scripts.System.Event;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Pandora.Scripts.Enemy
{
    public class SecondBossController : MonoBehaviour, IHitAble
    {
        //Component
        private Rigidbody2D rb;
        private Animator anim;
        private PolygonCollider2D polygonCollider;
        private GameObject target;
        private float delay = 300f; //����ȭ
        private float AttackPatternDelay = 15f; //�ٸ� ���� ���� ������
        private bool isCharging = false;
        public float chargeSpeed = 3f; // ���� �ӵ�
        public float chargeDuration = 0.5f; // ���� ���� �ð�
        public float cooldownTime = 10f; // ���� ��Ÿ��
        private bool isCooldown = false;
        public bool canAttack = false;

        //Status
        public EnemyStatus _enemyStatus;

        private void Awake()
        {
            _enemyStatus = new EnemyStatus("2StageBoss");
        }
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            polygonCollider = GetComponent<PolygonCollider2D>();
            StartCoroutine(StartChargingWithCooldown());
        }

        // Update is called once per frame
        void Update()
        {
            target = GameObject.FindGameObjectWithTag("Player"); //�÷��̾� ã��
            rb.velocity = Vector3.zero; //�и� ����
        }
        public void Hit(HitParams hitParams)
        {
            var damage = hitParams.damage;
            anim.SetBool("isFollow", false);
            anim.SetTrigger("Hit");
            transform.Find("BossHP").gameObject.SetActive(true);
            //damage effect
            var reduceDamage = damage - (damage * _enemyStatus.DefencePower / 100);
            var relativePos = new Vector3(1.5f, 1f, 0);
            hitParams.damage = reduceDamage;
            DamageTextEffectManager.Instance.SpawnDamageTextEffect(relativePos, gameObject, hitParams);

            _enemyStatus.NowHealth -= reduceDamage;
            CallHealthChangeEvetnt();

            if (_enemyStatus.NowHealth <= 0)
            {
                StartCoroutine(Death());
            }
        }
        private void OnDisable()
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.GetComponent<FadeTextEffect>() != null)
                    Destroy(child.gameObject);
            }
        }
        public void Attack()
        {
            if (canAttack)
            {
                target.GetComponent<PlayerController>().Hurt(_enemyStatus.BaseDamage, null, gameObject);
            }
            canAttack = false;
        }
        public void AnotherAttack() //�⺻ ���ݿ� n��ŭ�� �������� ����
        {
            if (canAttack)
            {
                target.GetComponent<PlayerController>().Hurt(_enemyStatus.BaseDamage + 3f, null, gameObject);
            }
            canAttack = false;
        }
        private void CallHealthChangeEvetnt()
        {
            var param = new BossHealthChangedParam(_enemyStatus.NowHealth, _enemyStatus.MaxHealth);
            EventManager.Instance.TriggerEvent(PandoraEventType.BossHealthChanged, param);
        }
        IEnumerator Death()
        {
            anim.SetTrigger("Death");
            yield return new WaitForSeconds(1.5f);
            Destroy(gameObject);
        }
        public void OnHitAnimationEnd()
        {
            anim.SetBool("isFollow", true);
        }
        IEnumerator WIDE_AREA()//����ȭ
        {
            yield return new WaitForSeconds(delay);
            _enemyStatus.AttackPower += 10f;
            _enemyStatus.Speed += 2f;
        }
        IEnumerator StartChargingWithCooldown()
        {
            if (isCooldown)
            {
                yield break; // �̹� ���� ���̸� ����������
            }
            isCooldown = true;
            while (true)
            {
                // ���� ��Ÿ�� ��ٸ���
                yield return new WaitForSeconds(cooldownTime);

                // ���� ����
                StartCharge();

                // ������ ���� ������ ��ٸ���
                yield return new WaitForSeconds(chargeDuration);
            }

        }
        void StartCharge()
        {
            isCharging = true;
            StartCoroutine(Charge());
        }

        IEnumerator Charge()
        {
            float distance = Vector2.Distance(target.transform.position, gameObject.transform.position);
            float startTime = Time.time;
            anim.SetBool("isFollow", false);
            anim.SetTrigger("Rush");
            while (distance <= 1f)
            {
                // ���� �������� �̵�
                transform.Translate((target.transform.position - gameObject.transform.position).normalized * chargeSpeed * Time.deltaTime);
            }
            yield return null; // ���� �����ӱ��� ���

            // ���� ����
            isCharging = false;
            anim.SetBool("isFollow", true);
        }
    }
}
