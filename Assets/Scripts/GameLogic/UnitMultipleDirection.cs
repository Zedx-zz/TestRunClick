using com.example.core;
using UniRx;
using UnityEngine;

namespace com.example.gameLogic
{
    public class UnitMultipleDirection : Unit
    {
        public float ChangeDirectionTime = 2;
        public float DecreaseChangeTime = 10;
        public float DecreasePerTimeValue = 0.2f;
        public Vector2[] PossibleDirections = new Vector2[]
        {
                    Vector2.down,
                    (Vector2.down + Vector2.left).normalized,
                    (Vector2.down + Vector2.right).normalized
        };

        public override void Init(int id, Camera cam)
        {
            base.Init(id, cam);

            CompositeDisposable disposables = GetDisposables();
            IDirectionProvider directionProvider = GetMoveSystem().GetDirectionProvider();

            float changeDirTime = Mathf.Max(ChangeDirectionTime - ChangeDirectionTime * (Mathf.FloorToInt(Time.timeSinceLevelLoad / DecreaseChangeTime) * DecreasePerTimeValue), 0.2f);

            // Change direction every b seconds
            Observable.Timer(System.TimeSpan.FromMilliseconds(changeDirTime * 1000))
                      .Repeat()
                      .Subscribe(_ =>
                      {
                          Vector3 rndDir = PossibleDirections[Random.Range(0, 3)];
                          directionProvider.SetDirection(rndDir);
                      })
                      .AddTo(disposables);
        }
    }
}