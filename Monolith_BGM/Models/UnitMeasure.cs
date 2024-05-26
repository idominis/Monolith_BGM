using System;
using System.Collections.Generic;

namespace Monolith_BGM.Models
{
    /// <summary>
    /// Unit of measure lookup table.
    /// </summary>
    public partial class UnitMeasure
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitMeasure"/> class.
        /// </summary>
        public UnitMeasure()
        {
            ProductSizeUnitMeasureCodeNavigations = new HashSet<Product>();
            ProductWeightUnitMeasureCodeNavigations = new HashSet<Product>();
        }

        /// <summary>
        /// Primary key.
        /// </summary>
        /// <value>
        /// The unit measure code.
        /// </value>
        public string UnitMeasureCode { get; set; } = null!;
        /// <summary>
        /// Unit of measure description.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; } = null!;
        /// <summary>
        /// Date and time the record was last updated.
        /// </summary>
        /// <value>
        /// The modified date.
        /// </value>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the product size unit measure code navigations.
        /// </summary>
        /// <value>
        /// The product size unit measure code navigations.
        /// </value>
        public virtual ICollection<Product> ProductSizeUnitMeasureCodeNavigations { get; set; }
        /// <summary>
        /// Gets or sets the product weight unit measure code navigations.
        /// </summary>
        /// <value>
        /// The product weight unit measure code navigations.
        /// </value>
        public virtual ICollection<Product> ProductWeightUnitMeasureCodeNavigations { get; set; }
    }
}
