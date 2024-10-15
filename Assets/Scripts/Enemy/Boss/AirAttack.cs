using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirAttack : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.Equals(GameObject.FindGameObjectWithTag("Player"))) //���� ���� Player�� �����Ѵٸ�
        {
            transform.parent.GetComponent<ShadowBossController>().AirAttackCollisionEvent();
            Debug.Log("���� ��ȯ: AirAttack -> Follow");
            transform.parent.GetComponent<ShadowBossController>().anim.SetBool("isFollow", true);
            gameObject.SetActive(false);
        }
    }
}
