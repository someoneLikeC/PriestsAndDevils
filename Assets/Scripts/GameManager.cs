using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] gameObjects = new GameObject[9];
    public int leftBankPriestCount = 3;
    public int rightBankPriestCount = 0;
    public int leftBankDevilCount = 3;
    public int rightBankDevilCount = 0;
    public int peopleOnBoat = 0;
    public bool moving = false;
    public bool isRight = false;

    private bool win = false;
    private bool firstSeatFull = false;
    private bool lastSeatFull = false;
    private int priestCount = 0;
    private int devilCount = 0;
    private GameObject river;
    private GameObject[] bank = new GameObject[2];
    private GameObject[] priest = new GameObject[3];
    private GameObject[] devil = new GameObject[3];
    private GameObject boat;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Update()
    {
        if (moving)
        {
            if (!isRight)
            {
                boat.transform.position += new Vector3(0, 0, 1 * Time.deltaTime);
            }
            else
            {
                boat.transform.position += new Vector3(0, 0, -1 * Time.deltaTime);
            }
        }
        if (boat.transform.position.z < -2.25 || boat.transform.position.z > 2.25)
        {
            moving = false;

            if (isRight)
            {
                boat.transform.position = new Vector3(0, 0.5f, -2.25f);
            }
            else
            {
                boat.transform.position = new Vector3(0, 0.5f, 2.25f);
            }
            isRight = !isRight;
        }
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(30, 25, 80, 220), "");
        if (GUI.Button(new Rect(35, 30, 70, 30), "重启")) Init();
        GUI.Box(new Rect(35, 65, 30, 140), "P");
        GUI.Box(new Rect(75, 65, 30, 140), "D");
        if (!gameOver())
        {
            if (GUI.Button(new Rect(35, 85, 30, 30), "1")) movePeople(priest[0]);
            if (GUI.Button(new Rect(35, 125, 30, 30), "2")) movePeople(priest[1]);
            if (GUI.Button(new Rect(35, 165, 30, 30), "3")) movePeople(priest[2]);
            if (GUI.Button(new Rect(75, 85, 30, 30), "1")) movePeople(devil[0]);
            if (GUI.Button(new Rect(75, 125, 30, 30), "2")) movePeople(devil[1]);
            if (GUI.Button(new Rect(75, 165, 30, 30), "3")) movePeople(devil[2]);
            if (GUI.Button(new Rect(35, 210, 70, 30), "开船")) boatMov();
        }
        else
        {
            if (win)
            {
                GUI.Box(new Rect(320, 150, 50, 50), "你赢了");
            }
            else
            {
                GUI.Box(new Rect(320, 150, 50, 50), "你输了");
            }
        }
    }


    void Init()
    {
        // 初始化变量
        leftBankPriestCount = 3;
        rightBankPriestCount = 0;
        leftBankDevilCount = 3;
        rightBankDevilCount = 0;
        peopleOnBoat = 0;
        moving = false;
        isRight = false;
        priestCount = 0;
        devilCount = 0;
        firstSeatFull = false;
        lastSeatFull = false;
        win = false;
        // 重启时消除原先生成的游戏对象
        GameObject[] obj = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach (GameObject child in obj)
        {
            if (child.name == "River(Clone)" || child.name == "Bank(Clone)" || child.name == "Boat(Clone)" 
                || child.name == "Priest_1(Clone)" || child.name == "Devil_1(Clone)" 
                || child.name == "Priest_2(Clone)" || child.name == "Devil_2(Clone)" 
                || child.name == "Priest_3(Clone)" || child.name == "Devil_3(Clone)")
            {
                Destroy(child);
            }
        }
        // 生成游戏对象
        river = Instantiate(gameObjects[1], Vector3.zero, transform.rotation);
        boat = Instantiate(gameObjects[2], new Vector3(0, 0.5f, -2.25f), transform.rotation);
        bank = new GameObject[2];
        priest = new GameObject[3];
        devil = new GameObject[3];
        for (int i = 0; i < 2; i++)
        {
            bank[i] = Instantiate(gameObjects[0], new Vector3(0, 0, 3.0f * (i - 0.5f) * 2), transform.rotation);
        }
        for (int i = 0; i < 3; i++)
        {
            devil[i] = Instantiate(gameObjects[3 + i], new Vector3(2 + i, 0.5f, -3.5f), transform.rotation);
            devil[i].GetComponent<settings>().onBoat = false;
            devil[i].GetComponent<settings>().isRight = false;
        }
        for (int i = 0; i < 3; i++)
        {
            priest[i] = Instantiate(gameObjects[6 + i], new Vector3(-4 + i, 0.5f, -3.5f), transform.rotation);
            priest[i].GetComponent<settings>().onBoat = false;
            priest[i].GetComponent<settings>().isRight = false;
        }
    }

    void movePeople(GameObject gameObject)
    {
        if (!moving)
        {
            if (!gameObject.GetComponent<settings>().onBoat)
            {
                if (gameObject.GetComponent<settings>().isRight == isRight)
                {
                    if (peopleOnBoat == 0)
                    {
                        gameObject.transform.position = boat.transform.position + new Vector3(0, 0, 0.3f);
                        gameObject.transform.parent = boat.transform;
                        peopleOnBoat++;
                        firstSeatFull = true;
                        gameObject.GetComponent<settings>().onBoat = true;
                        if (gameObject.name == "Priest_1(Clone)" || gameObject.name == "Priest_2(Clone)" 
                            || gameObject.name == "Priest_3(Clone)")
                        {
                            priestCount++;
                        }
                        else
                        {
                            devilCount++;
                        }
                    }
                    else if (peopleOnBoat == 1)
                    {
                        if (firstSeatFull)
                        {
                            gameObject.transform.position = boat.transform.position + new Vector3(0, 0, -0.3f);
                            lastSeatFull = true;
                        }
                        else if (lastSeatFull)
                        {
                            gameObject.transform.position = boat.transform.position + new Vector3(0, 0, 0.3f);
                            firstSeatFull = true;
                        }
                        gameObject.transform.parent = boat.transform;
                        peopleOnBoat++;
                        gameObject.GetComponent<settings>().onBoat = true;
                        if (gameObject.name == "Priest_1(Clone)" || gameObject.name == "Priest_2(Clone)" 
                            || gameObject.name == "Priest_3(Clone)")
                        {
                            priestCount++;
                        }
                        else
                        {
                            devilCount++;
                        }
                    }
                    else return;
                }
            }
            else
            {
                if(gameObject.transform.position == boat.transform.position + new Vector3(0, 0, 0.3f))
                {
                    firstSeatFull = false;
                }
                else
                {
                    lastSeatFull = false;
                }
                if (isRight)
                {
                    if(gameObject.name == "Priest_1(Clone)")
                    {
                        priestCount--;
                        gameObject.transform.position = new Vector3(-4, 0.5f, 3.5f);
                        gameObject.GetComponent<settings>().onBoat = false;
                    }
                    else if(gameObject.name == "Priest_2(Clone)")
                    {
                        priestCount--;
                        gameObject.transform.position = new Vector3(-3, 0.5f, 3.5f);
                        gameObject.GetComponent<settings>().onBoat = false;
                    }
                    else if(gameObject.name == "Priest_3(Clone)")
                    {
                        priestCount--;
                        gameObject.transform.position = new Vector3(-2, 0.5f, 3.5f);
                        gameObject.GetComponent<settings>().onBoat = false;
                    }
                    else if(gameObject.name == "Devil_1(Clone)")
                    {
                        devilCount--;
                        gameObject.transform.position = new Vector3(2, 0.5f, 3.5f);
                        gameObject.GetComponent<settings>().onBoat = false;
                    }
                    else if(gameObject.name == "Devil_2(Clone)")
                    {
                        devilCount--;
                        gameObject.transform.position = new Vector3(3, 0.5f, 3.5f);
                        gameObject.GetComponent<settings>().onBoat = false;
                    }
                    else
                    {
                        devilCount--;
                        gameObject.transform.position = new Vector3(4, 0.5f, 3.5f);
                        gameObject.GetComponent<settings>().onBoat = false;
                    }
                }
                else
                {
                    if (gameObject.name == "Priest_1(Clone)")
                    {
                        priestCount--;
                        gameObject.transform.position = new Vector3(-4, 0.5f, -3.5f);
                        gameObject.GetComponent<settings>().onBoat = false;
                    }
                    else if (gameObject.name == "Priest_2(Clone)")
                    {
                        priestCount--;
                        gameObject.transform.position = new Vector3(-3, 0.5f, -3.5f);
                        gameObject.GetComponent<settings>().onBoat = false;
                    }
                    else if (gameObject.name == "Priest_3(Clone)")
                    {
                        priestCount--;
                        gameObject.transform.position = new Vector3(-2, 0.5f, -3.5f);
                        gameObject.GetComponent<settings>().onBoat = false;
                    }
                    else if (gameObject.name == "Devil_1(Clone)")
                    {
                        devilCount--;
                        gameObject.transform.position = new Vector3(2, 0.5f, -3.5f);
                        gameObject.GetComponent<settings>().onBoat = false;
                    }
                    else if (gameObject.name == "Devil_2(Clone)")
                    {
                        devilCount--;
                        gameObject.transform.position = new Vector3(3, 0.5f, -3.5f);
                        gameObject.GetComponent<settings>().onBoat = false;
                    }
                    else
                    {
                        devilCount--;
                        gameObject.transform.position = new Vector3(4, 0.5f, -3.5f);
                        gameObject.GetComponent<settings>().onBoat = false;
                    }
                }
                gameObject.GetComponent<settings>().isRight = isRight;
                peopleOnBoat--;
                gameObject.transform.parent = null;
            }
        }
    }

    void boatMov()
    {
        if (priestCount >= 1)
        {
            moving = true;
            if (!isRight)
            {
                leftBankDevilCount = 3 - devilCount - rightBankDevilCount;
                leftBankPriestCount = 3 - priestCount - rightBankPriestCount;
            }
            else
            {
                rightBankDevilCount = 3 - devilCount - leftBankDevilCount;
                rightBankPriestCount = 3 - priestCount - leftBankPriestCount;
            }
        }
        else
        {
            return;
        }
    }

    bool gameOver()
    {
        if ((leftBankDevilCount > leftBankPriestCount && leftBankPriestCount != 0) 
            || (rightBankDevilCount > rightBankPriestCount && rightBankPriestCount != 0))
        {
            win = false;
            return true;
        }
        else if (rightBankPriestCount + priestCount == 3 && rightBankDevilCount + devilCount == 3)
        {
            win = true;
            return true;
        }
        else return false;
    }
}
