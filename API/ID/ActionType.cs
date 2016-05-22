namespace OTA.ID
{
    /// <summary>
    /// Action type.
    /// </summary>
    /// <remarks>Typically used in tile actions. For example when reading the messages from a client.</remarks>
    public enum ActionType : byte
    {
        KillTile = 0,
        PlaceTile = 1,
        KillWall = 2,
        PlaceWall = 3,
        KillTile1 = 4,
        PlaceWire = 5,
        KillWire = 6,
        PoundTile = 7,
        PlaceActuator = 8,
        KillActuator = 9,
        PlaceWire2 = 10,
        KillWire2 = 11,
        PlaceWire3 = 12,
        KillWire3 = 13,
        SlopeTile = 14,
        FrameTrack = 15,
        PlaceWire4 = 16,
        KillWire4 = 17,
        PlaceLogicGate = 18,
        Actuate = 19,
        UNKNOWN_1 = 100,
        UNKNOWN_2 = 101,
    }
}