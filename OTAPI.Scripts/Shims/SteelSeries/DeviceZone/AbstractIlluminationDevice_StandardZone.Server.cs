namespace SteelSeries.GameSense.DeviceZone
{
    public abstract class AbstractIlluminationDevice_StandardZone : AbstractIlluminationDevice_Zone
    {
        public string zone { get; }

        public AbstractIlluminationDevice_StandardZone(string device, string zone) : base(device) { }

        public override bool HasCustomZone() => false;

        protected string _zone;
    }
}
