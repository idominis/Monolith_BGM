using System;
using System.Collections.Generic;

namespace Monolith_BGM.Models
{
    /// <summary>
    /// Product model classification.
    /// </summary>
    public partial class ProductModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductModel"/> class.
        /// </summary>
        public ProductModel()
        {
            Products = new HashSet<Product>();
        }

        /// <summary>
        /// Primary key for ProductModel records.
        /// </summary>
        /// <value>
        /// The product model identifier.
        /// </value>
        public int ProductModelId { get; set; }
        /// <summary>
        /// Product model description.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; } = null!;
        /// <summary>
        /// Detailed product catalog information in xml format.
        /// </summary>
        /// <value>
        /// The catalog description.
        /// </value>
        public string? CatalogDescription { get; set; }
        /// <summary>
        /// Manufacturing instructions in xml format.
        /// </summary>
        /// <value>
        /// The instructions.
        /// </value>
        public string? Instructions { get; set; }
        /// <summary>
        /// ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.
        /// </summary>
        /// <value>
        /// The rowguid.
        /// </value>
        public Guid Rowguid { get; set; }
        /// <summary>
        /// Date and time the record was last updated.
        /// </summary>
        /// <value>
        /// The modified date.
        /// </value>
        public DateTime ModifiedDate { get; set; }


        /// <summary>
        /// Gets or sets the products.
        /// </summary>
        /// <value>
        /// The products.
        /// </value>
        public virtual ICollection<Product> Products { get; set; }
    }
}
