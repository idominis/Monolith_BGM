using AutoMapper;
using Monolith_BGM.DataAccess.DTO;
using Monolith_BGM.Models;

public class PurchaseOrderDetailProfile : Profile
{
    public PurchaseOrderDetailProfile()
    {
        CreateMap<PurchaseOrderDetailDto, PurchaseOrderDetail>()
            .ForMember(dest => dest.PurchaseOrderDetailId, opt => opt.Ignore()) // Explicitly ignore
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
