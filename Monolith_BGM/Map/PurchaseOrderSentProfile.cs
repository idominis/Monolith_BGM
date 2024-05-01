using AutoMapper;
using Monolith_BGM.DataAccess.DTO;
using Monolith_BGM.Models;

public class PurchaseOrderSentProfile : Profile
{
    public PurchaseOrderSentProfile()
    {
        CreateMap<PurchaseOrdersProcessedSent, PurchaseOrderSentDto>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
