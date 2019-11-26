using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;




public class PlayGround : MonoBehaviour
{
    public List<Sprite> mySprites = new List<Sprite>();             //sprites for playground items
    public GameObject playGroundItem;                               //prefab for palyground item
    public Text ScoreText;                                          //Score text Player1
    public Text ScoreText2;                                         //Score text Player1
    public Text TurnText;                                           //Score text For turn and timer;
    public Text EnergyText;                                         

    public Button buttonMainMenu;
    public Button buttonRestart;

    const int PLAYERTURNTIME = 30;
    const float ENEMYTURNTIME = 2.0f;

    public static PlayGround instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public enum FieldOwner { PlayerOne, PlayerTwo, None };

    private FieldOwner turn = FieldOwner.PlayerOne;
    private static int _width = 7;
    private static int _height = 11;
    private float timer = 30.0f;
    private int displayedPlayerTimer = PLAYERTURNTIME;
    private float displayedEnemyTimer = ENEMYTURNTIME;

    private bool isGameOver = true;

    private List<Color> myColors = new List<Color>(); //

    public class PlayGroundFieldItem
    {
        public GameObject fieldItem { get; set; }
        public int x;
        public int y;
        public FieldOwner owner;
        public Color color = Color.white;

        public PlayGroundFieldItem(GameObject fieldItem, int x, int y, FieldOwner owner = FieldOwner.None)
        {
            this.fieldItem = fieldItem;
            this.x = x;
            this.y = y;
            this.owner = owner;
        }
    }

    private int playerOneScore = 1;
    private int playerTwoScore = 1;

    public List<PlayGroundFieldItem> _playGroundItems = new List<PlayGroundFieldItem>();
    
    public void InitailizePlayGround()
    {
        turn = FieldOwner.PlayerOne;
        timer = 30.0f;
        isGameOver = false;

    //initializing colors
        myColors.Add(Color.yellow);
        myColors.Add(Color.red);
        myColors.Add(Color.cyan);
        myColors.Add(Color.blue);
        myColors.Add(Color.green);
        myColors.Add(Color.magenta);



        //initializing Playground
        for (int x = 0; x < _width; x++)
        {
            int myHeight;
            float myY;
            float myX;
            int rowNumber;

            if (x % 2 == 0)
            {
                myHeight = _height / 2 + 1;
                myY = -1.19f;
                myX = -0.04f + 0.89f * (x / 2);
                rowNumber = 0;
            }
            else
            {
                myHeight = _height / 2;
                myY = -0.75f;
                myX = 0.4f + 0.89f * ((x - 1) / 2);
                rowNumber = 1;
            }

            for (int y = 0; y < myHeight; y++)
            {

                _playGroundItems.Add(new PlayGroundFieldItem(Instantiate(playGroundItem, new Vector3(myX, myY + 0.89f * y), Quaternion.identity), x, rowNumber));

                rowNumber += 2;
            }

        }

        foreach (var item in _playGroundItems)
        {

            int rand = UnityEngine.Random.Range(0, 6);
            item.color = myColors[rand];
            item.fieldItem.GetComponent<SpriteRenderer>().sprite = mySprites[rand];


            if (item.x == 0 && item.y == 0)
            {
                item.owner = FieldOwner.PlayerOne;
                item.color = Color.white;
                item.fieldItem.GetComponent<SpriteRenderer>().sprite = mySprites[6];
            }
            if (item.x == _width - 1 && item.y == _height - 1)
            {
                item.owner = FieldOwner.PlayerTwo;
                item.color = Color.black;
                item.fieldItem.GetComponent<SpriteRenderer>().sprite = mySprites[7];
            }

        }

        TurnText.color = Color.white;
        TurnText.text = "It's your turn ! 30 seconds left";
        EnergyText.text = "Energy " + EnergyScript.currentEnergy + "/10";
    }

    // Start is called before the first frame update
    void Start()
    {
                
    }

