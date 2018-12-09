using Lottery.Models;
using Lottery.ViewModel;
using AutoMapper;

namespace Lottery
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Aposta, ApostaVM>()
                 .ForMember(dest => dest.Combinations, opt => opt.MapFrom(src => string.Join('-', src.Combinations)));
        }
    }
}
