
using UnityEngine;

namespace com.example.core.implementation
{
    public class DefaultDirectionProvider : IDirectionProvider
    {
        private Vector2 m_Direction;

        public DefaultDirectionProvider(Vector2 dir)
        {
            m_Direction = dir;
        }

        public Vector2 GetDirection()
        {
            return m_Direction;
        }

        public void SetDirection(Vector2 dir)
        {
            m_Direction = dir;
        }
    }
}