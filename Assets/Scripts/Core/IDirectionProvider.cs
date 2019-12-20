

using UnityEngine;

namespace com.example.core
{
    public interface IDirectionProvider
    {
        Vector2 GetDirection();
        void SetDirection(Vector2 dir);
    }
}