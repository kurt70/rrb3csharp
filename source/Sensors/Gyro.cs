using System;

namespace Sensors
{
    public class Gyro
    {
        private Func<GyroData> _callback;

        /// <summary>
        /// Initialization
        /// </summary>
        public Gyro(Func<GyroData> callback)
        {
            _callback = callback;
        }

        public void Start()
        {
            //DO stuff;

        }
    }
}
