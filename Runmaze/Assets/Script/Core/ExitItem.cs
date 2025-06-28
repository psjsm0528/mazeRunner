using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitItem : MonoBehaviour
{
    public string Itemname;
    public bool isActivate;
    //충돌을 하기 위해서 작성한 이벤트 코드

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 플레이어만 이 아이템과 충돌할 수 있다.
        {
            //Destroy(gameObject);       // Exititem 컴포넌트 가지고 있는 아이템이 무엇?
            gameObject.SetActive(false);
            isActivate = true;
            GameManager.Instance.GetKey();
        }
    }
}
