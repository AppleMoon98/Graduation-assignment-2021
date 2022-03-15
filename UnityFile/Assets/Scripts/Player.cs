using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    // #. 변수 선언
    public float speed;
    public Weapon[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public int hasGrenade;
    public GameObject grenadeObj;
    public Camera followCamera;
    public GameManager manager;
    public RandomStatus randomStatus;

    public AudioSource dodgeSound;
    public bool isAutoReload;
    public bool isAutoReloadClick = false;

    // 플레이어 정보
    public int ammo;
    public int coin;
    public int health;
    public int score;
    public float swapSpeed;
    public int potion;

    // 플레이어 정보 최대치
    public int maxHealth;
    public int maxAmmo;
    public int maxCoin;
    public int maxHasGrenade;
    public int maxPotion;

    // 초기 데이터 저장
    int basicMaxHealth;
    float basicSpeed;

    // 랜덤 뽑기로 인한 추가 스텟
    public int statusMaxHealth;
    public int statusHealthRecovery;
    public int statusSpeed;
    public int statusDamage;
    public int statusRate;
    public int statusReloadTime;
    public int statusMaxAmmo;
    public int statusRange;

    public JoyStick moveJoyStick;
    public float hAxis;
    public float vAxis;

    bool wDown;
    bool jDown;
    bool fDown;
    bool gDown;
    bool rDown;
    bool iDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;

    bool keyP_Down;
    bool keyO_Down;
    bool keyI_Down;
    bool keyJ_Down;
    bool keyK_Down;
    bool keyL_Down;
    bool keyESC_Down;

    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isReload;
    bool isFireReady = true;
    bool isBorder;
    bool isDamage;
    bool isShop;
    bool isDead;
    public bool isPause = true;
    public bool isClear;

    bool attacking;

    Vector3 moveVec;
    Vector3 grenadeVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;
    MeshRenderer[] meshs;

    public GameObject nearObject;
    public Weapon equipWeapon;
    int equipWeaponIndex = -1;
    float fireDelay;

    Shop shop;

    void Awake()
    {
        // #. 초기화
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();

        basicMaxHealth = maxHealth;
        basicSpeed = speed;
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        // Jump();
        Grenade();
        Attack();
        Reload();
        Dodge();
        Swap();
        Interation();
        Pause();
        Status();

        // Cheat
        Cheat();

        if (attacking)
            AttackButtonSystem();
    }

    void GetInput()
    {
        // #. 버튼 입력
        //hAxis = Input.GetAxisRaw("Horizontal");
        //vAxis = Input.GetAxisRaw("Vertical");

        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        //fDown = Input.GetButton("Fire1");
        //gDown = Input.GetButtonDown("Fire2");
        rDown = Input.GetButtonDown("Reload");
        iDown = Input.GetButtonDown("Interation");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
        keyESC_Down = Input.GetKeyDown(KeyCode.Escape);

        // Cheat
        keyP_Down = Input.GetKeyDown(KeyCode.P);
        keyO_Down = Input.GetKeyDown(KeyCode.O);
        keyI_Down = Input.GetKeyDown(KeyCode.I);
        keyJ_Down = Input.GetKeyDown(KeyCode.J);
        keyK_Down = Input.GetKeyDown(KeyCode.K);
        keyL_Down = Input.GetKeyDown(KeyCode.L);

    }

    void Move()
    {
        // #. 이동 설정
        // 'normalized' 란?
        // 피타고라스의 정리에 의해 hAxis와 vAxis가 둘 다 1일경우
        // 대각선으로 이동을 할 때 그 거리는 √2 (루트 2) 다.
        // 그 거리를 1로 만들어주는 역할을 한다.
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        // 수류탄 방향 백터값
        if (hAxis != 0 || vAxis != 0)
            grenadeVec = moveVec;

        // 회피시 이동방향 고정
        if (isDodge)
            moveVec = dodgeVec;

        if (isSwap || !isFireReady || isDead || ((fDown && !moveJoyStick.isBool) && !isDodge) || isReload || isClear || !isPause)
            moveVec = Vector3.zero;

        if (!isBorder)
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;

        // #. 이동 애니메이션 입력
        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        // #. 캐릭터 회전 : 나아가는 방향을 바라보도록 설정
        // 키보드에 의한 회전
        transform.LookAt(transform.position + moveVec);

        // 마우스에 의한 회전
        /*if (fDown && equipWeapon != null && !isDead && isPause)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }*/
    }

    void Grenade()
    {
        if (hasGrenade == 0)
            return;

        if (gDown && !isReload && !isSwap && !isDead && !isClear && isPause)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 10;

                GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasGrenade--;
                grenades[hasGrenade].SetActive(false);
            }
        }
    }

    void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate + 0.1f < fireDelay;

        if (fDown && isFireReady && !isDodge && !isSwap && !isReload && !isShop && !isDead && !isClear && isPause)
        {
            // Auto Reload and if Range Weapon ammo is zero
            if (equipWeapon.type == Weapon.Type.Range && equipWeapon.curAmmo < 1)
            {
                if (isAutoReload && !isAutoReloadClick)
                {
                    isAutoReloadClick = true;
                    Reload();
                }
                return;
            }

            // Attack
            fireDelay = 0;
            equipWeapon.Use();
            if (equipWeapon.type == Weapon.Type.Melee)
            {
                switch (equipWeapon.attackType)
                {
                    case Weapon.AttackType.First:
                        anim.speed = 0.4333f / equipWeapon.rate;
                        anim.SetTrigger("doSwing");
                        Invoke(nameof(AnimationSpeed), equipWeapon.rate / 0.999f);
                        break;
                    case Weapon.AttackType.Second:
                        anim.speed = 0.4333f / equipWeapon.rate;
                        anim.SetTrigger("doSideCut");
                        Invoke(nameof(AnimationSpeed), equipWeapon.rate / 0.999f);
                        break;
                }
            }
            else if (equipWeapon.type == Weapon.Type.Range)
            {
                anim.speed = 0.3333f / equipWeapon.rate;
                anim.SetTrigger("doShot");
                Invoke(nameof(AnimationSpeed), equipWeapon.rate / 1f);
            }

            // #. 공격 사운드 실행
            equipWeapon.weaponSound.Play();
        }
    }

    void AnimationSpeed()
    {
        anim.speed = 1;
    }

    void Reload()
    {
        if (equipWeapon == null)
            return;

        if (equipWeapon.type == Weapon.Type.Melee)
            return;

        if (ammo == 0)
            return;

        if (equipWeapon.maxAmmo == equipWeapon.curAmmo)
            return;

        if ((rDown || isAutoReloadClick) && !isJump && !isDodge && !isSwap && isFireReady && !isShop && !isDead && !isReload && !isClear && isPause)
        {
            anim.SetTrigger("doReload");
            isReload = true;
            isAutoReloadClick = false;

            anim.speed = 1.333f / equipWeapon.reloadTime;
            Invoke(nameof(ReloadOut), equipWeapon.reloadTime);
        }
    }

    void ReloadOut()
    {
        int reAmmo = ammo + equipWeapon.curAmmo < equipWeapon.maxAmmo ? ammo + equipWeapon.curAmmo : equipWeapon.maxAmmo;
        ammo -= ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo - equipWeapon.curAmmo;
        equipWeapon.curAmmo = reAmmo;
        AnimationSpeed();
        isReload = false;
    }

    void Dodge()
    {
        // #. 회피 입력
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap && !isDead && isPause)
        {
            dodgeVec = moveVec;

            // #. 속도 변경
            speed *= 2;

            // #. 회피 애니메이션 입력
            anim.SetTrigger("doDodge");
            isDodge = true;

            // #. 회피 사운드 실행
            dodgeSound.Play();

            // #. 회피 종료
            Invoke(nameof(DodgeOut), 0.5f);
        }
    }

    void DodgeOut()
    {
        // #. 회피 탈출
        speed *= 0.5f;
        isDodge = false;
    }

    void Swap()
    {
        bool isTrue = false;
        int weaponIndex = -1;

        // #. 무기가 중복 혹은 얻지 못하였을 경우 return
        if (sDown1 || sDown2 || sDown3)
        {
            foreach (Weapon weapon in weapons)
            {
                if (sDown1 && weapon.type == Weapon.Type.Melee && hasWeapons[weapon.weaponindex] && equipWeaponIndex != weapon.weaponindex)
                    isTrue = true;

                if (sDown2 && weapon.type == Weapon.Type.Range && hasWeapons[weapon.weaponindex] && equipWeaponIndex != weapon.weaponindex)
                    isTrue = true;

                if (sDown3 && weapon.type == Weapon.Type.Special && hasWeapons[weapon.weaponindex] && equipWeaponIndex != weapon.weaponindex)
                    isTrue = true;

                if (isTrue)
                {
                    weaponIndex = weapon.weaponindex;
                    break;
                }
            }
        }

        if (!isTrue)
            return;

        // #. 무기 변경
        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge && !isShop && !isDead && !isClear)
        {
            // #. 기존 장착중인 무기 제거
            if (equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);

            // #. 새로운 무기 생성 및 저장
            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            // #. 애니메이션 입력
            anim.SetTrigger("doSwap");
            anim.speed = 0.4f / swapSpeed;

            // #. 스왑 중...
            isSwap = true;
            isTrue = false;

            // #. 스왑 종료
            Invoke(nameof(SwapOut), swapSpeed);
        }
    }

    void SwapOut()
    {
        AnimationSpeed();
        equipWeapon.WeaponReload();
        isSwap = false;
    }

    void Interation()
    {
        // #. 아이템 습득
        if (iDown && nearObject != null && !isJump && !isDodge && !isDead)
        {
            // #. 습득한 아이템이 무기이다.
            if (nearObject.tag == "Weapon")
            {
                // #. 무기 습득
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                // #. 무기 오브젝트 삭제
                Destroy(nearObject);
            }
            else if (nearObject.tag == "Shop")
            {
                shop = nearObject.GetComponent<Shop>();
                shop.Enter(this);
                isShop = true;
            }
        }
        else if (iDown && nearObject == null && potion > 0)
        {
            if (health >= maxHealth)
                return;

            potion--;
            float recoveryHealth = health + (maxHealth * (0.25f * (1 + statusHealthRecovery / 100)));
            health = recoveryHealth > maxHealth ? maxHealth : (int)recoveryHealth;
            CallPotionReload();
        }
    }

    public void CallPotionReload()
    {
        manager.PotionReload(potion);
    }

    void Pause()
    {
        if (keyESC_Down && !isClear)
            if (!isShop)
                manager.GamePause(isPause);
            else
                shop.Exit();
    }

    void Status()
    {
        if (isDodge)
            return;

        maxHealth = basicMaxHealth + statusMaxHealth;
        speed = basicSpeed * (1 + (float)statusSpeed / 100);
    }

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }

    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }

    void OnCollisionEnter(Collision collision)
    {
        // #. 플레이어 착지 체크
        if (collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyBullet" || other.tag == "BossBullet")
        {
            if (!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                if (maxHealth < health)
                    health = maxHealth;

                health -= enemyBullet.damage;

                bool isBossAtk = other.name == "Boss Melee Area";
                StartCoroutine(onDamage(isBossAtk));
            }

            if (other.GetComponent<Rigidbody>() != null)
                Destroy(other.gameObject);
        }

        // #. 무기 습득 범위 내에 있다.
        if (other.tag == "Weapon" || other.tag == "Shop")
            nearObject = other.gameObject;

        // #. 아이템 지속 획득
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            Destroy(other.gameObject);
            switch (item.type)
            {
                case Item.Type.Ammo:
                    if (ammo == maxAmmo)
                        return;

                    ammo += item.value;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    if (health == maxHealth)
                        return;

                    health += item.value + statusHealthRecovery;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade:
                    grenades[hasGrenade].SetActive(true);
                    hasGrenade += item.value;
                    if (hasGrenade > maxHasGrenade)
                        hasGrenade = maxHasGrenade;
                    break;
            }
        }
    }

    IEnumerator onDamage(bool isBossAtk)
    {
        isDamage = true;

        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }

        if (isBossAtk)
            rigid.AddForce(transform.forward * -45, ForceMode.Impulse);

        if (health <= 0 && !isDead)
            OnDie();

        yield return new WaitForSeconds(1f);

        isDamage = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }

        if (isBossAtk)
            rigid.velocity = Vector3.zero;

    }

    void OnDie()
    {
        anim.SetTrigger("doDie");
        isDead = true;
        manager.GameOver();
    }

    void OnTriggerExit(Collider other)
    {
        // #. 무기 습득 범위를 벗어났다.
        if (other.tag == "Weapon")
            nearObject = null;
        else if (other.tag == "Shop")
        {
            Shop shop = nearObject.GetComponent<Shop>();
            if (shop != null)
                shop.Exit();
            isShop = false;
            nearObject = null;
        }
    }

    void Cheat()
    {
        if (keyP_Down)
            manager.stage++;

        if (keyO_Down)
            manager.stage--;

        if (keyI_Down)
            coin += 50000;

        if (keyK_Down)
        {
            health -= 100;
            StartCoroutine(onDamage(false));
        }

        if (keyL_Down)
            health += 100;

        if (keyJ_Down)
            randomStatus.Enter();
    }

    public void AttackButtonDown()
    {
        attacking = true;
    }

    public void AttackButtonUp()
    {
        attacking = false;
    }

    public void AttackButtonSystem()
    {
        try
        {
            if (nearObject.name == "StartZone")
                nearObject = null;
        }
        catch (NullReferenceException e)
        {
            Debug.Log(e);
        }

        if (nearObject != null && !isJump && !isDodge && !isDead)   // ShopButton
            ShopButton();
        else if (isFireReady && !isDodge && !isSwap && !isReload && !isShop && !isDead && !isClear && isPause && equipWeapon != null)
        {
            // Auto Reload and if Range Weapon ammo is zero
            if (equipWeapon.type == Weapon.Type.Range && equipWeapon.curAmmo < 1)
            {
                if (isAutoReload && !isAutoReloadClick)
                {
                    isAutoReloadClick = true;
                    Reload();
                }
                return;
            }

            // Attack
            fireDelay = 0;
            equipWeapon.Use();
            if (equipWeapon.type == Weapon.Type.Melee)
            {
                switch (equipWeapon.attackType)
                {
                    case Weapon.AttackType.First:
                        anim.speed = 0.4333f / equipWeapon.rate;
                        anim.SetTrigger("doSwing");
                        Invoke(nameof(AnimationSpeed), equipWeapon.rate / 0.999f);
                        break;
                    case Weapon.AttackType.Second:
                        anim.speed = 0.4333f / equipWeapon.rate;
                        anim.SetTrigger("doSideCut");
                        Invoke(nameof(AnimationSpeed), equipWeapon.rate / 0.999f);
                        break;
                }
            }
            else if (equipWeapon.type == Weapon.Type.Range)
            {
                anim.speed = 0.3333f / equipWeapon.rate;
                anim.SetTrigger("doShot");
                Invoke(nameof(AnimationSpeed), equipWeapon.rate / 1f);
            }

            // #. 공격 사운드 실행
            equipWeapon.weaponSound.Play();
        }
    }

    public void ReloadButton()
    {
        if (equipWeapon == null)
            return;

        if (equipWeapon.type == Weapon.Type.Melee)
            return;

        if (ammo == 0)
            return;

        if (equipWeapon.maxAmmo == equipWeapon.curAmmo)
            return;

        if (!isJump && !isDodge && !isSwap && isFireReady && !isShop && !isDead && !isReload && !isClear && isPause)
        {
            anim.SetTrigger("doReload");
            isReload = true;
            isAutoReloadClick = false;

            anim.speed = 1.333f / equipWeapon.reloadTime;
            Invoke(nameof(ReloadOut), equipWeapon.reloadTime);
        }
    }

    public void DodgeButton()
    {
        if (moveVec != Vector3.zero && !isJump && !isDodge && !isSwap && !isDead && isPause)
        {
            dodgeVec = moveVec;

            // #. 속도 변경
            speed *= 2;

            // #. 회피 애니메이션 입력
            anim.SetTrigger("doDodge");
            isDodge = true;

            // #. 회피 사운드 실행
            dodgeSound.Play();

            // #. 회피 종료
            Invoke(nameof(DodgeOut), 0.5f);
        }
    }

    public void ShopButton()
    {
        if (nearObject.tag == "Shop")
        {
            shop = nearObject.GetComponent<Shop>();
            shop.Enter(this);
            isShop = true;
        }
    }

    public void PotionButton()
    {
        if (potion > 0)
        {
            if (health >= maxHealth)
                return;

            potion--;
            float recoveryHealth = health + (maxHealth * (0.25f * (1 + statusHealthRecovery / 100)));
            health = recoveryHealth > maxHealth ? maxHealth : (int)recoveryHealth;
            CallPotionReload();
        }
    }

    public void Swap1Button()
    {
        bool isTrue = false;
        int weaponIndex = -1;

        // #. 무기가 중복 혹은 얻지 못하였을 경우 return

        foreach (Weapon weapon in weapons)
        {
            if (weapon.type == Weapon.Type.Melee && hasWeapons[weapon.weaponindex] && equipWeaponIndex != weapon.weaponindex)
                isTrue = true;

            if (isTrue)
            {
                weaponIndex = weapon.weaponindex;
                break;
            }
        }

        if (!isTrue)
            return;

        // #. 무기 변경
        if (!isJump && !isDodge && !isShop && !isDead && !isClear)
        {
            // #. 기존 장착중인 무기 제거
            if (equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);

            // #. 새로운 무기 생성 및 저장
            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            // #. 애니메이션 입력
            anim.SetTrigger("doSwap");
            anim.speed = 0.4f / swapSpeed;

            // #. 스왑 중...
            isSwap = true;
            isTrue = false;

            // #. 스왑 종료
            Invoke(nameof(SwapOut), swapSpeed);
        }
    }

    public void Swap2Button()
    {
        bool isTrue = false;
        int weaponIndex = -1;

        // #. 무기가 중복 혹은 얻지 못하였을 경우 return

        foreach (Weapon weapon in weapons)
        {
            if (weapon.type == Weapon.Type.Range && hasWeapons[weapon.weaponindex] && equipWeaponIndex != weapon.weaponindex)
                isTrue = true;

            if (isTrue)
            {
                weaponIndex = weapon.weaponindex;
                break;
            }
        }

        if (!isTrue)
            return;

        // #. 무기 변경
        if (!isJump && !isDodge && !isShop && !isDead && !isClear)
        {
            // #. 기존 장착중인 무기 제거
            if (equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);

            // #. 새로운 무기 생성 및 저장
            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            // #. 애니메이션 입력
            anim.SetTrigger("doSwap");
            anim.speed = 0.4f / swapSpeed;

            // #. 스왑 중...
            isSwap = true;
            isTrue = false;

            // #. 스왑 종료
            Invoke(nameof(SwapOut), swapSpeed);
        }
    }

    public void Swap3Button()
    {
        bool isTrue = false;
        int weaponIndex = -1;

        // #. 무기가 중복 혹은 얻지 못하였을 경우 return

        foreach (Weapon weapon in weapons)
        {
            if (weapon.type == Weapon.Type.Special && hasWeapons[weapon.weaponindex] && equipWeaponIndex != weapon.weaponindex)
                isTrue = true;

            if (isTrue)
            {
                weaponIndex = weapon.weaponindex;
                break;
            }
        }

        if (!isTrue)
            return;

        // #. 무기 변경
        if (!isJump && !isDodge && !isShop && !isDead && !isClear)
        {
            // #. 기존 장착중인 무기 제거
            if (equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);

            // #. 새로운 무기 생성 및 저장
            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            // #. 애니메이션 입력
            anim.SetTrigger("doSwap");
            anim.speed = 0.4f / swapSpeed;

            // #. 스왑 중...
            isSwap = true;
            isTrue = false;

            // #. 스왑 종료
            Invoke(nameof(SwapOut), swapSpeed);
        }
    }

    public void GrenadeButton()
    {
        if (hasGrenade == 0)
            return;

        if (!isReload && !isSwap && !isDead && !isClear && isPause)
        {
            GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);
            Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
            rigidGrenade.AddForce(grenadeVec * 20 + new Vector3(0,10), ForceMode.Impulse);
            rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

            hasGrenade--;
            grenades[hasGrenade].SetActive(false);
        }
    }

    public void PauseButton()
    {
        if (!isClear)
            if (!isShop)
                manager.GamePause(isPause);
            else
                shop.Exit();
    }

    public void HealthDown()
    {
        health -= maxHealth/4;
        StartCoroutine(onDamage(false));
    }
}
