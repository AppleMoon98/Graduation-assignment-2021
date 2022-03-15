using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomStatus : MonoBehaviour
{
    public RectTransform uiGroup;

    // Data
    public enum UnitType { Basic, Percent };
    public Sprite[] statusImgs;

    // Button
    public Image[] imgs;
    public Text[] txts;

    public Player player;

    public int buttonNum;
    public int[] buttonData = new int[6];

    string[] statusTxt = { "최대 체력", "물약 회복량", "이동 속도", "데미지", "공격 속도", "리로드 속도", "최대 탄약", "사거리" };
    int[,] data = { { 0, 5, 20 },          // 최대 체력
                      { 1, 5, 10 },          // 물약 회복량
                      { 1, 3, 7 },           // 이동 속도
                      { 1, 5, 20},          // 데미지
                      { 1, 3, 7 },          // 공격 속도
                      { 1, 5, 10 },         // 리로드 속도
                      { 1, 5, 10 },         // 최대 탄약
                      { 1, 3, 7 }           // 사거리
    };
    // data = { 증가 타입, 최소 증가량, 최대 증가량 };
    int randomStatus;


    public void Enter()
    {
        player.isClear = true;
        uiGroup.anchoredPosition = Vector3.zero;
        RandomSwitch();
    }

    public void Exit()
    {
        player.isClear = false;
        uiGroup.anchoredPosition = Vector3.down * 1000;
    }

    void RandomSwitch()
    {
        for (int i = 0; i < 3; i++)
        {
            randomStatus = Random.Range(0, statusTxt.Length);   // 랜덤뽑기
            buttonData[i] = randomStatus;   // 버튼에 랜덤값 저장
            imgs[i].sprite = statusImgs[randomStatus];  // 이미지 설정
            txts[i].text = statusTxt[randomStatus];    // 능력 설정
            string Txt = data[randomStatus, 0] == (int)UnitType.Basic ? "" : "%";
            randomStatus = Random.Range(data[randomStatus,1], data[randomStatus, 2] + 1);    // 상승값 설정
            buttonData[i + 3] = randomStatus;           // 랜덤값 초기화 및 버튼에 값 저장
            txts[i + 3].text = "+ " + randomStatus + Txt;   // 능력 상승값 출력
        }
    }

    public void OnClick(int buttonNum)
    {
        switch (buttonData[buttonNum])
        {
            case 0:
                player.statusMaxHealth += buttonData[buttonNum + 3];
                player.health += buttonData[buttonNum + 3];
                break;
            case 1:
                player.statusHealthRecovery += buttonData[buttonNum + 3];
                break;
            case 2:
                player.statusSpeed += buttonData[buttonNum + 3];
                break;
            case 3:
                player.statusDamage += buttonData[buttonNum + 3];
                break;
            case 4:
                player.statusRate += buttonData[buttonNum + 3];
                break;
            case 5:
                player.statusReloadTime += buttonData[buttonNum + 3];
                break;
            case 6:
                player.statusMaxAmmo += buttonData[buttonNum + 3];
                break;
            case 7:
                player.statusRange += buttonData[buttonNum + 3];
                break;
        }

        if (player.equipWeapon != null)
            player.equipWeapon.WeaponReload();

        Exit();
    }
}
