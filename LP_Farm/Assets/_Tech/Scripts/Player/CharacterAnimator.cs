using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimator : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator _animator;
    private int _speedForwHash = Animator.StringToHash("SpeedForward"),
                _speedSideHash = Animator.StringToHash("SpeedSide");

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Quaternion r = Quaternion.Euler(0, Vector3.SignedAngle(transform.forward, Vector3.forward, Vector3.up), 0);

        Vector3 rotatedVel = r * _agent.velocity / _agent.speed;

        _animator.SetFloat(_speedForwHash, rotatedVel.z);
        _animator.SetFloat(_speedSideHash, rotatedVel.x);
    }
}
