using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoApproach : MonoBehaviour
{
    [SerializeField] private float _approachSpeed = 2f;
    [SerializeField] private float _retreatDistance = 1f;
    [SerializeField] private Transform _target;
    [SerializeField] private float _combatRange = 0.5f;

    void Update()
    {
        if (Vector3.Distance(transform.position, _target.position) > _combatRange)
        {
            // Move towards the target if outside combat range
            Vector3 direction = (_target.position - transform.position).normalized;
            transform.position += direction * _approachSpeed * Time.deltaTime;
        }
        else
        {
            // Stop moving when within combat range
            transform.position = new Vector3(transform.position.x, transform.position.y, _target.position.z);
        }
    }
}
