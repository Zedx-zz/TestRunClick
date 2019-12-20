
namespace com.example.core.implementation
{
    public class DefaultSpeedProvider : ISpeedProvider
    {
        private float m_Speed;

        public DefaultSpeedProvider(float speed)
        {
            m_Speed = speed;
        }

        public float GetSpeed()
        {
            return m_Speed;
        }

        public void SetSpeed(float speed)
        {
            m_Speed = speed;
        }
    }
}