using UnityEngine;

public class DeadState : BaseState
{
    public override void Enter()
    {
        // Zatrzymaj agenta NavMesh
        if (enemy.Agent != null)
        {
            enemy.Agent.isStopped = true;
            enemy.Agent.updatePosition = false;
            enemy.Agent.updateRotation = false;
            enemy.Agent.ResetPath(); // Resetuj ścieżkę, aby agent nie próbował się poruszać
        }
    }

    public override void Perform()
    {
        // Nic nie robi – przeciwnik jest martwy
    }

    public override void Exit()
    {
        // Nie ma potrzeby nic robić w Exit, ponieważ martwy przeciwnik nie powinien opuszczać tego stanu
    }
}
