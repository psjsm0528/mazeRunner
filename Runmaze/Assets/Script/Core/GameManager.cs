using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<ExitItem> exitItems = new List<ExitItem>();
    public List<GameObject> lockedWall = new List<GameObject>();
    // 탈출 아이템을 모든 활성화 했을 때 CanExit를 활성화 시킨다.
    public bool CanExit = false;
    public TextMeshProUGUI getItemList; // 소유한 아이템의 이름을 출력하는 UI

    private void Start()
    {
        Instance = this;
    }
    public void UpdateItem()
    {
        getItemList.text = "획득한 아이템 리스트:";
        for (int i = 0; i < exitItems.Count; i++)
        {
            if (exitItems[i].isActivate == true)
            {
                getItemList.text += exitItems[i].Itemname;
            }
        }
    }

     public bool GetAllkey()
    {
        for (int i = 0; i < exitItems.Count; i++)
        {
            if (exitItems[i].isActivate == false)
            {
                return false;
            }
        }

        return true;
    }

    public void GetKey()
    {
        UpdateItem();
        CanExit = GetAllkey();

        if(CanExit)
        {
            foreach(GameObject g in lockedWall)
            {
                g.SetActive(false);
            }
        }
    }   
      

}