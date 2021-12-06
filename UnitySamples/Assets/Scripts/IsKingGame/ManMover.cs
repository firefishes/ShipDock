using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManMover : MonoBehaviour
{
    [SerializeField]
    private Transform m_Target;
    [SerializeField]
    private Pathfinding.AIPath m_AI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_AI != default && m_Target != default)
        {
            m_AI.destination = m_Target.position;
        }
    }
}
