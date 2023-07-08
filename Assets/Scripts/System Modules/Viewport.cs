using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewport : Singleton<Viewport>
{
    float minX;
    float maxX;
    float minY;
    float maxY;
    float middleX;

    void Start() 
    {
        minX = Grounds.Instance.minX;
        minY = Grounds.Instance.minY;
        maxX = Grounds.Instance.maxX;
        maxY = Grounds.Instance.maxY;
    }

    public Vector3 PlayerMoveablePosition(Vector3 playerPosition, float paddingX, float paddingY)
    {
        Vector3 position = Vector3.zero;

        Debug.Log("before: " + playerPosition);
        position.x = Mathf.Clamp(playerPosition.x, minX + paddingX, maxX - paddingX);
        position.y = Mathf.Clamp(playerPosition.y, minY + paddingY, maxY - paddingY);
        Debug.Log("after: " + position);

        return position;
    }

    public Vector3 RandomEnemySpawnPosition(float paddingX, float paddingY)
    {
        Vector3 position = Vector3.zero;

        position.x = maxX + paddingX;
        position.y = Random.Range(minY + paddingY, maxY - paddingY);

        return position;
    }

    public Vector3 RandomRightHalfPosition(float paddingX, float paddingY)
    {
        Vector3 position = Vector3.zero;

        position.x = Random.Range(middleX, maxX - paddingX);
        position.y = Random.Range(minY + paddingY, maxY - paddingY);

        return position;
    }

    // enemy all position
    public Vector3 RandomEnemyMovePosition(float paddingX, float paddingY)
    {
        Vector3 position = Vector3.zero;

        position.x = Random.Range(minX + paddingX, maxX - paddingX);
        position.y = Random.Range(minY + paddingY, maxY - paddingY);

        return position;
    }

    public Vector3 FollowPosition(Vector3 main, Vector3 target, Vector3 minPosition, Vector3 maxPosition, float somthing)
    {
        if (main == target) return main;

        // target.x = Mathf.Clamp(target.x, minPosition.x, maxPosition.x);
        // target.y = Mathf.Clamp(target.y, minPosition.y, maxPosition.y);
        // target.z = main.z;

        var z = main.z;
        main = Vector3.Lerp(main, target, somthing);
        main.z = z;
        return main;
    }

}