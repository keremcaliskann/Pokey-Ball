using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    Vector2 touchDown;
    Vector2 touchUp;

    Ball ball;
    float maxPower;
    float delay;
    float height;

    [SerializeField]
    int level;
    public Transform[] blocks;
    Transform[] levelBlocks;
    Vector3[] levelBlocksPos;
    int blockCount;
    public Transform levelHolder;

    public Transform finish;
    public Text levelText;
    public Text nextLevelText;
    public GameObject gameOver;
    public GameObject win;
    public Slider levelProgress;

    Vector3 defaultBallPos;
    Vector3 defaultCamPos;

    MainCamera mainCamera;

    private void Awake()
    {
        mainCamera = FindObjectOfType<MainCamera>();
        level = PlayerPrefs.GetInt("level") + 1;
        ball = FindObjectOfType<Ball>();
        levelText.text = level.ToString();
        nextLevelText.text = (level + 1).ToString();
        blockCount = level * 2 + 20;
        if (level < 6)
        {
            blockCount = level + 10;
        }
        gameOver.SetActive(false);
        win.SetActive(false);
        BuildLevel(level);
    }

    void Start()
    {
        maxPower = 160;
        delay = 1 / 5f;
    }

    void BuildLevel(int seed)
    {
        Random.InitState(seed);
        levelBlocks = new Transform[blockCount];
        levelBlocksPos = new Vector3[blockCount];
        int i = 0;
        height = -1;
        float distanceBetweenNormal = 0f;
        while (i < blockCount)
        {
            Transform tempBlock = RandomBlock();
            if (i < 2 || i == blockCount - 1)
            {
                tempBlock = blocks[0];
            }
            if (tempBlock.CompareTag("Normal"))
            {
                distanceBetweenNormal = 0;
            }
            else
            {
                distanceBetweenNormal += tempBlock.localScale.y;
                if (distanceBetweenNormal > Random.Range(4, 9))
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        tempBlock = blocks[0];
                        distanceBetweenNormal = 0;
                    }
                    else
                    {
                        tempBlock = blocks[4];
                        distanceBetweenNormal = -4;
                    }
                }
            }
            height += tempBlock.localScale.y;
            levelBlocks[i] = tempBlock;
            levelBlocksPos[i] = StructurePosition(height);
            i++;
        }
        Instantiate(finish, StructurePosition(height + 0.5f), Quaternion.identity);
        for (int j = 0; j < levelBlocks.Length; j++)
        {
            Instantiate(levelBlocks[j], levelBlocksPos[j], Quaternion.identity).SetParent(levelHolder);
        }
    }
    Vector3 StructurePosition(float height)
    {
        Vector3 position = new Vector3(0, height, 1.5f);
        return position;
    }

    void Update()
    {
        float progress = ball.transform.position.y / (height + 1) * 100;
        levelProgress.value = progress;
        if (Input.GetKeyDown(KeyCode.R))
        {
            level = 0;
            Finish();
        }
        if (ball.flying && Input.touchCount > 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                ball.Poke();
            }
        }
        if (!ball.flying)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    defaultBallPos = ball.transform.position;
                    defaultCamPos = mainCamera.transform.position;
                    touchDown = touch.position;
                }
                touchUp = touch.position;
                float swipePower = touchDown.y - touchUp.y;
                float power = Mathf.Clamp(swipePower, 0, maxPower);
                float maxStrainPowerCamera = maxPower;
                if (ball.powerX)
                {
                    power *= 1.5f;
                    maxStrainPowerCamera /= 2;
                }
                if (power > 0)
                {
                    ball.straining = true;
                    Vector3 strainPos = new Vector3(0, defaultBallPos.y - power / maxPower, -1.5f);
                    Vector3 strainCameraPos = new Vector3(1.5f, defaultCamPos.y, defaultCamPos.z - power / maxStrainPowerCamera);
                    Vector3 strainRot = new Vector3(0 - power / 3, 0, 0);
                    ball.transform.position = Vector3.Lerp(ball.transform.position, strainPos, delay);
                    mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, strainCameraPos, delay);
                    ball.transform.eulerAngles = strainRot;
                }
                if (power * 2 > maxPower)
                {
                    ball.Shake(ball.transform,power);
                }
                if (touch.phase == TouchPhase.Ended && power < maxPower / 4)
                {
                    ball.transform.position = defaultBallPos;
                    ball.straining = false;
                }
                if (touch.phase == TouchPhase.Ended && power > maxPower / 4)
                {
                    ball.Jump(power);
                    ball.straining = false;
                }
            }
        }
    }

    public void GameOver()
    {
        gameOver.SetActive(true);
        Invoke("ReloadScene", 1f);
    }
    public void Finish()
    {
        win.SetActive(true);
        PlayerPrefs.SetInt("level", level);
        Invoke("ReloadScene", 1f);
    }

    void ReloadScene()
    {
        SceneManager.LoadScene("Game");
    }

    Transform RandomBlock()
    {
        if (level == 1)
        {
            return blocks[0];
        }
        int rand = Random.Range(0, 100);
        Transform tempBlock = blocks[0];
        if (level < 4)
        {
            if (rand < 50)
            {
                tempBlock = blocks[0];
            }
            else if (rand < 100)
            {
                tempBlock = blocks[1];
            }
            return tempBlock;
        }
        if (level < 6)
        {
            if (rand < 40)
            {
                tempBlock = blocks[0];
            }
            else if (rand < 70)
            {
                tempBlock = blocks[1];
            }
            else if (rand < 100)
            {
                tempBlock = blocks[2];
            }
            return tempBlock;
        }
        else
        {
            float block0Rand = Mathf.Clamp((30 - level), 5, 30);
            float block1Rand = Mathf.Clamp((60 - level), 20, 60);
            float block2Rand = Mathf.Clamp((90 - level), 50, 90);
            if (rand < block0Rand)
            {
                tempBlock = blocks[0];
            }
            else if (rand < block1Rand)
            {
                tempBlock = blocks[1];
            }
            else if (rand < block2Rand)
            {
                tempBlock = blocks[2];
            }
            else if (rand < 100)
            {
                tempBlock = blocks[3];
            }
            return tempBlock;
        }
    }
}
