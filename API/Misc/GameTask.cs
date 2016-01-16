using System;

namespace OTA.Misc
{
    /// <summary>
    /// A triggerable task that can be used call a function at intervals
    /// </summary>
    public class GameTask /* Needed this to be a reference type */
    {
        //public static readonly Task Empty;

        private DateTime _insertedAt;
        private bool _enabled;

        public object Data;

        public bool Triggerable
        {
            get
            {
                var span = (DateTime.Now - _insertedAt).TotalSeconds;
                return _enabled && span >= Trigger;
            }
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;

                if (_enabled) Reset(false);
            }
        }

        /// <summary>
        /// Gets or sets the trigger in Seconds.
        /// </summary>
        /// <value>
        /// The trigger.
        /// </value>
        public int Trigger { get; set; }

        /// <summary>
        /// Informs if the tack has been performed at leat once
        /// </summary>
        /// <value>
        /// The trigger.
        /// </value>
        public bool HasTriggered { get; private set; }

        /// <summary>
        /// Gets or sets the method to be called.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        public Action<GameTask> Method { get; set; }

        public void Reset(bool triggered = true, bool clearData = true)
        {
            _insertedAt = DateTime.Now;

            if (clearData) Data = null;
            HasTriggered = triggered;
        }

        public void SetImmediate()
        {
            _insertedAt = DateTime.Now.AddSeconds(-Trigger);
        }

        public GameTask Init()
        {
            Reset(false);
            return this;
        }

        //public bool IsEmpty()
        //{
        //    return this.Trigger == Empty.Trigger && this.Method == Empty.Method;
        //}
    }
}

