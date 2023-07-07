using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Grounds : Singleton<Grounds>
{
    public const string GROUND = "Ground";

    public Vector3 Center => grids[row/2,column/2].transform.position;
    public  Vector3 MinPosition => transform.TransformPoint(grids[0,0].transform.position);
    public Vector3 MaxPosition => transform.TransformPoint(grids[row-1,column-1].transform.position);
    [SerializeField] int row = 10;
    [SerializeField] int column = 10;
    [SerializeField] Vector3 startPosition = Vector3.zero;
    private GameObject gridPre;
    private GameObject[,] grids;

    [System.Obsolete]
    void Start()
    {
        grids = new GameObject[row, column];

        Addressables.LoadAsset<GameObject>(GROUND).Completed += (obj) => {
            gridPre = obj.Result;
            Create();


            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null ) return;

            // player position to center
            Vector3 center = Center;
            center.y += 10;
            center.z -= 1;
            player.transform.position =  center;
            player.transform.GetChild(0).gameObject.SetActive(true);
        };
        
    }

    void Create ()
    {
        Vector3 size = gridPre.GetComponentInChildren<MeshRenderer>().bounds.size;

        for (int r = 0; r < row; r++) {
            for (int c = 0; c < column ; c++) {
                var gameObject =  GameObject.Instantiate(gridPre);
                gameObject.transform.SetParent(this.transform);

                var x = startPosition.x + r *  size.x;
                var y = startPosition.y + c *  size.y;

                gameObject.transform.position = new Vector3(x,y,0);

                grids[r,c]  = gameObject;
            }

        }

        // this.gameObject.transform.rotation = Quaternion.AngleAxis(-90, Vector3.right);
    }
}
