using NDesk.Options;

namespace OTAPI.Patcher.Modification
{
    /// <summary>
    /// Defines the bare minimum requirements for a <typeparamref name="Modification"/> to run in a <typeparamref name="ModificationRunner"/>
    /// </summary>
    public interface IModification
    {
        /// <summary>
        /// Back reference to the global <typeparamref name="ModificationContext"/> that is shared across modifications.
        /// </summary>
        ModificationContext ModificationContext { get; set; }

        /// <summary>
        /// Determines if the current modification can be executed.
        /// </summary>
        /// <returns></returns>
        bool IsAvailable(OptionSet options);

        /// <summary>
        /// Run the external modification.
        /// </summary>
        /// <param name="options"></param>
        void Run(OptionSet options);
    }
}