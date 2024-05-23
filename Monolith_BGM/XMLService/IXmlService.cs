using Monolith_BGM.DataAccess.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monolith_BGM.XMLService
{
    public interface IXmlService
    {
        //T LoadFromXml<T>(string filePath);
        T LoadFromXml<T>(string filePath) where T : class;
        void GenerateXMLFiles(List<PurchaseOrderSummary> summaries, DateTime? startDate = null, DateTime? endDate = null);

        List<int> ExtractPurchaseOrderIdsFromXml(string filePath);

        List<int> ExtractPurchaseOrderDetailIdsFromXml(string filePath);
    }
}
