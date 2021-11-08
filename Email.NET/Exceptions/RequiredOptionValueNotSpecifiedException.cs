namespace Email.NET.Exceptions
{
    using System;

    /// <summary>
    /// exception thrown when a required exception value is not specified
    /// </summary>
    [Serializable]
    public class RequiredOptionValueNotSpecifiedException : Exception
    {
        /// <inheritdoc/>
        public RequiredOptionValueNotSpecifiedException(Type optionsType, string optionsName, string message) : base(message)
        {
            OptionsType = optionsType;
            OptionsName = optionsName;
        }

        /// <inheritdoc/>
        protected RequiredOptionValueNotSpecifiedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        /// <summary>
        /// the option object type
        /// </summary>
        public Type OptionsType { get; }

        /// <summary>
        /// option parameter name
        /// </summary>
        public string OptionsName { get; }
    }
}
