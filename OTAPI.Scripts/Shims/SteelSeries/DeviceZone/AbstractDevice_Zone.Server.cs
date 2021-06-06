namespace SteelSeries.GameSense.DeviceZone
{
    public abstract class AbstractDevice_Zone
    {
        public string device { get; }

        public AbstractDevice_Zone(string device) { }

        protected string _device;
    }
}
