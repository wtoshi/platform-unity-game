using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager instance;

    [System.Serializable]
    public class Assigns
    {
        public Transform CubesHolder;
        public Camera MainCamera;
        public GameObject MovingCube;
    }
    public enum GameStates
    {
        Menu, Playing, GameOver, LevelPassed, EndGame
    }

    [Header("Assignable Objects")]
    public Assigns assigns = new Assigns();

    GameStates currentState = GameStates.Playing;

    float cubeLenght = 2f;
    [SerializeField] float m_cubeSpeed;
    [SerializeField] float playerLevel = 1;

    public float CubeLenght { get { return cubeLenght; } set { cubeLenght = value; onCubeLenghtChanged?.Invoke(); } }
    public float CubeSpeed { get { return m_cubeSpeed; } set { m_cubeSpeed = value; } }
    public float LevelToSize { get { return playerLevelToSize.Evaluate(playerLevel); } }

    [SerializeField] AnimationCurve playerLevelToSize;

    MovingCube lastCube;
    public bool canCreateCube = false;


    // Actions
    public delegate void CubeSize();
    public event CubeSize onCubeLenghtChanged;

    public delegate void GameState();
    public event GameState onGameStateStarted;

    public delegate void CubeState();
    public event CubeState onCubeMoved;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        if (currentState == GameStates.Playing)
        {
            GetChoice();
        }
    }

    public void StartGame()
    {
        StartCoroutine(doStartGame());
    }

    IEnumerator doStartGame()
    {
        // Leveli oluþtur
        LevelController.instance.CreateLevel((int)Mathf.Round(LevelToSize));

        currentState = GameStates.Playing;

        // Level dizayna göre Camera size ve position ayarý
        Vector3 camPos = LevelController.instance.assigns.MyCollider.center;
        assigns.MainCamera.GetComponent<CameraController>().UpdatePosition(camPos);

        // UI Game panel göster
        UIManager.instance.SetGamePanel();

        yield return new WaitForSeconds(2f);

        canCreateCube = true;
        onGameStateStarted?.Invoke();
    }

    public void RestartGame()
    {
        playerLevel = 1;
        
        StartGame();
    }

    public void NextLevel()
    {
        playerLevel++;
        assigns.MainCamera.GetComponent<CameraController>().ChangeBGColor();
        StartGame();

    }

    void GetChoice()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = assigns.MainCamera.ScreenPointToRay(Input.mousePosition);

            int layerMask = LayerMask.GetMask("SigmedTile");
            RaycastHit hit;
            if (Physics.Raycast(ray,out hit,100f, layerMask))
            {
                // Basýlan SignedTile ise cube oluþtur
                SignedTile signedTile = hit.collider.GetComponent<SignedTile>();
                CreateCube(signedTile);
            }
        }
    }

    public bool CreateCube(SignedTile _clickedTile, bool _prepare = false)
    {
        
        if (!canCreateCube && !_prepare)
            return false;

        if (!_clickedTile.CanSendCube)
            return false;

        lastCube = Instantiate(assigns.MovingCube, assigns.CubesHolder).GetComponent<MovingCube>();
        lastCube.Set(_clickedTile, _prepare);

        SoundManager.instance.PlaySound("snd_click");

        // Listen Action fm Cube
        if (true)
        {
            canCreateCube = false;
            lastCube.onCubeStopped += CheckGameState;
        }

        return true;
    }

    #region StateControls
    void CheckGameState()
    {
        onCubeMoved?.Invoke();
        // Remove Action fm LastCube
        lastCube.onCubeStopped -= CheckGameState;

        bool endGame = CheckForGameEnd();

        Debug.Log("oyun bitti mi: "+ endGame);
        if (endGame)
        {
            bool win = CheckForWin();

            if (win)
            {
                Debug.Log("Kazandý");
                canCreateCube = false;
                currentState = GameStates.LevelPassed;
                UIManager.instance.ShowPanel( UIManager.PanelTypes.Passed);
                SoundManager.instance.PlaySound("snd_congratulation");
                return;
            }
            else
            {
                Debug.Log("Game Over");
                canCreateCube = false;
                currentState = GameStates.GameOver;
                UIManager.instance.ShowPanel(UIManager.PanelTypes.Failed);
                SoundManager.instance.PlaySound("snd_fail");

                return;
            }
        }
        else
        {
            // Oyun bitmedi devam
            canCreateCube = true;
        }
    }

    bool CheckForGameEnd()
    {
        List<List<bool>> replyList = new List<List<bool>>();

        int[] checkLenghts = { 2,3,4};

        foreach (SignedTile tile in LevelController.instance.signedTileList)
        {
            replyList.Add(tile.CheckForEnd(checkLenghts));
        }

        for (int i = 0; i < replyList.Count; i++)
        {
            for (int j = 0; j < replyList[i].Count; j++)
            {
                // Eðer SingleTile'lar da hareket yeri var ise, oyun bitmedi henüz
                if (replyList[i][j] == true)
                {
                    return false;
                }
            }
        }

        // SingleTile 'da hiç hareket yeri yok ise hepsi false döndürürse, oyun bitti!
        return true;
    }

    bool CheckForWin()
    {
        List<bool> replyList = new List<bool>();

        foreach (BaseTile tile in LevelController.instance.baseTileList)
        {
            replyList.Add(tile.CheckSlot());
        }

        for (int i = 0; i < replyList.Count; i++)
        {
            // Eðer BaseTile boþ olan varsa Kazanamadý?
            if (replyList[i] == false)
            {
                return false;
            }
        }

        return true;
    }
    #endregion
}
