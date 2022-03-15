using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    // Original Var
    public GameObject menuCam;
    public GameObject gameCam;
    public Player player;
    public Boss boss;
    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject startZone;
    public RandomStatus randomStatus;
    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCutA;
    public int enemyCutB;
    public int enemyCutC;
    public int enemyCutD;

    public Transform[] enemyZones;
    public GameObject[] enemies;
    public List<int> enemyList;

    // TITLE UI
    public GameObject menuPanel;
    public Text maxScoreTxt;
    public Text timeTxt;

    // In Game UI
    public GameObject gamePanel;
    public Text scoreTxt;
    public Text stageTxt;
    public Text playTimeTxt;
    public Text playerHealthTxt;
    public Text playerAmmoTxt;
    public Text playerCoinTxt;
    public Image[] weaponImgs;
    public Image weaponRImg;
    public Text enemyATxt;
    public Text enemyBTxt;
    public Text enemyCTxt;
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;
    public Text bossHealthTxt;
    public Image potionImg;
    public Image potionNumImg;
    public Sprite[] potionNum;

    // Game UI
    public GameObject pausePanel;
    public RectTransform optionPanel;
    public GameObject ClearPanel;
    public GameObject reloadButton;

    // GameOption
    public Dropdown screenDropdown;
    public Dropdown fullscreenDropdown;
    public Toggle autoReloadToggle;
    public Slider[] sliders;
    public AudioMixer audioMixer;
    public string[] soundStr = { "Master", "BGM", "SE" };

    // Game Over UI
    public GameObject overPanel;
    public Text curScoreText;
    public Text bestText;
    public Text curTimeText;

    public Shop weaponShopData;
    public Material floor;

    void Awake()
    {
        enemyList = new List<int>();
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));
        playTime = PlayerPrefs.GetFloat("Time");

        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int second = (int)(playTime % 60);
        timeTxt.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second);

        // 스코어
        if (!PlayerPrefs.HasKey("MaxScore"))
            PlayerPrefs.SetInt("MaxScore", 0);

        if (!PlayerPrefs.HasKey("Time"))
            PlayerPrefs.SetFloat("Time", 0f);

        // 플레이어 옵션 정보 초기화 및 로드
        PlayerOptionReload();

        // 포션 리로드
        PotionReload(player.potion);

        // Floor 색 설정
        floor.color = new Vector4(0.55f, 0.72f, 1, 1);
    }

    public void PlayerOptionReload()
    {
        if (!PlayerPrefs.HasKey("FullScreenMode"))
            PlayerPrefs.SetInt("FullScreenMode", 1);
        else { fullscreenDropdown.value = PlayerPrefs.GetInt("FullScreenMode"); FullScreenModeChange(); }
        if (!PlayerPrefs.HasKey("ScreenSize"))
            PlayerPrefs.SetInt("ScreenSize", 0);
        else { screenDropdown.value = PlayerPrefs.GetInt("ScreenSize"); ScreenSizeChange(); }
        if (!PlayerPrefs.HasKey("AutoReload"))
            PlayerPrefs.SetInt("AutoReload", 1);
        else autoReloadToggle.isOn = PlayerPrefs.GetInt("AutoReload") == 1 ? true : false;
        if (!PlayerPrefs.HasKey("MasterVolume"))
            PlayerPrefs.SetFloat("MasterVolume", 0);
        if (!PlayerPrefs.HasKey("BGMVolume"))
            PlayerPrefs.SetFloat("BGMVolume", 0);
        if (!PlayerPrefs.HasKey("SEVolume"))
            PlayerPrefs.SetFloat("SEVolume", 0);
        for (int i = 0; i < sliders.Length; i++)
            sliders[i].value = PlayerPrefs.GetFloat(soundStr[i]);
    }

    public void PotionReload(int potionNum)
    {
        if (potionNum > 0)
            potionNumImg.sprite = this.potionNum[potionNum - 1];
        else
        {
            potionNumImg.enabled = false;
            potionImg.color = Color.gray;
        }
    }

    public void GameStart()
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        gamePanel.SetActive(false);
        overPanel.SetActive(true);
        curScoreText.text = scoreTxt.text;
        curTimeText.text = playTimeTxt.text;

        int maxScore = PlayerPrefs.GetInt("MaxScore");
        float TimeScore = PlayerPrefs.GetFloat("Time");
        if (player.score > maxScore)
        {
            bestText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.score);
            PlayerPrefs.SetFloat("Time", playTime);
        }
        else if (player.score == maxScore)
            if (TimeScore > playTime)
                PlayerPrefs.SetFloat("Time", playTime);
    }

    public void GamePause(bool isPause)
    {
        pausePanel.SetActive(isPause);
        if (isPause)
            Time.timeScale = 0;
        else
        {
            Time.timeScale = 1;
            OptionGroupClose();
        }

        player.isPause = !isPause;
    }

    public void AutoReload(Toggle isAutoReload)
    {
        bool check = isAutoReload.isOn;

        player.isAutoReload = check;
        reloadButton.SetActive(!check);
        PlayerPrefs.SetInt("AutoReload", check ? 1 : 0);
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void StartStage()
    {
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);

        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(true);

        // #. Tag : Item 탐색 >> 삭제
        GameObject[] itemObjects = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject itemObject in itemObjects)
            Destroy(itemObject);

        if (stage == 20)
        {
            floor.color = Color.black;
            foreach (Transform zone in enemyZones)
                zone.gameObject.SetActive(false);
        }

        isBattle = true;
        StartCoroutine(InBattle());
    }

    public void StageEnd()
    {
        if (stage >= 20)
        {
            StageClear();
            return;
        }

        player.transform.position = Vector3.up * 0.8f;
        player.coin += stage > 10 ? 2000 : 1000;
        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        weaponShopData.itemReload();
        startZone.SetActive(true);

        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(false);

        // #. Tag : EnemyBullet 탐색 >> 삭제
        GameObject[] bulletObjects = GameObject.FindGameObjectsWithTag("EnemyBullet");
        foreach (GameObject bulletObject in bulletObjects)
            Destroy(bulletObject);
        bulletObjects = GameObject.FindGameObjectsWithTag("BossBullet");
        foreach (GameObject bulletObject in bulletObjects)
            Destroy(bulletObject);

        isBattle = false;
        randomStatus.Enter();
        int reAmmo = player.ammo + player.equipWeapon.curAmmo < player.equipWeapon.maxAmmo ? player.ammo + player.equipWeapon.curAmmo : player.equipWeapon.maxAmmo;
        player.ammo -= player.ammo < player.equipWeapon.maxAmmo ? player.ammo : player.equipWeapon.maxAmmo - player.equipWeapon.curAmmo;
        player.equipWeapon.curAmmo = reAmmo;
        stage++;
    }

    public void StageClear()
    {
        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(false);
        floor.color = new Vector4(0.55f, 0.72f, 1, 1);

        // #. Tag : EnemyBullet 탐색 >> 삭제
        GameObject[] bulletObjects = GameObject.FindGameObjectsWithTag("EnemyBullet");
        foreach (GameObject bulletObject in bulletObjects)
            Destroy(bulletObject);
        bulletObjects = GameObject.FindGameObjectsWithTag("BossBullet");
        foreach (GameObject bulletObject in bulletObjects)
            Destroy(bulletObject);

        GameOver();
    }

    IEnumerator InBattle()
    {
        int stagemob = 0;
        if (stage % 20 == 0)
        {
            enemyCutD++;
            GameObject instantEnemy = Instantiate(enemies[9], /*enemyZones[0].position*/ new Vector3(1, 3, 1), enemyZones[0].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.target = player.transform;
            enemy.manager = this;
            boss = instantEnemy.GetComponent<Boss>();
        }
        else if (stage % 15 == 0)
        {
            enemyCutD++;
            GameObject instantEnemy = Instantiate(enemies[8], /*enemyZones[0].position*/ new Vector3(1, 3, 1), enemyZones[0].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.target = player.transform;
            enemy.manager = this;
            boss = instantEnemy.GetComponent<Boss>();
        }
        else if (stage % 10 == 0)
        {
            enemyCutD++;
            GameObject instantEnemy = Instantiate(enemies[4], /*enemyZones[0].position*/ Vector3.one, enemyZones[0].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.target = player.transform;
            enemy.manager = this;
            boss = instantEnemy.GetComponent<Boss>();
        }
        else if (stage % 5 == 0)
        {
            enemyCutD++;
            GameObject instantEnemy = Instantiate(enemies[3], /*enemyZones[0].position*/ Vector3.one, enemyZones[0].rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.target = player.transform;
            enemy.manager = this;
            boss = instantEnemy.GetComponent<Boss>();
        }
        else
        {
            for (int index = 0; index < (stage > 10 ? stage / 2 : stage); index++)
            {
                int ran = Random.Range(0, 3);
                enemyList.Add(ran);

                switch (ran)
                {
                    case 0:
                        enemyCutA++;
                        break;
                    case 1:
                        enemyCutB++;
                        break;
                    case 2:
                        enemyCutC++;
                        break;
                }
            }

            while (enemyList.Count > 0)
            {
                int ranZone = Random.Range(0, 4);
                if (stage > 10)
                    stagemob = 5;
                GameObject instantEnemy = Instantiate(enemies[enemyList[0] + stagemob],
                                                    enemyZones[ranZone].position, enemyZones[ranZone].rotation);
                Enemy enemy = instantEnemy.GetComponent<Enemy>();
                enemy.target = player.transform;
                enemy.manager = this;
                enemyList.RemoveAt(0);
                yield return new WaitForSeconds(stage > 5 ? 1.5f : 4f);
            }
        }

        while (enemyCutA + enemyCutB + enemyCutC + enemyCutD > 0)
        {
            yield return null;
        }

        ClearPanel.SetActive(true);
        yield return new WaitForSeconds(4f);
        ClearPanel.SetActive(false);
        boss = null;
        StageEnd();
    }

    void Update()
    {
        if (isBattle)
            playTime += Time.deltaTime;

        if (boss != null)
        {
            int bossHealth = (int)boss.curHealth;
            int bossMaxHealth = (int)boss.maxHealth;
            bossHealthTxt.text = bossHealth + " / " + bossMaxHealth;
        }
    }

    void LateUpdate()
    {
        int weaponImgNum = 0;
        // 상단 UI
        scoreTxt.text = string.Format("{0:n0}", player.score);
        stageTxt.text = "STAGE " + stage;

        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int second = (int)(playTime % 60);
        playTimeTxt.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second);

        // 플레이어 UI
        playerHealthTxt.text = player.health + " / " + player.maxHealth;
        playerCoinTxt.text = string.Format("{0:n0}", player.coin);
        if (player.equipWeapon == null || player.equipWeapon.type == Weapon.Type.Melee)
            playerAmmoTxt.text = "- / " + player.ammo;
        else
            playerAmmoTxt.text = player.equipWeapon.curAmmo + " / " + player.ammo;

        // 무기 UI
        foreach (Image weaponImg in weaponImgs)
            weaponImg.color = new Color(1, 1, 1, player.hasWeapons[weaponImgNum++] ? 1 : 0);
        weaponRImg.color = new Color(1, 1, 1, player.hasGrenade > 0 ? 1 : 0);

        // 몬스터 숫자 UI
        enemyATxt.text = "x " + enemyCutA.ToString();
        enemyBTxt.text = "x " + enemyCutB.ToString();
        enemyCTxt.text = "x " + enemyCutC.ToString();

        // 보스 체력 UI
        if (boss != null)
        {
            bossHealthGroup.anchoredPosition = Vector3.down * 30;
            float bossHealthPer = (float)boss.curHealth / boss.maxHealth;
            float bossHealth = bossHealthPer > 1 ? 1 : bossHealthPer < 0 ? 0 : bossHealthPer;
            bossHealthBar.localScale = new Vector3(bossHealth, 1, 1);
        }
        else
        {
            bossHealthGroup.anchoredPosition = Vector3.up * 200;
        }
    }

    public void GameExit()
    {
        Application.Quit();
    }

    public void OptionGroupOpen()
    {
        optionPanel.anchoredPosition = new Vector3(0, -180);
    }

    public void OptionGroupClose()
    {
        optionPanel.anchoredPosition = new Vector3(0, -1000);
        for (int i = 0; i < sliders.Length; i++)
            PlayerPrefs.SetFloat(soundStr[i], sliders[i].value);
    }

    public void ScreenSizeChange()  // 화면 사이즈 체인지
    {
        switch (screenDropdown.value)
        {
            case 0:
                Screen.SetResolution(1920, 1080, Screen.fullScreen);
                PlayerPrefs.SetInt("ScreenSize", 0);
                break;
            case 1:
                Screen.SetResolution(1600, 900, Screen.fullScreen);
                PlayerPrefs.SetInt("ScreenSize", 1);
                break;
            case 2:
                Screen.SetResolution(1280, 720, Screen.fullScreen);
                PlayerPrefs.SetInt("ScreenSize", 2);
                break;
            case 3:
                Screen.SetResolution(960, 540, Screen.fullScreen);
                PlayerPrefs.SetInt("ScreenSize", 3);
                break;
        }
    }

    public void FullScreenModeChange()  // 풀스크린 모드 체인지
    {
        switch (fullscreenDropdown.value)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                PlayerPrefs.SetInt("FullScreenMode", 0);
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                PlayerPrefs.SetInt("FullScreenMode", 1);
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                PlayerPrefs.SetInt("FullScreenMode", 2);
                break;
        }
    }

    public void JoyStickAttackUp()
    {
        player.AttackButtonUp();
    }

    public void JoyStickAttackDown()
    {
        player.AttackButtonDown();
    }

    public void JoyStickReload()
    {
        player.ReloadButton();
    }

    public void JoyStickDodge()
    {
        player.DodgeButton();
    }

    public void JoyStickPotion()
    {
        player.PotionButton();
    }

    public void JoyStickSwap1()
    {
        player.Swap1Button();
    }

    public void JoyStickSwap2()
    {
        player.Swap2Button();
    }

    public void JoyStickSwap3()
    {
        player.Swap3Button();
    }

    public void JoyStickGrenade()
    {
        player.GrenadeButton();
    }

    public void UpCheat()
    {
        player.statusMaxHealth = 9850;
        player.coin = 10000000;
        player.statusDamage = 200;
        player.statusRate = 50;
        player.statusSpeed = 20;
        player.health = player.maxHealth;
        stage += 1;
    }

    public void DownCheat()
    {
        stage -= 1;
    }
}
