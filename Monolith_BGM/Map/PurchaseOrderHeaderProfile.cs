using AutoMapper;
using Monolith_BGM.DataAccess.DTO;
using Monolith_BGM.Models;

public class PurchaseOrderHeaderProfile : Profile
{
    public PurchaseOrderHeaderProfile()
    {
        CreateMap<PurchaseOrderHeaderDto, PurchaseOrderHeader>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
