using UnityEngine;

public class RoleRes : MonoBehaviour
{
    [Header("»ù´¡")]
    [SerializeField]
    private Transform m_AimPoint;
    [SerializeField]
    private GameObject m_SignTarget;
    [SerializeField]
    private CapsuleCollider m_BodyCollider;
    [SerializeField]
    private Animator m_Animator;
    [SerializeField]
    private float m_SpeedMove;
    [SerializeField]
    private float m_SpeedRotate;
    [Header("ÎäÆ÷")]
    [SerializeField]
    protected Transform m_WeaponFirePoint;
    [SerializeField]
    protected GameObject m_BulletRes;

    public float MoveSpeed
    {
        get
        {
            return m_SpeedMove;
        }
    }

    public float RotateSpeed
    {
        get
        {
            return m_SpeedRotate;
        }
    }

    public Animator Animator
    {
        get
        {
            return m_Animator;
        }
    }

    public Transform WeaponFirePoint
    {
        get
        {
            return m_WeaponFirePoint;
        }
    }

    public GameObject BulletRes
    {
        get
        {
            return m_BulletRes;
        }
    }
}
