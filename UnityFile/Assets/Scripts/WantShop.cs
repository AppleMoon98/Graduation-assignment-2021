using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WantShop : MonoBehaviour
{
    public Player player;
    public RectTransform IObj;

    public int price = 0;
    public int getWeapon = -1;
    public int newWeapon = -1;
    public int weaponClass = 0;

    public void Enter(int price, int getWeapon, int newWeapon, int weaponClass)
    {
        IObj.anchoredPosition = Vector3.zero;
        this.price = price;
        this.getWeapon = getWeapon;
        this.newWeapon = newWeapon;
        this.weaponClass = weaponClass;
    }

    public void ButtonNo()
    {
        IObj.anchoredPosition = Vector3.down * 1000;
    }

    public void ButtonYes()
    {
        IObj.anchoredPosition = Vector3.down * 1000;
        player.coin -= price;
        player.hasWeapons[getWeapon] = false;
        Weapon weapon = player.weapons[getWeapon];

        // 기존 무기 변동 정보 초기화
        if (weapon.meleeArea != null)
        {
            weapon.meleeArea.size = weapon.basicSize;
            weapon.meleeArea.center = weapon.basicCenter;
        }
        weapon.gameObject.transform.localScale = weapon.basicLocalScale;
        if (weapon.meleeEffect != null)
            weapon.meleeEffect.transform.localScale = weapon.basicEffectScale;


        player.hasWeapons[newWeapon] = true;
        player.weapons[newWeapon].weaponClass = (Weapon.Class)weaponClass;
        player.weapons[newWeapon].DataReload();

        // 들고있는 무기 제거
        if (player.equipWeapon != null)
            player.equipWeapon.gameObject.SetActive(false);
        player.equipWeapon = player.weapons[newWeapon].GetComponent<Weapon>();
        player.equipWeapon.gameObject.SetActive(true);
    }
}