    // Update is called once per frame 
    void Update()
    {
        
        if (isGameOver == false)
        {

            timer -= Time.deltaTime;

            if (timer < 0)
            {
                displayedEnemyTimer = ENEMYTURNTIME;
                displayedPlayerTimer = PLAYERTURNTIME;

                if (turn == FieldOwner.PlayerOne)
                {
                    var res = _playGroundItems.Where(p => p.owner == FieldOwner.None && isColliderNearPlayerGround(p, FieldOwner.PlayerTwo));

                    if (res.Count() == 0)
                    {
                        //autoturn needed
                        timer = 30;

                    }
                    else
                    {
                        turn = FieldOwner.PlayerTwo;
                        timer = 2;
                        StartCoroutine(BlockPlayerInput());
                        StartCoroutine(AIturn());
                    }
                    
                }
                else
                    timer = 30;


            }

            else
            {
                // display timer here;

                if (turn == FieldOwner.PlayerOne)
                {
                    //display only seconds 
                    if ((int)timer < displayedPlayerTimer)
                    {
                        displayedPlayerTimer = (int)timer;
                        TurnText.color = Color.white;
                        TurnText.text = "It's your turn ! " + displayedPlayerTimer + " seconds left";
                    }

                }
                else
                {
                    if (Math.Round(timer, 1) < displayedEnemyTimer)
                    {
                        displayedEnemyTimer = (float)Math.Round(timer, 1);
                        TurnText.color = Color.red;
                        TurnText.text = "It's enemy turn ! " + displayedEnemyTimer + " seconds left";
                    }

                }

            }


            var result = _playGroundItems.Where(p => p.owner == FieldOwner.None && isColliderNearPlayerGround(p, FieldOwner.PlayerOne));
            var result2 = _playGroundItems.Where(p => p.owner == FieldOwner.None && isColliderNearPlayerGround(p, FieldOwner.PlayerTwo));

            if (result.Count() == 0 && result2.Count() == 0) //game over there are no moves for any player
            {
                if (playerOneScore >= playerTwoScore)
                {
                    if (playerOneScore == playerTwoScore)
                    {
                        TurnText.text = "Game over! it's a draw";
                        TurnText.color = Color.white;
                    }
                    else
                    {
                        TurnText.text = "Game over! you win";
                        TurnText.color = Color.green;
                    }
                }
                else
                {
                    TurnText.text = "Game over! Enemy wins";
                    TurnText.color = Color.red;
                }
                
                isGameOver = true;
                return;
            }

            if (result.Count() == 0 && turn == FieldOwner.PlayerOne) //if player one has no moves - enemy takes a turn;
            {
                    turn = FieldOwner.PlayerTwo;
                    displayedEnemyTimer = ENEMYTURNTIME;
                    displayedPlayerTimer = PLAYERTURNTIME;
                    timer = 2;
                    StartCoroutine(BlockPlayerInput());
                    StartCoroutine(AIturn());
            }
            
        }
        
    }

    
    public void onItemMouseClick(GameObject collider)
    {
        if (turn != FieldOwner.PlayerOne) return;

        PlayGroundFieldItem result;
        try
        {
            result = _playGroundItems.Single(p => p.fieldItem == collider);
        }
        catch (System.InvalidOperationException)
        {
            Debug.Log("Collider not found!");
            return;
        }
        //need to know is collider belong to None and near to player fields
        if (result.owner != FieldOwner.None)
        {
            return;
        }

        if (isColliderNearPlayerGround(result, FieldOwner.PlayerOne))
        {
            int colorCount = 0;
            //recolor field abd change owner
            Color pickedColor = result.color;

            for (int i = 0; i < myColors.Count(); i++)
            {
                if (myColors[i] == pickedColor) colorCount = i;
            }

            result.owner = FieldOwner.PlayerOne;

            StartCoroutine(ChangeSprite(result.fieldItem, mySprites[6]));
            PlayerOneHit();

            //recolor fields
            var res = _playGroundItems.Where(p => p.owner == FieldOwner.PlayerOne);

            foreach (var item in res)
            {
                item.color = Color.white;
                item.fieldItem.GetComponent<SpriteRenderer>().sprite = mySprites[6];
            }

            //check all near fields for same color 

            do
            {

                res = _playGroundItems.Where(p => p.owner == FieldOwner.None && p.color == pickedColor && isColliderNearPlayerGround(p, FieldOwner.PlayerOne));


                foreach (var item in res)
                {

                    {
                        item.owner = FieldOwner.PlayerOne;
                        item.color = Color.white;
                        StartCoroutine(ChangeSprite(item.fieldItem, mySprites[6]));
                        PlayerOneHit();
                    }
                }

            }

            while (res.Count() != 0);

            //start AI turn
            var enemyturns = _playGroundItems.Where(p => p.owner == FieldOwner.None && isColliderNearPlayerGround(p, FieldOwner.PlayerTwo));

            displayedEnemyTimer = ENEMYTURNTIME;
            displayedPlayerTimer = PLAYERTURNTIME;

            if (enemyturns.Count() > 0)
            {
                turn = FieldOwner.PlayerTwo;
                
                timer = 2;
                StartCoroutine(BlockPlayerInput());
                StartCoroutine(AIturn());
            }
            else
            {
                //autoturn needed
                timer = 30;
            }
        }

        else return;
    }

