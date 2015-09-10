using System;

namespace OTA.Data.Entity.Models
{
    /// <summary>
    /// API account role.
    /// </summary>
    public class APIAccountRole
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the account identifier.
        /// </summary>
        /// <value>The account identifier.</value>
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the claim type.
        /// </summary>
        /// <value>The type.</value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the claims value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the valid starting date
        /// </summary>
        /// <value>The date from.</value>
        public DateTime DateFrom { get; set; }

        /// <summary>
        /// Gets or sets the end of the valid date range
        /// </summary>
        /// <remarks>If set to NULL the role is indefinite</remarks>
        /// <value>The date to.</value>
        public DateTime? DateTo { get; set; }
    }
}

