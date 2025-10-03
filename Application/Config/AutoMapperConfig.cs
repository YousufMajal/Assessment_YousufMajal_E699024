using AutoMapper;

namespace Application.Config;

public class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        //CreateMap<Contact, ClientContactDTO>()
        //    .ForMember(dst => dst.ClientContactIdNo, opt => opt.MapFrom(src => src.san_idnumber ?? string.Empty))
        //    .ForMember(dst => dst.ContactEmailAdrress, opt => opt.MapFrom(src => src.eMailaddress1 ?? string.Empty))
        //    .ForMember(dst => dst.ContactCellNo, opt => opt.MapFrom(src => src.mobilephone ?? string.Empty))
        //    .ForMember(dst => dst.ContactTelNo, opt => opt.MapFrom(src => string.Empty))
        //    .ForMember(dst => dst.CRMClientContactKey, opt => opt.MapFrom(src => Guid.Parse(src.contactid)))
        //    .ForMember(dst => dst.ContactFirstName, opt => opt.MapFrom(src => src.firstname ?? string.Empty))
        //    .ForMember(dst => dst.ContactLastName, opt => opt.MapFrom(src => src.lastname ?? string.Empty))
        //    .ForMember(dst => dst.ContactFaxNo, opt => opt.MapFrom(src => src.fax ?? string.Empty))
        //    .ForMember(dst => dst.UserCode, opt => opt.MapFrom(src => string.Empty))
        //    .ForMember(dst => dst.PrimaryContact, opt => opt.MapFrom(src => src.san_isprimarycontact ? Constants.Yes : Constants.No))
        //    .ForMember(dst => dst.ClientContactStatus, opt => opt.MapFrom(src => src.statecode == (int)StateCode.Active ? "Active" : "Inactive"))
        //    .ForMember(dst => dst.ClientContactStatusChangeDate, opt => opt.MapFrom(src => src.modifiedon.ToLocalTime()))
        //    .ForMember(dst => dst.ClientContactStatusChangeReason, opt => opt.MapFrom(src => src.statecode == (int)StateCode.Active ? "Active" : "Inactive"));
    }
}