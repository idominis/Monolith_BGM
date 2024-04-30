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
        T LoadFromXml<T>(string filePath);
        void GenerateXMLFiles(List<PurchaseOrderSummary> summaries, DateTime? startDate = null, DateTime? endDate = null);
    }
}
