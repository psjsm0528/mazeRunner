using UnityEngine;

public class AttachCube : MonoBehaviour
{
    public GameObject character;
    public GameObject cube;

    void Start()
    {
        cube.transform.parent = character.transform; // 부모 설정
        cube.transform.localPosition = new Vector3(0, 2, 0); // 캐릭터 기준 위치
    }
}
