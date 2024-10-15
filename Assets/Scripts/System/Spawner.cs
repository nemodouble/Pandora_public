using Pandora.Scripts.System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    float timer;

    private void Awake()
    {
        timer = 0;
    }

    //�ӽ� : �ִ� 5�������� 5�ʸ��� ��� ����
    void Update()
    {
        timer += Time.deltaTime;

        if(timer > 5 && GameManager.Instance.pool.numObject <5)
        {
            GameManager.Instance.pool.Get(1);

            timer = 0;
        }
    }
}
