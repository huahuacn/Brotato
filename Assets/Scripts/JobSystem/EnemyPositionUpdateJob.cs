using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

[BurstCompile]
public struct EnemyPositionUpdateJob : IJobParallelForTransform
{
    
    // 给每个物体设置一个速度
    [ReadOnly]
    public float velocity;
    public Vector3 playerPosition;
    public float fixDeltaTime;


    public void Execute(int i, TransformAccess transform)
    {
        if (Vector3.Distance(transform.position, playerPosition) >= velocity * fixDeltaTime)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerPosition, velocity * fixDeltaTime);
        }
    }
}