
namespace com.example.core.implementation
{
    public class DefaultIDGenerator : IDGenerator
    {
        private int m_LastID = 0;

        public int GenerateID()
        {
            return m_LastID++;
        }
    }
}