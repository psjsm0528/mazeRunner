using UnityEngine;

public class AttachCube : MonoBehaviour
{
    public GameObject character;
    public GameObject cube;

    void Start()
    {
        cube.transform.parent = character.transform; // �θ� ����
        cube.transform.localPosition = new Vector3(0, 2, 0); // ĳ���� ���� ��ġ
    }
}
