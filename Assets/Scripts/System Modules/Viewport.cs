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
        Camera mainCamera = Camera.main;

        // TOOD why 
        Vector2 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f));
        Vector2 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f));

        minX = bottomLeft.x;
        minY = bottomLeft.y;
        maxX = topRight.x;
        maxY = topRight.y;

        middleX = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0f, 0f)).x;
    }

    public Vector3 PlayerMoveablePosition(Vector3 playerPosition, float paddingX, float paddingY)
    {
        Vector3 position = Vector3.zero;

        position.x = Mathf.Clamp(playerPosition.x, minX + paddingX, maxX - paddingX);
        position.y = Mathf.Clamp(playerPosition.y, minY + paddingY, maxY - paddingY);

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

    public void CameraFollow(GameObject followTarget)
    {
        if (followTarget != null)
        {
            //从 摄像机 开始 向 屏幕中心 的2D 坐标 发射 射线
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));//射线
 
            Vector3 vec_cam = Camera.main.transform.position;
            Vector3 dir = ray.direction;
            float num = (0 - ray.origin.y) / dir.y;//射线上 Y=0的 坐标点
            var vec = ray.origin + ray.direction * num;//获得 屏幕中心点 对应 Y=0 平面 的坐标点
 
            vec = vec - new Vector3(vec_cam.x,0, vec_cam.z);//跟随目标 相对于 摄像机的 偏移值
 
            Camera.main.transform.position = -vec + new Vector3(
                followTarget.transform.position.x,
                vec_cam.y,
                followTarget.transform.position.z
            );//在不改变 摄像机transform.position.y值的情况下 在XZ平面进行移动
            print("  vec:" + vec);
        }//
        else return;
    }

}