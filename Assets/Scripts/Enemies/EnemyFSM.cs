
using UnityEngine;

public class MovingToPlayerState : BaseState
{
    public override void OnEnter(FSMSystem fsm)
    {
        fsm.holder.NextMove();
    }
    public override void OnExit(FSMSystem fsm)
    {
    }
    public override void OnStay(FSMSystem fsm)
    {
        float distance = fsm.holder.CheckPlayerDistance();
        if (distance >= fsm.holder.nextMove) fsm.holder.MovingToPlayer();
        if (distance <= 7) fsm.SwitchState<PubchingToPlayerState>();
    }
}
public class PubchingToPlayerState : BaseState
{
    public override void OnEnter(FSMSystem fsm)
    {
        fsm.holder.CheckMoveDirection();
    }
    public override void OnExit(FSMSystem fsm)
    {
    }
    public override void OnStay(FSMSystem fsm)
    {
        if (!fsm.holder.CheckPubchingPosition()) fsm.holder.PubchingToPlayer();
        else fsm.SwitchState<MovingToPlayerState>();
    }
}
public class EscapePlayerState : BaseState
{
    public override void OnEnter(FSMSystem fsm)
    {
    }
    public override void OnExit(FSMSystem fsm)
    {
    }
    public override void OnStay(FSMSystem fsm)
    {
    }
}