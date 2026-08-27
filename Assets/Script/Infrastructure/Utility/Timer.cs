namespace Project
{
    public class Timer
    {
        public bool IsFinished => _currentTime <= 0f;
        public bool IsRunning => _currentTime > 0f;
        
        private float _startTime = 0;
        private float _currentTime = 0;

        private readonly TimerUpdateHandler _timerUpdateHandler = null; 
        
        public Timer(TimerUpdateHandler timerUpdateHandler)
        {
            _timerUpdateHandler = timerUpdateHandler;
        }
        
        public void Setup(float startTime)
        {
            _startTime = _currentTime = startTime;

            _timerUpdateHandler.Register(this);
        }

        public void Tick(float deltaTime)
        {
            _currentTime -= deltaTime;
        }
    }
}