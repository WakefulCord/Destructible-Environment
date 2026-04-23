using System;
using UnityEngine;

public interface IDestructable 
{
    DestructionLayer GetLayer { get; }
    public void ApplyDamage(DestructionHitData hitData);
}
public struct DestructionHitData 
{
    public float damage;
    public float radius;
    public Vector3 hitPoint;
    public Vector3 hitNormal;
}

[System.Flags]
public enum DestructionLayer 
{
    None = 0,
    MarchingCubes = 1 << 0,
    VoxelWall = 1 << 1,
    WallSwitch = 1 << 2,
    
}