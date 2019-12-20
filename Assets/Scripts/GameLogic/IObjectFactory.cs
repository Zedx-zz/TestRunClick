

using UnityEngine;

namespace com.example.gameLogic
{
    public interface IObjectFactory
    {
        Unit CreateMoveForwardEnemy(int id, Vector2 pos);
        Unit CreateMoveForwardDiagonalEnemy(int id, Vector2 pos);
        Unit CreateMoveForwardDiagonalAlly(int id, Vector2 pos);
    }
}