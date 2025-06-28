using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<ExitItem> exitItems = new List<ExitItem>();
    public List<GameObject> lockedWall = new List<GameObject>();
    // Ż�� �������� ��� Ȱ��ȭ ���� �� CanExit�� Ȱ��ȭ ��Ų��.
    public bool CanExit = false;
    public TextMeshProUGUI getItemList; // ������ �������� �̸��� ����ϴ� UI

    private void Start()
    {
        Instance = this;
    }
    public void UpdateItem()
    {
        getItemList.text = "ȹ���� ������ ����Ʈ:";
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