    public bool isColliderNearPlayerGround(PlayGroundFieldItem fieldItem, FieldOwner player)
    {
        var result = _playGroundItems.Any(p => Math.Abs(p.x - fieldItem.x) == 1 && Math.Abs(p.y - fieldItem.y) == 1 && p.owner == player);
        return result;
    }

    public void PlayerOneHit()
    {
        playerOneScore += 1;
        ScoreText.text = "   You : " + playerOneScore;
    }

    public void PlayerTwoHit()
    {
        playerTwoScore += 1;
        ScoreText2.text = "Enemy : " + playerTwoScore + "   ";
    }

    public IEnumerator AIturn()
    {
        yield return new WaitForSeconds(1.4f);
       
        Color pickedColor;
        var result = _playGroundItems.Where(p => p.owner == FieldOwner.None && isColliderNearPlayerGround(p, FieldOwner.PlayerTwo)).OrderBy(p => p.color.ToString());
       
        if (result.Count() == 0) yield break;
        else
        {
            //choosing next move = pick the most color near;
            int max = 0;
            int current = 0;
            Color currentColor = result.ElementAt(0).color;
            pickedColor = currentColor;
            var res = result.ElementAt(0);
            var previtem = result.ElementAt(0);


            foreach (var item in result)
            {


                if (currentColor == item.color) current++;
                else
                {
                    if (current >= max)
                    {
                        max = current;
                        pickedColor = currentColor;
                        res = previtem;
                    }

                    currentColor = item.color;
                    current = 1;
                    previtem = item;
                }

            }

            if (current >= max)
            {
                pickedColor = currentColor;
                res = previtem;
            }


            res.owner = FieldOwner.PlayerTwo;
            StartCoroutine(ChangeSprite(res.fieldItem, mySprites[7]));

            PlayerTwoHit();

            var res2 = _playGroundItems.Where(p => p.owner == FieldOwner.PlayerTwo);
            foreach (var item in res2)
            {
                item.color = Color.black;
                item.fieldItem.GetComponent<SpriteRenderer>().sprite = mySprites[7];
            }

        }

        var result2 = _playGroundItems.Where(p => p.owner == FieldOwner.None && p.color == pickedColor && isColliderNearPlayerGround(p, FieldOwner.PlayerTwo));

        do
        {

            result2 = _playGroundItems.Where(p => p.owner == FieldOwner.None && p.color == pickedColor && isColliderNearPlayerGround(p, FieldOwner.PlayerTwo));
            
            foreach (var item in result2)
            {
                item.owner = FieldOwner.PlayerTwo;
                item.color = Color.black;
                StartCoroutine(ChangeSprite(item.fieldItem,mySprites[7]));
                PlayerTwoHit();
            }

        }

        while (result2.Count() != 0);

    }

    IEnumerator BlockPlayerInput()
    {
        yield return new WaitForSeconds(2f);
        turn = FieldOwner.PlayerOne;
        timer = 30;
    }

    //borders recoloring
    public void BordersRecolor()
    {
        Component[] borderSprites;
        var result = _playGroundItems.Where(p => p.owner == FieldOwner.PlayerOne).OrderBy(p => p.x).ThenByDescending(p => p.y);
        
        foreach (var item in result)
        {
            borderSprites = item.fieldItem.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 1; i < borderSprites.Count(); i++)
            {
                borderSprites[i].GetComponent<SpriteRenderer>().color = Color.green;
                borderSprites[i].GetComponent<SpriteRenderer>().sortingOrder = 5;
            }

            
        }

        result = _playGroundItems.Where(p => p.owner == FieldOwner.PlayerTwo).OrderBy(p => p.x).ThenByDescending(p => p.y);

        foreach (var item in result)
        {
            borderSprites = item.fieldItem.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 1; i < borderSprites.Count(); i++)
            {
                borderSprites[i].GetComponent<SpriteRenderer>().color = Color.red;
                borderSprites[i].GetComponent<SpriteRenderer>().sortingOrder = 4;
            }
            
        }
    }

    IEnumerator ChangeSpriteAnimationToZero(GameObject obj)
    {
        for (float i = 0.75f; i>=0; i -= 0.05f)
        {
            obj.transform.localScale = new Vector3(0.75f, i, 0.5f);
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator ChangeSpriteAnimationGetBig(GameObject obj)
    {
        for (float i = 0; i <= 0.75f; i += 0.05f)
        {
            obj.transform.localScale = new Vector3(0.75f, i, 0.5f);
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator ChangeSprite(GameObject obj, Sprite sprite)
    {
        StartCoroutine(ChangeSpriteAnimationToZero(obj));

        yield return new WaitForSeconds(0.2f);

        obj.GetComponent<SpriteRenderer>().sprite = sprite;

        StartCoroutine(ChangeSpriteAnimationGetBig(obj));
    }






}


