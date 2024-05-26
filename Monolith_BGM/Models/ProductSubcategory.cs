using System;
using System.Collections.Generic;

namespace Monolith_BGM.Models
{
    /// <summary>
    /// Product subcategories. See ProductCategory table.
    /// </summary>
    public partial class ProductSubcategory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductSubcategory"/> class.
        /// </summary>
        public ProductSubcategory()
        {
            Products = new HashSet<Product>();
        }

        /// <summary>
        /// Primary key for ProductSubcategory records.
        /// </summary>
        /// <value>
        /// The product subcategory identifier.
        /// </value>
        public int ProductSubcategoryId { get; set; }
        /// <summary>
        /// Product category identification number. Foreign key to ProductCategory.ProductCategoryID.
        /// </summary>
        /// <value>
        /// The product category identifier.
        /// </value>
        public int ProductCategoryId { get; set; }
        /// <summary>
        /// Subcategory description.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; } = null!;
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
        /// Gets or sets the product category.
        /// </summary>
        /// <value>
        /// The product category.
        /// </value>
        public virtual ProductCategory ProductCategory { get; set; } = null!;
        /// <summary>
        /// Gets or sets the products.
        /// </summary>
        /// <value>
        /// The products.
        /// </value>
        public virtual ICollection<Product> Products { get; set; }
    }
}
