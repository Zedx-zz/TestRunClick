

using com.example.core;
using com.example.core.implementation;
using com.example.ui;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.example.gameLogic
{
    public class Level : MonoBehaviour
    {
        public float InitialSpawnTime = 3;
        public float DecreaseSpawnTimeInterval = 10;
        public float DecreaseSpawnTimeValue = 0.3f;
        public int MistakesToLose = 3;
        public GameObject ObjectFactoryPrefab;
        public GameObject MainUIPrefab;
        public GameObject ResultUIPrefab;
        public Camera MainCam;

        private float m_CurrentSpawnTime = 0;
        private IDGenerator m_IDGenerator;
        private IObjectFactory m_ObjectFactory;
        private ReactiveProperty<int> m_Mistackes;
        private float m_MinYWorldBound;
        private Dictionary<int, Unit> m_Units = new Dictionary<int, Unit>();
        private GameObject m_MainUIGo;

        private System.IDisposable m_PrevTimerDisposable;
        private CompositeDisposable m_Disposables;

        void Awake()
        {
            m_CurrentSpawnTime = InitialSpawnTime;
            m_IDGenerator = new DefaultIDGenerator();

            GameObject factoryGo = Instantiate(ObjectFactoryPrefab);
            DefaultObjectFactory objectFactory = factoryGo.GetComponent<DefaultObjectFactory>();
            objectFactory.SetMainCam(MainCam);

            m_ObjectFactory = objectFactory;
            m_MinYWorldBound = MainCam.ScreenToWorldPoint(new Vector3(0, -100, 0)).y;
        }

        void Start()
        {
            Init();
            CreateMainUI();
        }

        private void Init()
        {
            // decrease spawn time every n seconds
            Observable.Timer(System.TimeSpan.FromMilliseconds(DecreaseSpawnTimeInterval * 1000))
                      .Repeat()
                      .Subscribe(x =>
                      {
                          m_CurrentSpawnTime = Mathf.Max(m_CurrentSpawnTime - DecreaseSpawnTimeValue, 0.5f);
                      })
                      .AddTo(m_Disposables);

            SpawnObject();
            CreateObjectSpawnTimer(InitialSpawnTime);
        }

        private void CreateObjectSpawnTimer(float timeToSpawn)
        {
            if (m_PrevTimerDisposable != null)
                m_PrevTimerDisposable.Dispose();

            m_PrevTimerDisposable = Observable.Timer(System.TimeSpan.FromMilliseconds(timeToSpawn * 1000))
                                        .Subscribe(x =>
                                        {
                                            SpawnObject();
                                            CreateObjectSpawnTimer(m_CurrentSpawnTime);
                                        });
        }

        private void SpawnObject()
        {
            float xPos = Random.Range(100, Screen.width - 100);
            float yPos = Screen.height - 100;
            int rndType = Random.Range(0, 3);

            Vector2 worldPos = MainCam.ScreenToWorldPoint(new Vector2(xPos, yPos));

            Unit unit;

            switch (rndType)
            {
                case 0:
                    unit = m_ObjectFactory.CreateMoveForwardEnemy(m_IDGenerator.GenerateID(), worldPos);

                    break;
                case 1:
                    unit = m_ObjectFactory.CreateMoveForwardDiagonalEnemy(m_IDGenerator.GenerateID(), worldPos);
                    break;

                default:
                    unit = m_ObjectFactory.CreateMoveForwardDiagonalAlly(m_IDGenerator.GenerateID(), worldPos);
                    break;
            }

            AssignUnitInputInteraction(unit);
            AssignUnitOutsideBehaviour(unit);

            m_Units.Add(unit.GetID(), unit);
        }

        private void AssignUnitInputInteraction(Unit unit)
        {
            // check alive unit clicked
            System.IObservable<UniRx.Unit> downClick = unit.OnMouseDownAsObservable()
                                                           .Where(d => !unit.IsDeath);

            downClick.Where(e => unit.IsEnemy == true)
                     .Subscribe(t =>
                     {
                         UnitDeath(unit, 2);
                     })
                     .AddTo(m_Disposables);

            downClick.Where(e => unit.IsEnemy == false)
                     .Subscribe(t =>
                     {
                         UnitDeath(unit, 2);
                         Lose("You killed civilian");
                     })
                     .AddTo(m_Disposables);
        }

        private void AssignUnitOutsideBehaviour(Unit unit)
        {
            // Check unit outside screen
            unit.UpdateAsObservable()
                .Where(d => unit.IsDeath == false)
                .Where(p => unit.transform.position.y <= m_MinYWorldBound)
                .Subscribe(_ =>
                {
                    UnitDeath(unit, 0);

                    if (unit.IsEnemy)
                        ++m_Mistackes.Value;
                })
                .AddTo(m_Disposables);
        }

        private void UnitDeath(Unit unit, float destroyTime)
        {
            unit.Death();

            if (destroyTime > 0)
                Destroy(unit.gameObject, destroyTime);
            else
                Destroy(unit.gameObject);

            m_Units.Remove(unit.GetID());
        }

        private void CreateMainUI()
        {
            m_MainUIGo = Instantiate(MainUIPrefab);
            MainUI mainUi = m_MainUIGo.GetComponent<MainUI>();

            m_Mistackes = new ReactiveProperty<int>(0);
            m_Mistackes.SubscribeToText(mainUi.MistakesCount, t => t + "/" + MistakesToLose)
                        .AddTo(m_Disposables);

            m_Mistackes.Where(v => v >= MistakesToLose)
                        .Subscribe(_ =>
                        {
                            Lose("You are missed too much enemy soldiers");
                        })
                        .AddTo(m_Disposables);

            Observable.EveryUpdate()
                .Select(t => Time.timeSinceLevelLoad.ToString("F2"))
                .SubscribeToText(mainUi.TimeValue)
                .AddTo(m_Disposables);
        }

        private void Lose(string reason)
        {
            foreach (Unit u in m_Units.Values)
                u.enabled = false;

            m_MainUIGo.SetActive(false);
            this.enabled = false;

            GameObject resultGo = Instantiate(ResultUIPrefab);
            ResultUI resultUi = resultGo.GetComponent<ResultUI>();

            resultUi.ReasonText.text = reason;
            resultUi.TimeValue.text = Time.timeSinceLevelLoad.ToString("F2");
            resultUi.RestartButton.OnClickAsObservable().Subscribe(_ =>
            {
                SceneManager.LoadScene("sampleScene");
            });
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

            if (m_PrevTimerDisposable != null)
                m_PrevTimerDisposable.Dispose();
        }
    }
}