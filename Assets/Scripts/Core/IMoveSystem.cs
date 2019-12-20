

namespace com.example.core
{
    public interface IMoveSystem
    {
        ISpeedProvider GetSpeedProvider();
        IDirectionProvider GetDirectionProvider();
        void Move(float t);
    }
}