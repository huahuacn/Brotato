using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态基类
/// </summary>
public abstract class BaseState
{
    public abstract void OnEnter(FSMSystem fsm);
    public abstract void OnStay(FSMSystem fsm);
    public abstract void OnExit(FSMSystem fsm);
}

/// <summary>
/// 有限状态机
/// </summary>
public class FSMSystem
{
    private Dictionary<Type, BaseState> stateCache = new Dictionary<Type, BaseState>();//所有状态

    public BaseState CurState { get; set; }//当前状态

    //FSM持有者
    public EnemyController holder { get; private set; }
    public void SetHolder(EnemyController holder)
    {
        this.holder = holder;
    }

    /// <summary>
    /// 添加状态
    /// </summary>
    public void AddState(BaseState state)
    {
        if (stateCache.ContainsKey(state.GetType()))
        {
            Debug.LogError("已经存在此状态：" + state.GetType());
            return;
        }
        stateCache.Add(state.GetType(), state);
    }

    /// <summary>
    /// 删除状态
    /// </summary>
    public void DeleteState(BaseState state)
    {
        if (!stateCache.ContainsKey(state.GetType()))
        {
            Debug.LogError("不存在此状态：" + state.GetType());
            return;
        }
        stateCache.Remove(state.GetType());
    }

    /// <summary>
    /// 切换状态
    /// </summary>
    public void SwitchState<T>()
        where T : BaseState
    {
        Type type = typeof(T);
        if (!stateCache.ContainsKey(type))
        {
            Debug.LogError("不存在此状态：" + type);
            return;
        }
        if (CurState != null)
        {
            CurState.OnExit(this);
        }
        BaseState state = stateCache[type];
        state.OnEnter(this);
        CurState = state;
    }

    /// <summary>
    /// 切换状态
    /// </summary>
    public void SwitchState(Type type)
    {
        if (!stateCache.ContainsKey(type))
        {
            Debug.LogError("不存在此状态：" + type);
            return;
        }
        if (CurState != null)
        {
            CurState.OnExit(this);
        }
        BaseState state = stateCache[type];
        state.OnEnter(this);
        CurState = state;
    }
}