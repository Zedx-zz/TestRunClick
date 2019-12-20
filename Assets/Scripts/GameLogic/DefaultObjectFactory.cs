using UnityEngine;

namespace com.example.gameLogic
{
    public class DefaultObjectFactory : MonoBehaviour, IObjectFactory
    {
        public GameObject MoveForwardEnemyPrefab;
        public GameObject MoveForwardDiagonalEnemyPrefab;
        public GameObject MoveForwardDiagonalAllyPrefab;

        private Camera m_MainCam;

        public void SetMainCam(Camera cam)
        {
            m_MainCam = cam;
        }

        public Unit CreateMoveForwardEnemy(int id, Vector2 pos)
        {
            GameObject enemyGo = Instantiate(MoveForwardEnemyPrefab, pos, Quaternion.identity);
            Unit unit = enemyGo.GetComponent<Unit>();
            unit.Init(id, m_MainCam);

            return unit;
        }

        public Unit CreateMoveForwardDiagonalEnemy(int id, Vector2 pos)
        {
            GameObject enemyGo = Instantiate(MoveForwardDiagonalEnemyPrefab, pos, Quaternion.identity);
            Unit unit = enemyGo.GetComponent<Unit>();
            unit.Init(id, m_MainCam);

            return unit;
        }

        public Unit CreateMoveForwardDiagonalAlly(int id, Vector2 pos)
        {
            GameObject enemyGo = Instantiate(MoveForwardDiagonalAllyPrefab, pos, Quaternion.identity);
            Unit unit = enemyGo.GetComponent<Unit>();
            unit.Init(id, m_MainCam);

            return unit;
        }
    }
}