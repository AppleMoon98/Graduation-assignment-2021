using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    public Animator anim;
    public RandomStatus randomStatus;

    public GameObject[] itemObj;
    public int[] itemPrice;
    public Transform[] itemPos;
    public string[] talkData;
    public Text talkText;
    public GameManager manager;
    Player enterPlayer;

    // 무기상 전용
    public int[] weaponClass = new int[3];
    public WantShop wantShop;
    public Image[] weaponImgs;
    public Text[] nameTxt;
    public Text[] priceTxt;
    public int[] data;

    public Sprite[] weaponsSprite;
    public Weapon.Type[] weaponsType;
    public string[] weaponsName;
    public int[] weaponsPrice;

    void Start()
    {
        itemReload();
    }

    public void Enter(Player player)
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;
    }

    public void itemReload()
    {
        if (weaponsSprite != null)
            RotationItem();
    }

    public void Exit()
    {
        anim.SetTrigger("doHello");
        uiGroup.anchoredPosition = Vector3.down * 1000;
    }

    public void ItemBuy(int index)
    {
        int price = itemPrice[index];
        if (price > enterPlayer.coin)
        {
            StopAllCoroutines();
            StartCoroutine(PoorTalk());
            return;
        }

        enterPlayer.coin -= price;
        Vector3 ranVec = Vector3.right * Random.Range(-3, 3)
                            + Vector3.forward * Random.Range(-3, 3);
        Instantiate(itemObj[index], itemPos[index].position + ranVec, itemPos[index].rotation);
    }

    public void PotionBuy(int index)
    {
        int price = itemPrice[index];
        if (price > enterPlayer.coin)
        {
            StopAllCoroutines();
            StartCoroutine(PoorTalk());
            return;
        }

        if(enterPlayer.potion >= enterPlayer.maxPotion)
        {
            StopAllCoroutines();
            StartCoroutine(RichTalk());
            return;
        }

        enterPlayer.coin -= price;
        manager.potionNumImg.enabled = true;
        manager.potionImg.color = Color.white;
        manager.PotionReload(++enterPlayer.potion);
    }

    public void StatusBuy(int index)
    {
        int price = itemPrice[index];
        if (price > enterPlayer.coin)
        {
            StopAllCoroutines();
            StartCoroutine(PoorTalk());
            return;
        }

        enterPlayer.coin -= price;
        Exit();
        randomStatus.Enter();
    }

    public void WeaponBuy(int index)
    {
        int price = itemPrice[index];
        if (price > enterPlayer.coin)
        {
            StopAllCoroutines();
            StartCoroutine(PoorTalk());
            return;
        }
        /*else if (enterPlayer.hasWeapons[data[index]])
        {
            StopAllCoroutines();
            StartCoroutine(RichTalk());
            return;
        } */
        else 
            for(int i = 0; i < enterPlayer.hasWeapons.Length; i++)
            {
                if (weaponsType[data[index]] == enterPlayer.weapons[i].type && enterPlayer.hasWeapons[i])
                {
                    wantShop.Enter(price, i, data[index], weaponClass[index]);
                    return;
                }
            }

        enterPlayer.coin -= price;
        enterPlayer.weapons[data[index]].weaponClass = (Weapon.Class)weaponClass[index];
        enterPlayer.weapons[data[index]].DataReload();
        enterPlayer.hasWeapons[data[index]] = true;
    }

    public void RotationItem()
    {
        int R = 0, G = 0, B = 0;
        data = new int[3] { -1, -1, -1 };

        for(int i = 0; i < 3; i++)
        {
            data[i] = Random.Range(0, weaponsSprite.Length);

            // 중복검사
            for (int j = 0; j < i; j++)
            {
                if (data[j] == data[i] && j != i)
                {
                    i--;
                    continue;
                }
            }

            if (weaponImgs.Length == 0)
                return;

            weaponClass[i] = Random.Range(0, 10);
            weaponImgs[i].sprite = weaponsSprite[data[i]];
            nameTxt[i].text = weaponsName[data[i]];
            switch (weaponClass[i])
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    R = 50; G = 50; B = 50;
                    weaponClass[i] = 0;
                    break;
                case 4:
                case 5:
                case 6:
                    R = 51; G = 96; B = 236;
                    weaponClass[i] = 1;
                    break;
                case 7:
                case 8:
                    R = 135; G = 70; B = 209;
                    weaponClass[i] = 2;
                    break;
                case 9:
                    R = 207; G = 207; B = 73;
                    weaponClass[i] = 3;
                    break;
            }
            nameTxt[i].color = new Color(R / 255f, G / 255f, B / 255f, 1);
            priceTxt[i].text = string.Format( "{0:N0}" ,weaponsPrice[data[i]]);
            itemPrice[i] = weaponsPrice[data[i]];
        }
    }

    IEnumerator PoorTalk()
    {
        talkText.text = talkData[1];
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0];
    }

    IEnumerator RichTalk()
    {
        talkText.text = talkData[2];
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0];
    }
}
