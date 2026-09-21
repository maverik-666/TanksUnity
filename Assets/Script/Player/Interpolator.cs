namespace Project
{
    using UnityEngine;

    public class Interpolator
    {
        private float _timeElapsed = 0;
        private float _timeToReachTarget = 0.05f;
        private float _lastStateReceivedTime = -1f;

        private PlayerState _to;
        private Vector2 _positionCorrection;
        private float _tankRotCorrection;
        private float _turretRotCorrection;

        private Vector2 _currentPos;
        private float _currentTankRot;
        private float _currentTurretRot;
        private Vector2 _posVelocity;
        private float _tankRotVelocity;
        private float _turretRotVelocity;
        
        private Vector2 _capPos;
        private Vector2 _capVelocity;
        private float _capTankRot;
        private bool _capComputed = false;

        private readonly NetworkPlayer _player;

        private const float MaxCorrectionDistance = 2f;
        private const float MaxInterpolationTime = 0.25f;
        private const float SmoothTime = 0.075f;

        public Interpolator(NetworkPlayer player, PlayerState initialState)
        {
            _player = player;
            SetState(initialState);
        }

        public void SetState(PlayerState state)
        {
            _to = state;
            _currentPos = state.Position;
            _currentTankRot = state.TankRotation;
            _currentTurretRot = state.TurretRotation;
            _positionCorrection = Vector2.zero;
            _tankRotCorrection = 0f;
            _turretRotCorrection = 0f;
            _timeElapsed = 0f;
            _capComputed = false;
            _posVelocity = Vector2.zero;
            _tankRotVelocity = 0f;
            _turretRotVelocity = 0f;
        }

        public void Update()
        {
            _timeElapsed += Time.deltaTime;
            float correctionFactor = 1f - Mathf.Clamp01(_timeElapsed / _timeToReachTarget);

            Vector2 targetPos;
            float targetTankRot;

            if (_timeElapsed <= MaxInterpolationTime)
            {
                _capComputed = false;
                targetPos = ExtrapolatePosition(_to.Position, _to.Velocity, _to.TankAngularVelocity, _timeElapsed)
                            + _positionCorrection * correctionFactor;
                targetTankRot = _to.TankRotation + _to.TankAngularVelocity * _timeElapsed + _tankRotCorrection * correctionFactor;
            }
            else
            {
                if (!_capComputed)
                {
                    _capPos = ExtrapolatePosition(_to.Position, _to.Velocity, _to.TankAngularVelocity, MaxInterpolationTime);
                    _capTankRot = _to.TankRotation + _to.TankAngularVelocity * MaxInterpolationTime;

                    float omega = _to.TankAngularVelocity * Mathf.Deg2Rad;
                    if (Mathf.Abs(omega) < 0.001f)
                    {
                        _capVelocity = _to.Velocity;
                    }
                    else
                    {
                        float angle = omega * MaxInterpolationTime;
                        _capVelocity = new Vector2(
                            _to.Velocity.x * Mathf.Cos(angle) - _to.Velocity.y * Mathf.Sin(angle),
                            _to.Velocity.x * Mathf.Sin(angle) + _to.Velocity.y * Mathf.Cos(angle)
                        );
                    }

                    _capComputed = true;
                }

                float extraTime = _timeElapsed - MaxInterpolationTime;
                targetPos = _capPos + _capVelocity * extraTime + _positionCorrection * correctionFactor;
                targetTankRot = _capTankRot + _tankRotCorrection * correctionFactor;
            }

            float targetTurretRot = _to.TurretRotation + _turretRotCorrection * correctionFactor;

            _currentPos = Vector2.SmoothDamp(_currentPos, targetPos, ref _posVelocity, SmoothTime);
            _currentTankRot = Mathf.SmoothDampAngle(_currentTankRot, targetTankRot, ref _tankRotVelocity, SmoothTime);
            _currentTurretRot = Mathf.SmoothDampAngle(_currentTurretRot, targetTurretRot, ref _turretRotVelocity, SmoothTime);

            _player.SetState(_currentPos, _currentTankRot, _currentTurretRot);
        }

        private static Vector2 ExtrapolatePosition(Vector2 position, Vector2 velocity, float angularVelocityDeg, float dt)
        {
            float omega = angularVelocityDeg * Mathf.Deg2Rad;

            if (Mathf.Abs(omega) < 0.001f)
                return position + velocity * dt;

            float sinOmegaT = Mathf.Sin(omega * dt);
            float cosOmegaT = Mathf.Cos(omega * dt);

            return new Vector2(
                position.x + (velocity.x * sinOmegaT + velocity.y * (cosOmegaT - 1f)) / omega,
                position.y + (velocity.x * (1f - cosOmegaT) + velocity.y * sinOmegaT) / omega
            );
        }

        public void AddState(PlayerState state)
        {
            if (state.Tick <= _to.Tick)
                return;

            float now = Time.time;
            _timeToReachTarget = _lastStateReceivedTime >= 0
                ? Mathf.Min(now - _lastStateReceivedTime, MaxInterpolationTime)
                : Time.fixedDeltaTime;
            _lastStateReceivedTime = now;

            _capComputed = false;

            var current = _player.GetCurrentState();
            _positionCorrection = current.Position - state.Position;

            if (_positionCorrection.sqrMagnitude > MaxCorrectionDistance * MaxCorrectionDistance)
            {
                _positionCorrection = Vector2.zero;
                _tankRotCorrection = 0f;
                _turretRotCorrection = 0f;
                _currentPos = state.Position;
                _currentTankRot = state.TankRotation;
                _currentTurretRot = state.TurretRotation;
                _posVelocity = Vector2.zero;
                _tankRotVelocity = 0f;
                _turretRotVelocity = 0f;
            }
            else
            {
                _tankRotCorrection = Mathf.DeltaAngle(state.TankRotation, current.TankRotation);
                _turretRotCorrection = Mathf.DeltaAngle(state.TurretRotation, current.TurretRotation);
            }

            _to = state;
            _timeElapsed = 0;
        }
    }
}
