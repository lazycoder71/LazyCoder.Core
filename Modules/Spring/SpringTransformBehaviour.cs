using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Threading;
using UnityEngine;

namespace LazyCoder.Spring
{
    public class SpringTransformBehaviour : MonoBase
    {
        [Title("Config")]
        [Range(0.01f, 100f)]
        [SerializeField] protected float Damping = 5f;

        [Range(0, 500)]
        [SerializeField] protected float Stiffness = 200f;

        private CancellationTokenSource _ctsPosition;
        private CancellationTokenSource _ctsRotation;
        private CancellationTokenSource _ctsScale;

        private SpringVector3 _springPosition;
        private SpringVector3 _springRotation;
        private SpringVector3 _springScale;

        protected override void OnDisable()
        {
            base.OnDisable();

            _ctsPosition?.Cancel();
            _ctsPosition.Dispose();

            _ctsRotation?.Cancel();
            _ctsRotation?.Dispose();

            _ctsScale?.Cancel();
            _ctsScale?.Dispose();
        }

        private void InitSpring(ref SpringVector3 spring, Vector3 startValue, Vector3 endValue)
        {
            if (spring == null)
            {
                spring = new SpringVector3()
                {
                    Damping = Damping,
                    Stiffness = Stiffness,
                    StartValue = startValue,
                    EndValue = endValue,
                };
            }
            else
            {
                spring.Damping = Damping;
                spring.Stiffness = Stiffness;
                spring.StartValue = startValue;
                spring.EndValue = endValue;
            }
        }

        private async UniTask SpringValueAsync(SpringVector3 spring, CancellationTokenSource cts, Action<Vector3> setter, Func<Vector3> getter)
        {
            while (true)
            {
                setter.Invoke(spring.Evaluate(Time.deltaTime));

                if (Mathf.Approximately(Vector3.SqrMagnitude(getter.Invoke() - spring.EndValue), 0f))
                    break;

                await UniTask.Yield(cts.Token);
            }

            spring.Reset();
        }

        private void SpringValue(ref SpringVector3 spring, ref CancellationTokenSource cts, Vector3 valueStart, Vector3 valueEnd, Action<Vector3> setter, Func<Vector3> getter)
        {
            InitSpring(ref spring, valueStart, valueEnd);

            if (Mathf.Approximately(spring.CurrentVelocity.sqrMagnitude, 0))
            {
                spring.Reset();
                spring.StartValue = valueStart;
                spring.EndValue = valueEnd;
            }
            else
            {
                spring.UpdateEndValue(valueEnd, spring.CurrentVelocity);
            }

            cts?.Cancel();
            cts = new CancellationTokenSource();

            SpringValueAsync(spring, cts, setter, getter).Forget();
        }

        private void NudegeValue(ref SpringVector3 spring, ref CancellationTokenSource cts, Vector3 amount, Action<Vector3> setter, Func<Vector3> getter)
        {
            if (spring == null)
                InitSpring(ref spring, getter.Invoke(), getter.Invoke());
            else
                InitSpring(ref spring, spring.StartValue, spring.EndValue);

            if (Mathf.Approximately(spring.CurrentVelocity.sqrMagnitude, 0))
            {
                spring.Reset();
                spring.StartValue = getter.Invoke();
                spring.EndValue = getter.Invoke();
                spring.InitialVelocity = amount;

                cts?.Cancel();
                cts = new CancellationTokenSource();

                SpringValueAsync(spring, cts, setter, getter).Forget();
            }
            else
            {
                spring.UpdateEndValue(spring.EndValue, spring.CurrentVelocity + amount);
            }
        }

        public void SpringPosition(Vector3 positionEnd)
        {
            SpringPosition(TransformCached.position, positionEnd);
        }

        public void SpringPosition(Vector3 positionStart, Vector3 positionEnd)
        {
            SpringValue(ref _springPosition, ref _ctsPosition, positionStart, positionEnd, x => TransformCached.localPosition = x, () => TransformCached.localPosition);
        }

        public void NudgePosition(Vector3 amount)
        {
            NudegeValue(ref _springPosition, ref _ctsPosition, amount, x => TransformCached.localPosition = x, () => TransformCached.localPosition);
        }

        public void SpringRotation(Vector3 eulerEnd)
        {
            SpringRotation(TransformCached.localEulerAngles, eulerEnd);
        }

        public void SpringRotation(Vector3 eulerStart, Vector3 eulerEnd)
        {
            SpringValue(ref _springRotation, ref _ctsRotation, eulerStart, eulerEnd, (x) => TransformCached.localEulerAngles = x, () => TransformCached.localEulerAngles);
        }

        public void NudeRotation(Vector3 amount)
        {
            NudegeValue(ref _springRotation, ref _ctsPosition, amount, (x) => TransformCached.localEulerAngles = x, () => TransformCached.localEulerAngles);
        }

        public void SpringScale(Vector3 scaleEnd)
        {
            SpringScale(TransformCached.localScale, scaleEnd);
        }

        public void SpringScale(Vector3 scaleStart, Vector3 scaleEnd)
        {
            SpringValue(ref _springScale, ref _ctsScale, scaleStart, scaleEnd, (x) => TransformCached.localScale = x, () => TransformCached.localScale);
        }

        public void NudgeScale(Vector3 amount)
        {
            NudegeValue(ref _springScale, ref _ctsScale, amount, (x) => TransformCached.localScale = x, () => TransformCached.localScale);
        }
    }
}