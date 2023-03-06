using UnityEngine;

namespace USCSL
{
    #region Float

    public class SmoothDampFloat
    {
        private bool _setup;
        private float _value;
        private float _velocity;

        public SmoothDampFloat() {}
        
        public SmoothDampFloat(float initValue)
        {
            _value = initValue;
            _velocity = 0;
            _setup = true;
        }
        
        public float Update(float target, float smooth)
        {
            if (!this._setup)
            {
                this._value = target;
                this._velocity = 0.0f;
                this._setup = true;
            }

            this._value = Mathf.SmoothDamp(
                this._value,
                target,
                ref _velocity,
                smooth
            );

            if (Mathf.Approximately(_value, target))
            {
                this._value = target;
            }

            return this._value;
        }

        public float Get()
        {
            return _value;
        }

        public void Set(float value)
        {
            this._value = value;
            this._velocity = 0.0f;
        }

        public void Reset()
        {
            this._value = 0.0f;
            this._velocity = 0.0f;
            this._setup = false;
        }

        public static implicit operator float(SmoothDampFloat dampFloat)
        {
            return dampFloat.Get();
        }
        
        public static implicit operator SmoothDampFloat(float dampFloat)
        {
            SmoothDampFloat r = new SmoothDampFloat(dampFloat);
            return r;
        }
    }

    public class SmoothStepFloat
    {
        private bool _setup;
        private bool _updateBasedOnFrame;
        private float _start;
        private float _target;
        private float _currentValue;
        private float _time;
        private float _progress;
        private int _lastFrame;

        void Setup(float start, float target, float time, bool updateBasedOnFrame = true)
        {
            this._start = start;
            this._target = target;
            this._time = time;
            this._updateBasedOnFrame = updateBasedOnFrame;
            _setup = true;
        }

        public float Update()
        {
            if (_updateBasedOnFrame && _lastFrame == Time.frameCount) return _currentValue;

            _lastFrame = Time.frameCount;

            if (!_setup)
            {
                Debug.LogWarning("SmoothStepFloat update called without prior setup! Returning 0.0f");
                return 0.0f;
            }

            _progress = Mathf.Clamp01(Time.deltaTime * (1.0f / _time));
            if (Mathf.Approximately(_progress, 1.0f))
            {
                _progress = 1;
            }
            return _currentValue = Mathf.SmoothStep(_start, _target, _progress);
        }

        public float Get()
        {
            return _currentValue;
        }

        public void Reset(bool onlyResetProgress = false)
        {
            if (!onlyResetProgress)
            {
                _start = 0.0f;
                _target = 0.0f;
                _updateBasedOnFrame = true;
                _target = _progress;
                _time = 0.0f;
                _setup = false;
            }

            _progress = 0.0f;
        }
    }

    #endregion
}