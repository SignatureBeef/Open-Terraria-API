namespace SteelSeries.GameSense.DeviceZone
{
    public abstract class AbstractIlluminationDevice_Zone : AbstractDevice_Zone
    {
        public AbstractIlluminationDevice_Zone(string device) : base(device) { }

        public abstract bool HasCustomZone();
    }
}
