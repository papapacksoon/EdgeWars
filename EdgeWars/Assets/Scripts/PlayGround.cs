using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;




public class PlayGround : MonoBehaviour
{
    public List<Sprite> mySprites = new List<Sprite>();             //sprites for playground items
    public GameObject playGroundItem;                               //prefab for palyground item
    public GameObject playGroundItem_ar_2;                               //prefab for palyground item
    public GameObject playGroundItem_ar_3;                               //prefab for palyground item
    public Text ScoreText;                                          //Score text Player1
    public Text ScoreText2;                                         //Score text Player1
    public Text TurnText;                                           //Score text For turn and timer;
    public Text EnergyText;
    public GameObject board;

    public Button buttonMainMenu;
    public Button buttonRestart;

    const int PLAYERTURNTIME = 30;
    const float ENEMYTURNTIME = 2.0f;



    public static PlayGround instance;

    private enum AspectRatios {ar_1_7, ar_2, ar_2_05};
    private float localScaleFactor = 0;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (instance == null)
        {
            instance = this;
        }

        Debug.Log(" Playground = " + instance);
    }

    public enum FieldOwner { PlayerOne, PlayerTwo, None };

    private FieldOwner turn = FieldOwner.PlayerOne;
    private static int _width = 7;
    private static int _height = 11;
    private float timer = 30.0f;
    private int displayedPlayerTimer = PLAYERTURNTIME;
    private float displayedEnemyTimer = ENEMYTURNTIME;


    public int enemyRank = 1000;

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
        //    UnityEditor.GameObjectUtility.SetParentAndAlign(this.fieldItem, PlayGround.instance.board);
            this.x = x;
            this.y = y;
            this.owner = owner;
            
        }
    }

    private int playerOneScore = 1;
    private int playerTwoScore = 1;

    public List<PlayGroundFieldItem> _playGroundItems = new List<PlayGroundFieldItem>();
    
    public void InitializePlayGround()
    {
        turn = FieldOwner.PlayerOne;
        timer = 30.0f;

        Debug.Log(" width = " + Screen.width + " height = " + Screen.height);
        float aspectRatio = Screen.height / (float)Screen.width;
        AspectRatios currentAspectRatio;

        if (aspectRatio < 1.9166666667f)
        {
            localScaleFactor = 0.7f;
            currentAspectRatio = AspectRatios.ar_1_7;
        }
        else if (aspectRatio < 2.0276f)
        {
            localScaleFactor = 0.65f;
            currentAspectRatio = AspectRatios.ar_2;
        }
        else
        {
            localScaleFactor = 0.6f;
            currentAspectRatio = AspectRatios.ar_2_05;
            _height += 2;
        }

        Debug.Log(currentAspectRatio);
        Debug.Log(aspectRatio);

        if (myColors.Count > 0) myColors.Clear();
        if (_playGroundItems.Count > 0)
        {
            foreach (var item in _playGroundItems)
            {
                Destroy(item.fieldItem);
            }

            _playGroundItems.Clear();
        }

        //initializing colors

        myColors.Add(Color.yellow);
        myColors.Add(Color.red);
        myColors.Add(Color.cyan);
        myColors.Add(Color.blue);
        myColors.Add(Color.green);
        myColors.Add(Color.magenta);


        // AspectRatio 

        //initializing Playground
        for (int x = 0; x < _width; x++)
        {

            int myHeight;
            float myY;
            float myX;
            int rowNumber;
            float zRotation = -0.026182f;

            if (x % 2 == 0)
            {
                myY = 0;
                myX = 0;
                rowNumber = 0;
                myHeight = _height / 2 + 1;

                switch (currentAspectRatio)
                {
                    case AspectRatios.ar_1_7:
                        myY = -2.53f;
                        myX = -1.195f + 0.8f * (x / 2);
                        break;

                    case AspectRatios.ar_2:
                        myY = -2.3f;
                        myX = -1.14f + 0.75f * (x / 2);
                        break;

                    case AspectRatios.ar_2_05:
                        myY = -2.5f;
                        myX = -1.065f + 0.7f * (x / 2);
                        break;
                }
       
            }
            else
            {
                
                myY = 0;
                myX = 0;
                rowNumber = 1;
                myHeight = _height / 2;

                switch (currentAspectRatio)
                {
                    case AspectRatios.ar_1_7:
                        myY = -2.12f;
                        myX = -0.785f + 0.8f * (x / 2);
                        break;

                    case AspectRatios.ar_2:
                        myY = -1.93f;
                        myX = -0.76f + 0.75f * (x / 2);
                        break;

                    case AspectRatios.ar_2_05:
                        myY = -2.15f;
                        myX = -0.715f + 0.7f * (x / 2);
                        break;
                }
            }

            for (int y = 0; y < myHeight; y++)
            {
                if (currentAspectRatio == AspectRatios.ar_1_7)
                {
                    _playGroundItems.Add(new PlayGroundFieldItem(Instantiate(playGroundItem, new Vector3(myX, myY + 0.83f * y), new Quaternion(0, 0, zRotation, 1)), x, rowNumber));
                }
                else if (currentAspectRatio == AspectRatios.ar_2)
                {
                    _playGroundItems.Add(new PlayGroundFieldItem(Instantiate(playGroundItem_ar_2, new Vector3(myX, myY + 0.75f * y), new Quaternion(0, 0, zRotation, 1)), x, rowNumber));
                }
                else
                {
                    _playGroundItems.Add(new PlayGroundFieldItem(Instantiate(playGroundItem_ar_3, new Vector3(myX, myY + 0.7f * y), new Quaternion(0, 0, zRotation, 1)), x, rowNumber));
                }
                    
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
        if (GameManager.instance.singlePlayerWithoutLogginIn) EnergyText.text = "";
        else EnergyText.text = "Energy " + EnergyScript.currentEnergy + "/10";

        ScoreText.text = "   You : 1";
        ScoreText2.text = "Enemy : 1   ";
        GameManager.instance.isGameOver = false;
    }

    // Start is called before the first frame update
    void Start()
    {
                
    }

    // Update is called once per frame 
    void Update()
    {
        
        if (GameManager.instance.isGameOver == false)
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
                        CountPlayerRank(true);
                    }
                }
                else
                {
                    TurnText.text = "Game over! Enemy wins";
                    TurnText.color = Color.red;
                    CountPlayerRank(false);
                }

                GameManager.instance.isGameOver = true;
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
    /*public void BordersRecolor()
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
    }*/

    IEnumerator ChangeSpriteAnimationToZero(GameObject obj)
    {
        for (float i = localScaleFactor; i>=0; i -= 0.05f)
        {
            obj.transform.localScale = new Vector3(localScaleFactor, i, 0.5f);
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator ChangeSpriteAnimationGetBig(GameObject obj)
    {
        for (float i = 0; i <= localScaleFactor+0.001f; i += 0.05f)
        {
            obj.transform.localScale = new Vector3(localScaleFactor, i, 0.5f);
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

    public void CountPlayerRank(bool playerWin = false)
    {
        if (GameManager.instance.singlePlayerWithoutLogginIn)
        {
            Debug.Log("Player doesn`t log in => rank is not calculated");
            return;
        }
        
        int randomElement = UnityEngine.Random.Range(0, 10); //rank between 920 and 1080
        int gameEnd = 0; //lose

        if (playerWin) gameEnd = 1; //win

        if (randomElement < 5) enemyRank -= randomElement * 20;
        else enemyRank += (randomElement - 5) * 20;

        Debug.Log("Player rank = " + PlayerManager.instance.playerRank);
        Debug.Log("Enemy rank = " + enemyRank);

        double expectedPlayerPoints = 1f / (1f + Math.Pow(10f, (enemyRank - PlayerManager.instance.playerRank) / 400f));

        PlayerManager.instance.playerRank += (int)(16 * (gameEnd - expectedPlayerPoints));
        Debug.Log("Claculated Player rank = " + PlayerManager.instance.playerRank);

        GameManager.instance.UpdateLeaderboard();
        GameManager.instance.UpdatePlayerRank();
    }

    public void ClearPlayground()
    {
        if (_playGroundItems.Count > 0)
        {
            foreach (var item in _playGroundItems)
            {
                Destroy(item.fieldItem);
            }
        }
    }
}


