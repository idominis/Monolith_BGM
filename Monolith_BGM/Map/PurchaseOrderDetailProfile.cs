using AutoMapper;
using Monolith_BGM.DataAccess.DTO;
using Monolith_BGM.Models;

/// <summary>
/// Defines the mapping profile for PurchaseOrderDetail objects.
/// </summary>
public class PurchaseOrderDetailProfile : Profile
{
    // ********************************************************************************
    /// <summary>
    /// Defines the mapping profile for PurchaseOrderDetail objects.
    /// </summary>
    /// <returns></returns>
    // <created>,5/26/2024</created>
    // <changed>,5/26/2024</changed>
    // ********************************************************************************
    public PurchaseOrderDetailProfile()
    {
        CreateMap<PurchaseOrderDetailDto, PurchaseOrderDetail>()
            //.ForMember(dest => dest.PurchaseOrderDetailId, opt => opt.Ignore()) // Explicitly ignore
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
