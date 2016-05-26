using NDesk.Options;

namespace OTAPI.Patcher.Inject
{
    public interface IInjection
    {
        /// <summary>
        /// Back reference to the global InjectionRunner context that is shared across injections.
        /// </summary>
        InjectionContext InjectionContext { get; set; }

        /// <summary>
        /// Run the external injection.
        /// </summary>
        /// <param name="options"></param>
        void Inject(OptionSet options);
    }

    /// <summary>
    /// Defines the bare minimum requirements of the injection for it to run in an InjectionRunner.
    /// </summary>
    public abstract class Injection<TInjectionContext> : IInjection
        where TInjectionContext : InjectionContext
    {
        public InjectionContext InjectionContext
        {
            get { return this.Context; }
            set
            { this.Context = (TInjectionContext)value; }
        }


        /// <summary>
        /// Back reference to the global InjectionRunner context that is shared across injections.
        /// </summary>
        public TInjectionContext Context { get; internal set; }

        /// <summary>
        /// Run the external injection.
        /// </summary>
        /// <param name="options"></param>
        public abstract void Inject(OptionSet options);
    }

    /// <summary>
    /// Defines the bare minimum requirements of the injection for it to run in an InjectionRunner, using the default InjectionContext
    /// </summary>
    public abstract class Injection : Injection<InjectionContext> { }
}