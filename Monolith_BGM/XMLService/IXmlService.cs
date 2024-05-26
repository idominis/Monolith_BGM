using Monolith_BGM.DataAccess.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monolith_BGM.XMLService
{
    /// <summary>
    /// 
    /// </summary>
    public interface IXmlService
    {
        //T LoadFromXml<T>(string filePath);
        /// <summary>
        /// Loads from XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        T LoadFromXml<T>(string filePath) where T : class;
        /// <summary>
        /// Generates the XML files.
        /// </summary>
        /// <param name="summaries">The summaries.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        void GenerateXMLFiles(List<PurchaseOrderSummary> summaries, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Extracts the purchase order ids from XML.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        List<int> ExtractPurchaseOrderIdsFromXml(string filePath);

        /// <summary>
        /// Extracts the purchase order detail ids from XML.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        List<int> ExtractPurchaseOrderDetailIdsFromXml(string filePath);
    }
}
