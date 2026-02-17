using StateMachineAAI;

Entity entity = new Entity(10, new StatePatrol());

while (true)
{
    entity.Update();

    var currentStateType = entity.state.GetType();

    if (currentStateType == typeof(StatePatrol) && entity.EnemyClose())
    {
        if (entity.strength >= 10)
        {
            entity.ChangeState(new StateAttack());
        }
        else
        {
            entity.ChangeState(new StateHide());
        }
    }
    else if (currentStateType == typeof(StateAttack) && entity.strength < 5)
    {
        entity.ChangeState(new StateHide());
    }
    else if (currentStateType == typeof(StateHide) && !entity.EnemyClose())
    {
        entity.ChangeState(new StatePatrol());
    }

    Thread.Sleep(1000);
}