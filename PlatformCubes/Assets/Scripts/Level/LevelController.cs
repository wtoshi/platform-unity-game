using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static LevelController instance;

 
    [System.Serializable]
    public class Assigns
    {
        public BoxCollider MyCollider;
        public GameObject TilePrefab;
        public GameObject SignTilePrefab;
        public Transform LevelHolder;
        public Transform CubesHolder;
    }

    [System.Serializable]
    public class LevelBorders
    {
        public GameObject up;
        public GameObject down;
        public GameObject left;
        public GameObject right;
    }

    [Header("Level Borders")]
    public LevelBorders levelBorders = new LevelBorders();
    [Header("Assignable Objects")] 
    public Assigns assigns = new Assigns();

    [Space(30f)]

    int m_minLevel = 5;
    int m_maxLevel = 10;
    Vector2 m_Size;
    [HideInInspector]   public float m_TileSize = 1;   

    // Prepare için
    public List<SignedTile> signedTileList = new List<SignedTile>();
    public List<BaseTile> baseTileList = new List<BaseTile>();

    [HideInInspector] public int Size {
        set 
        {
            value = Mathf.Clamp(value, m_minLevel, m_maxLevel);
            m_Size = new Vector2(value,value); 
        }
    }

    public enum TileType
    {
        BaseTile, SignedTile
    }

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

    // Verilen size'a göre Level oluþturur
    public void CreateLevel(int _level)
    {
        ClearLevel();

        Size = _level;

        for (int i = 0; i < m_Size.x; i++)
        {
            for (int j = 0; j < m_Size.y+2; j++)
            {
                if ((j == 0))
                {
                    // Alt SignedTile oluþtur
                    SpawnTile(i, j, TileType.SignedTile).GetComponentInChildren<SignedTile>().Set(SignedTile.Direction.ToUp);
                }
                else if ((j == m_Size.y + 1))
                {
                    // Ust SignedTile oluþtur
                    SpawnTile(i, j, TileType.SignedTile).GetComponentInChildren<SignedTile>().Set(SignedTile.Direction.ToBottom);
                }
                else
                {
                    SpawnTile(i, j, TileType.BaseTile);

                    if (i==0)
                    {
                        // Sol taraftaki SignedTile oluþtur
                        SpawnTile(-m_TileSize, j, TileType.SignedTile).GetComponentInChildren<SignedTile>().Set(SignedTile.Direction.ToRight);
                    }
                    else if (i == m_Size.x-1)
                    {  
                        // Sað taraftaki SignedTile oluþtur
                        SpawnTile(i+ m_TileSize, j, TileType.SignedTile).GetComponentInChildren<SignedTile>().Set(SignedTile.Direction.ToLeft);
                    }
                }
                
                
            }
        }

        //TODO Set Level Holder collider 
        assigns.MyCollider.center = new Vector3(m_Size.x/2 - m_TileSize/2,0f, m_Size.y/2 + m_TileSize/2);
        assigns.MyCollider.size = new Vector3(m_Size.x,0,m_Size.y);

        CreateColliders();

        PrepareLevel();
    }

    void CreateColliders()
    {
        float offset = 0.50f;
        levelBorders.up.GetComponent<BoxCollider>().size = new Vector3(1f, m_Size.x, m_Size.y);
        levelBorders.down.GetComponent<BoxCollider>().size = new Vector3(1f, m_Size.x, m_Size.y);
        levelBorders.left.GetComponent<BoxCollider>().size = new Vector3(1f, m_Size.x, m_Size.y);
        levelBorders.right.GetComponent<BoxCollider>().size = new Vector3(1f, m_Size.x, m_Size.y);

        levelBorders.up.transform.localPosition = new Vector3(m_Size.x / 2 - m_TileSize / 2, 0f, m_Size.y + m_TileSize / 2 + offset);
        levelBorders.down.transform.localPosition = new Vector3(m_Size.x / 2 - m_TileSize / 2, 0f, m_TileSize / 2 - offset);
        levelBorders.left.transform.localPosition = new Vector3(-m_TileSize/2 - offset, 0f, m_Size.y/2 + m_TileSize / 2);
        levelBorders.right.transform.localPosition = new Vector3(m_Size.x - m_TileSize/2 + offset, 0f, m_Size.y/2 + m_TileSize / 2);

        levelBorders.up.transform.localRotation = Quaternion.Euler(0,90f,0);
        levelBorders.down.transform.localRotation = Quaternion.Euler(0, 90f, 0);
        levelBorders.left.transform.localRotation = Quaternion.Euler(0, 0, 0);
        levelBorders.right.transform.localRotation = Quaternion.Euler(0, 0, 0);

    }

    GameObject SpawnTile(float _i, float _j, TileType _tileType)
    {
        GameObject tile = null;
        bool addedToSingedList = false;
        bool addedToBaseList = false;


        switch (_tileType)
        {
            case TileType.BaseTile:
                tile = assigns.TilePrefab;
                addedToBaseList = true;
                break;
            case TileType.SignedTile:
                tile = assigns.SignTilePrefab;
                addedToSingedList = true;
                break;
        }

        if (tile)
        {
            Vector3 addPos = new Vector3(_i, 0, _j) * m_TileSize;
            GameObject go = Instantiate(tile, tile.transform.position + addPos, Quaternion.identity, assigns.LevelHolder);
            if (addedToSingedList)
            {
                signedTileList.Add(go.GetComponentInChildren<SignedTile>());
            }
            else if (addedToBaseList)
            {
                baseTileList.Add(go.GetComponent<BaseTile>());
            }
            return go;
        }
        else
        {
            Debug.LogError("Tile is NULL!");
            return null;
        }

    }

    void ClearLevel()
    {
        signedTileList.Clear();
        baseTileList.Clear();

        // Level Objectleri temizle
        for (int i = 0; i < assigns.LevelHolder.childCount; i++)
        {
            Destroy(assigns.LevelHolder.GetChild(i).gameObject);
        }

        // Atýlan Cube'leri temizle
        for (int i = 0; i < assigns.CubesHolder.childCount; i++)
        {
            Destroy(assigns.CubesHolder.GetChild(i).gameObject);
        }
    }

    // Level üzerine ilk primitiveleri oluþturur
    void PrepareLevel()             
    {
        StartCoroutine(doPrepare());
    }

    IEnumerator doPrepare()
    {
        int max = 2;
        int step = 0;

        while (step < max)
        {
            SignedTile signedTitle = signedTileList[Random.Range(0, signedTileList.Count)];
            if(GameManager.instance.CreateCube(signedTitle,true))
                step++;
            yield return new WaitForSeconds(.5f);
        }
    }




}
