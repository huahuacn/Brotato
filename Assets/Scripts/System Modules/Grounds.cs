using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Grounds : Singleton<Grounds>
{
    public const string GROUND = "Ground";

    [SerializeField] Vector3 startPosition = Vector3.zero;
    private GameObject gridPre;
    private Vector3 size;
    private GameObject[,] grids;
    private PolygonCollider2D polygonCollider2D;
    Vector3 bottomLeft;
    Vector3 topRight;
    public float minX;
    public float minY;
    public float maxX;
    public float maxY;
    public Vector3 center;

    protected override void Awake()
    {
        base.Awake();

        polygonCollider2D = GetComponent<PolygonCollider2D>();
        if (polygonCollider2D == null) return;

        minX = polygonCollider2D.bounds.min.x;
        minY = polygonCollider2D.bounds.min.y;
        maxX = polygonCollider2D.bounds.max.x;
        maxY = polygonCollider2D.bounds.max.y;
        center = polygonCollider2D.bounds.center;
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        // player position to center
        player.transform.position =  center;
        player.transform.GetChild(0).gameObject.SetActive(true);
        
    }

    // void Create ()
    // {
    //     size = gridPre.GetComponentInChildren<MeshRenderer>().bounds.size;

    //     for (int r = 0; r < row; r++) {
    //         for (int c = 0; c < column ; c++) {
    //             var gameObject =  GameObject.Instantiate(gridPre);
    //             gameObject.transform.SetParent(this.transform);

    //             var x = startPosition.x + r *  size.x;
    //             var y = startPosition.y + c *  size.y;

    //             gameObject.transform.position = new Vector3(x,y,0);

    //             grids[r,c]  = gameObject;
    //         }

    //     }

    //     // this.gameObject.transform.rotation = Quaternion.AngleAxis(-90, Vector3.right);
    // }

    // void CreateBorder()
    // {
    //     Vector3 grids0 = grids[0,0].transform.position;
    //     Vector3 grids1 = grids[row-1,column-1].transform.position;

    //     bottomLeft = new Vector3(grids0.x - size.x/2, grids0.y - size.y/2);
    //     topRight = new Vector3(grids1.x + size.x/2, grids1.y + size.y/2);

    //     minX = bottomLeft.x;
    //     maxX = topRight.x;
    //     minY = bottomLeft.y;
    //     maxY = topRight.y;

    //     Vector2[] newPoint = new Vector2[polygonCollider2D.points.Length];

    //     newPoint[0] = new Vector2(minX,minY);
    //     newPoint[1] = new Vector2(maxX, minY);
    //     newPoint[2] = new Vector2(maxX,maxY);
    //     newPoint[3] = new Vector2(minX,maxY);

    //     polygonCollider2D.SetPath(0,newPoint);

    // }
}
