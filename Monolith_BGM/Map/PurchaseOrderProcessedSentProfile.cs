using AutoMapper;
using Monolith_BGM.DataAccess.DTO;
using Monolith_BGM.Models;

public class PurchaseOrderProcessedSentProfile : Profile
{
    public PurchaseOrderProcessedSentProfile()
    {
        CreateMap<PurchaseOrdersProcessedSent, PurchaseOrderSentDto>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
