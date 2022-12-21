using UnityEngine;

namespace IsKing
{
    public class HeroComponent : IsKingRoleComponent
    {
        protected override void InitRoleComponents()
        {
            base.InitRoleComponents();

            int entitas = m_EntityComponent.Entitas;
            mMovementComponent.Aacceleration(entitas, 100f);
            mMovementComponent.ShouldMove(entitas, false);
        }

        private void Update()
        {
            if (IsUpdateValid())
            {
                int entitas = m_EntityComponent.Entitas;

                Vector2 direction = Vector2.zero;

                const string HAxisName = "Horizontal";
                const string VAxisName = "Vertical";

                float h = Input.GetAxis(HAxisName);
                float v = Input.GetAxis(VAxisName);

                if (h != 0f)
                {
                    direction.x = -h;
                }
                else { }


                if (v != 0f)
                {
                    direction.y = v;
                }
                else { }

                bool flag = direction != Vector2.zero;
                if (flag)
                {
                    Vector3 pos = mMovementComponent.GetLocalPosition(entitas);
                    transform.localPosition = pos;

                    float speedMax = mMovementComponent.GetMoveSpeedMax(entitas);

                    h = Mathf.Abs(h);
                    v = Mathf.Abs(v);

                    mMovementComponent.MoveSpeed(entitas, Mathf.Max(h, v) * speedMax);
                }
                else
                {
                    mMovementComponent.MoveSpeed(entitas, 0f);
                }

                mMovementComponent.ShouldMove(entitas, flag);
                mMovementComponent.MoveDirection(entitas, direction);
            }
            else { }
        }

        private void LateUpdate()
        {
            if (IsUpdateValid())
            {
            }
            else { }
        }

        private bool IsUpdateValid()
        {
            return m_EntityComponent != default;
        }
    }

}