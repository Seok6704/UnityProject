using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
메인 카메라 시점 관련 스크립트, 디폴트는 캐릭터를 쫓게 설정.
*/

public class MainCamera : MonoBehaviour
{
    public GameObject character; // 카메라가 쫓을 캐릭터

    [Header("카메라 시점 위치")]
    public float posX;
    public float posY;
    public float posZ;
    [Header("카메라 시점 각도")]
    public float rotX;
    public float rotY;
    public float rotZ;

    void Awake()
    {
        transform.eulerAngles = new Vector3(rotX, rotY, rotZ); // 카메라의 각도를 설정한 각도로 변경.
    }

    void Update()
    {
        transform.position = character.transform.position + new Vector3(posX, posY, posZ); // 카메라의 위치를 매 프레임마다 캐릭터의 현재 위치로 이동, 원하는 시점을 위해 x축, y축 z축 값을 설정.
    }
}
