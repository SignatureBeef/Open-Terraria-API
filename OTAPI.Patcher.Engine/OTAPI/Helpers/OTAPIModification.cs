using NDesk.Options;
using OTAPI.Patcher.Engine.Modification;

namespace OTAPI.Patcher.Engine.Modifications.Helpers
{
	/// <summary>
	/// In addition to IInjection, this class provides helper methods for OTAPI specific
	/// functionalities
	/// </summary>
	[Ordered]
	public abstract class OTAPIModification<TModificationContext> : IModification
		where TModificationContext : ModificationContext
	{
		public ModificationContext ModificationContext
		{
			get { return this.Context; }
			set
			{ this.Context = (TModificationContext)value; }
		}


		/// <summary>
		/// Back reference to the global TModificationContext context that is shared across modifications.
		/// </summary>
		public TModificationContext Context { get; internal set; }

		/// <summary>
		/// A description of the modification that is running
		/// </summary>
		public abstract string Description { get; }

		/// <summary>
		/// Determines if the current instance can run it's modifications.
		/// </summary>
		/// <returns></returns>
		public virtual bool IsAvailable(OptionSet options) => true;

		/// <summary>
		/// Run the external modification.
		/// </summary>
		/// <param name="options"></param>
		public abstract void Run(OptionSet options);

		
	}
}
