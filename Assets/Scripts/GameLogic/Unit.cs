using com.example.core;
using com.example.core.implementation;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace com.example.gameLogic
{
    public class Unit : MonoBehaviour, IMovingObject
    {
        public float InitialMoveSpeed = 1;
        public Vector2 InitialDirection = Vector2.down;
        public float ChangeSpeedTime = 10;
        public float SpeedMultiplayer = 1.1f;
        public bool IsEnemy = true;

        private int m_ID;
        private IMoveSystem m_MoveSystem;
        private CompositeDisposable m_Disposables;

        public bool IsDeath
        {
            get; private set;
        }

        protected CompositeDisposable GetDisposables()
        {
            return m_Disposables;
        }

        void Awake()
        {
            IsDeath = false;
        }

        public virtual void Init(int id, Camera cam)
        {
            m_ID = id;

            float speed = InitialMoveSpeed + InitialMoveSpeed * (Mathf.FloorToInt(Time.timeSinceLevelLoad / ChangeSpeedTime) * SpeedMultiplayer);

            ISpeedProvider speedProvider = new DefaultSpeedProvider(speed);
            IDirectionProvider directionProvider = new DefaultDirectionProvider(InitialDirection);

            Vector2 minScreenBorder = new Vector2(100, 0);
            Vector2 maxScreenBorder = new Vector2(Screen.width - 100, 0);
            Vector2 worldMinScreenBorder = cam.ScreenToWorldPoint(minScreenBorder);
            Vector2 worldMaxScreenBorder = cam.ScreenToWorldPoint(maxScreenBorder);

            m_MoveSystem = new MoveDirectionSystem(transform, speedProvider, directionProvider, worldMinScreenBorder.x, worldMaxScreenBorder.x);

            this.UpdateAsObservable()
                .Subscribe(_ =>
                    {
                        m_MoveSystem.Move(Time.deltaTime);
                    })
                .AddTo(m_Disposables);
        }

        public int GetID()
        {
            return m_ID;
        }

        public IMoveSystem GetMoveSystem()
        {
            return m_MoveSystem;
        }

        public void Death()
        {
            IsDeath = true;

            // release all resources
            m_Disposables.Dispose();

            // decrease alpha - simulate death state
            SpriteRenderer[] rends = this.GetComponentsInChildren<SpriteRenderer>();
            if (rends != null)
                foreach (SpriteRenderer r in rends)
                    r.color = new Color(r.color.r, r.color.g, r.color.b, 0.2f);
        }

        void OnEnable()
        {
            m_Disposables = new CompositeDisposable();
        }

        void OnDisable()
        {
            if (m_Disposables != null)
            {
                m_Disposables.Dispose();
            }
        }
    }
}