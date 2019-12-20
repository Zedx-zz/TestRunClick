
using UnityEngine;

namespace com.example.core.implementation
{
    public class MoveDirectionSystem : IMoveSystem
    {
        private Transform m_Trans;
        private ISpeedProvider m_SpeedProvider;
        private IDirectionProvider m_DirectionProvider;
        private float m_MinWorldX;
        private float m_MaxWorldX;

        public MoveDirectionSystem(Transform trans, ISpeedProvider speedProvider, IDirectionProvider directionProvider, float minWorldX, float maxWorldX)
        {
            m_Trans = trans;
            m_SpeedProvider = speedProvider;
            m_DirectionProvider = directionProvider;
            m_MinWorldX = minWorldX;
            m_MaxWorldX = maxWorldX;
        }

        public IDirectionProvider GetDirectionProvider()
        {
            return m_DirectionProvider;
        }

        public ISpeedProvider GetSpeedProvider()
        {
            return m_SpeedProvider;
        }

        private void ReflectDir()
        {
            Vector2 curDir = m_DirectionProvider.GetDirection();
            curDir.x = curDir.x * -1;

            m_DirectionProvider.SetDirection(curDir);
        }

        public void Move(float t)
        {
            Vector3 nextPos = m_Trans.position + new Vector3(m_DirectionProvider.GetDirection().x * m_SpeedProvider.GetSpeed() * t,
                                                             m_DirectionProvider.GetDirection().y * m_SpeedProvider.GetSpeed() * t);

            if (nextPos.x < m_MinWorldX)
            {
                nextPos.x -= nextPos.x - m_MinWorldX;
                ReflectDir();
            }
            else if (nextPos.x > m_MaxWorldX)
            {
                nextPos.x -= nextPos.x - m_MaxWorldX;
                ReflectDir();
            }

            m_Trans.position = nextPos;
        }
    }
}