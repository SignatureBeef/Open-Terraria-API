using System;

namespace OTA.Command
{
    /// <summary>
    /// Argument converter callback.
    /// </summary>
    public delegate bool ArgumentConverter<T>(ArgumentList list, int position, out T converted);
}

