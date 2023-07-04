using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Grounds : MonoBehaviour
{
    public const string GROUND = "Ground";

    [SerializeField] int width;
    [SerializeField] int height;
    private GameObject gridPre;
    private Grid[,] grids;

    [System.Obsolete]
    void Awake()
    {
        Addressables.LoadAsset<GameObject>(GROUND).Completed += (obj) => {
            gridPre = obj.Result;
            CreateGrid(width, height);
        };
    }

    public void CreateMesh()
    {
        
    }

     public void CreateGrid(int row,int column)
    {
        GameObject go = GameObject.Instantiate(gridPre, this.gameObject.transform);
        Grid grid = go.GetComponent<Grid>();
 
        float posX = startPos.x + grid.gridWidght * row;
        float posZ = startPos.z + grid.girdHeight * column;
        go.transform.position = new Vector3(posX, startPos.y, posZ);
        grids[row, column] = grid;
        gridEvent?.Invoke(grid);
    }


}
