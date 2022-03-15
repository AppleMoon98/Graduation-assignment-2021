using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipUi : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum Type { Melee, Range, Special };
    public Type type;

    public GameManager manager;
    public Player player;

    public Weapon[] weapons;    // 해당 칸에 들어가는 무기를 넣어둠

    public GameObject itemDataGroup;
    public RectTransform itemDataRectTransform;
    public Text[] itemDataText = new Text[7];   // {이름, 등급, 데미지, 공속, 사거리, 재장전속도, 최탄수}

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (type == Type.Melee)
        {
            itemDataRectTransform.sizeDelta = new Vector2(500, 310);
            if (player.hasWeapons[0])
            {
                Weapon weapon = weapons[0];
                weapon.WeaponReload();
                itemDataText[0].text = "해 머";
                itemDataText[1].text = "등급 : " + weapon.weaponClass;
                itemDataText[2].text = "데미지 : " + weapon.damage;
                itemDataText[3].text = "공격속도 : " + string.Format("{0:F2}", weapon.rate) + "s";
                itemDataText[4].text = "사거리 : " + string.Format("{0:F2}", weapon.meleeArea.size.y);
                itemDataText[5].text = "";
                itemDataText[6].text = "";
            }
            else if (player.hasWeapons[3])
            {
                Weapon weapon = weapons[1];
                weapon.WeaponReload();
                itemDataText[0].text = "파이어 스피어";
                itemDataText[1].text = "등급 : " + weapon.weaponClass;
                itemDataText[2].text = "데미지 : " + weapon.damage;
                itemDataText[3].text = "공격속도 : " + string.Format("{0:F2}", weapon.rate) + "s";
                itemDataText[4].text = "사거리 : " + string.Format("{0:F2}", weapon.meleeArea.size.y);
                itemDataText[5].text = "";
                itemDataText[6].text = "";
            }
            else if (player.hasWeapons[4])
            {
                Weapon weapon = weapons[2];
                weapon.WeaponReload();
                itemDataText[0].text = "화염 주먹";
                itemDataText[1].text = "등급 : " + weapon.weaponClass;
                itemDataText[2].text = "데미지 : " + weapon.damage;
                itemDataText[3].text = "공격속도 : " + string.Format("{0:F2}", weapon.rate) + "s";
                itemDataText[4].text = "사거리 : " + string.Format("{0:F2}", weapon.meleeArea.size.y);
                itemDataText[5].text = "";
                itemDataText[6].text = "";
            }
            else if (player.hasWeapons[5])
            {
                Weapon weapon = weapons[3];
                weapon.WeaponReload();
                itemDataText[0].text = "냉기의 심장";
                itemDataText[1].text = "등급 : " + weapon.weaponClass;
                itemDataText[2].text = "데미지 : " + weapon.damage;
                itemDataText[3].text = "공격속도 : " + string.Format("{0:F2}", weapon.rate) + "s";
                itemDataText[4].text = "사거리 : " + string.Format("{0:F2}", weapon.meleeArea.size.y);
                itemDataText[5].text = "";
                itemDataText[6].text = "";
            }
            else if (player.hasWeapons[6])
            {
                Weapon weapon = weapons[4];
                weapon.WeaponReload();
                itemDataText[0].text = "아이스 스피어";
                itemDataText[1].text = "등급 : " + weapon.weaponClass;
                itemDataText[2].text = "데미지 : " + weapon.damage;
                itemDataText[3].text = "공격속도 : " + string.Format("{0:F2}", weapon.rate) + "s";
                itemDataText[4].text = "사거리 : " + string.Format("{0:F2}", weapon.meleeArea.size.y);
                itemDataText[5].text = "";
                itemDataText[6].text = "";
            }
            else return;
        }
        else if (type == Type.Range)
        {
            itemDataRectTransform.sizeDelta = new Vector2(500, 410);
            if (player.hasWeapons[1])
            {
                Weapon weapon = weapons[0];
                weapon.WeaponReload();
                itemDataText[0].text = "핸드건";
                itemDataText[1].text = "등급 : " + weapon.weaponClass;
                itemDataText[2].text = "데미지 : " + weapon.damage;
                itemDataText[3].text = "공격속도 : " + string.Format("{0:F2}", weapon.rate) + "s";
                itemDataText[4].text = "탄속 : " + weapon.basicBulletSpeed + " + " + string.Format("{0:F2}", (float)weapon.basicBulletSpeed * weapon.range);
                itemDataText[5].text = "재장전 시간 : " + string.Format("{0:F2}", weapon.reloadTime) + "s";
                itemDataText[6].text = "최대 탄약 : " + weapon.maxAmmo;
            }
            else if (player.hasWeapons[2])
            {
                Weapon weapon = weapons[1];
                weapon.WeaponReload();
                itemDataText[0].text = "서브머신건";
                itemDataText[1].text = "등급 : " + weapon.weaponClass;
                itemDataText[2].text = "데미지 : " + weapon.damage;
                itemDataText[3].text = "공격속도 : " + string.Format("{0:F2}", weapon.rate) + "s";
                itemDataText[4].text = "탄속 : " + weapon.basicBulletSpeed + " + " + string.Format("{0:F2}", (float)weapon.basicBulletSpeed * weapon.range);
                itemDataText[5].text = "재장전 시간 : " + string.Format("{0:F2}", weapon.reloadTime) + "s";
                itemDataText[6].text = "최대 탄약 : " + weapon.maxAmmo;
            }
            else if (player.hasWeapons[7])
            {
                Weapon weapon = weapons[2];
                weapon.WeaponReload();
                itemDataText[0].text = "파이어 핸드건";
                itemDataText[1].text = "등급 : " + weapon.weaponClass;
                itemDataText[2].text = "데미지 : " + weapon.damage;
                itemDataText[3].text = "공격속도 : " + string.Format("{0:F2}", weapon.rate) + "s";
                itemDataText[4].text = "탄속 : " + weapon.basicBulletSpeed + " + " + string.Format("{0:F2}", (float)weapon.basicBulletSpeed * weapon.range);
                itemDataText[5].text = "재장전 시간 : " + string.Format("{0:F2}", weapon.reloadTime) + "s";
                itemDataText[6].text = "최대 탄약 : " + weapon.maxAmmo;
            }
            else if (player.hasWeapons[8])
            {
                Weapon weapon = weapons[3];
                weapon.WeaponReload();
                itemDataText[0].text = "아이스 핸드건";
                itemDataText[1].text = "등급 : " + weapon.weaponClass;
                itemDataText[2].text = "데미지 : " + weapon.damage;
                itemDataText[3].text = "공격속도 : " + string.Format("{0:F2}", weapon.rate) + "s";
                itemDataText[4].text = "탄속 : " + weapon.basicBulletSpeed + " + " + string.Format("{0:F2}", (float)weapon.basicBulletSpeed * weapon.range);
                itemDataText[5].text = "재장전 시간 : " + string.Format("{0:F2}", weapon.reloadTime) + "s";
                itemDataText[6].text = "최대 탄약 : " + weapon.maxAmmo;
            }
            else if (player.hasWeapons[9])
            {
                Weapon weapon = weapons[4];
                weapon.WeaponReload();
                itemDataText[0].text = "파이어 머신건";
                itemDataText[1].text = "등급 : " + weapon.weaponClass;
                itemDataText[2].text = "데미지 : " + weapon.damage;
                itemDataText[3].text = "공격속도 : " + string.Format("{0:F2}", weapon.rate) + "s";
                itemDataText[4].text = "탄속 : " + weapon.basicBulletSpeed + " + " + string.Format("{0:F2}", (float)weapon.basicBulletSpeed * weapon.range);
                itemDataText[5].text = "재장전 시간 : " + string.Format("{0:F2}", weapon.reloadTime) + "s";
                itemDataText[6].text = "최대 탄약 : " + weapon.maxAmmo;
            }
            else if (player.hasWeapons[10])
            {
                Weapon weapon = weapons[5];
                weapon.WeaponReload();
                itemDataText[0].text = "아이스 머신건";
                itemDataText[1].text = "등급 : " + weapon.weaponClass;
                itemDataText[2].text = "데미지 : " + weapon.damage;
                itemDataText[3].text = "공격속도 : " + string.Format("{0:F2}", weapon.rate) + "s";
                itemDataText[4].text = "탄속 : " + weapon.basicBulletSpeed + " + " + string.Format("{0:F2}", (float)weapon.basicBulletSpeed * weapon.range);
                itemDataText[5].text = "재장전 시간 : " + string.Format("{0:F2}", weapon.reloadTime) + "s";
                itemDataText[6].text = "최대 탄약 : " + weapon.maxAmmo;
            }
            else return;
        }
        itemDataGroup.transform.position = eventData.position;
        itemDataGroup.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        itemDataGroup.SetActive(false);
    }
}
