using System.Collections.Generic;

namespace SteelSeries.GameSense
{
    public class Bind_Event
    {
        public Bind_Event(string gameName, string eventName, int minValue, int maxValue, EventIconId iconId, AbstractHandler[] handlers)
        {
        }

        public Bind_Event(string gameName, string eventName, string defaultDisplayName, Dictionary<string, string> localizedDisplayNames, int minValue, int maxValue, EventIconId iconId, AbstractHandler[] handlers)
        {
        }

        public string game;

        public string eventName;

        public int minValue;

        public int maxValue;

        public EventIconId iconId;

        public AbstractHandler[] handlers;

        public string defaultDisplayName;

        public Dictionary<string, string> localizedDisplayNames;
    }
}
