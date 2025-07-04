using UnityEngine;

public interface ICombatant
{
    void TakeDamage(float damage);
    bool ReceiveHit(string actionName, int damage);
    string GetCurrentAction();
    Animator GetAnimator();
}