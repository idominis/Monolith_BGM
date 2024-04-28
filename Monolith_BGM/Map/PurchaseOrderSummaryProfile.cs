using AutoMapper;
using Monolith_BGM.DataAccess.DTO;
using Monolith_BGM.Models;

public class PurchaseOrderSummaryProfile : Profile
{
    public PurchaseOrderSummaryProfile()
    {
        CreateMap<VPurchaseOrderSummary, PurchaseOrderSummary>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
