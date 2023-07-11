using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct EnemyControllerJob: IJob
{
    public float3 TargetPositions;
    public float3 SeekerPositions;
    public float MoveSpeed;
    public NativeReference<float3> NearestTargetPositions;
    public void Execute()
    {
        // Compute the square distance from each seeker to every target.
        float3 seekerPos = SeekerPositions;
        float3 val = NearestTargetPositions.Value;

        float nearestDistSq = float.MaxValue;
        float distSq = math.distancesq(seekerPos, TargetPositions);
        if (distSq < nearestDistSq)
        {
            nearestDistSq = distSq;
            Vector3 towards = Vector3.MoveTowards(seekerPos, TargetPositions, MoveSpeed * Time.fixedDeltaTime);

            val = new float3(towards.x, towards.y, towards.z);

            NearestTargetPositions.Value = val;
        }
    }
}