using AutoMapper;
using LegalGen.Domain.DTOs;
using LegalGen.Domain.Models;

namespace LegalGen.Domain.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<ResearchBook, ResearchBookDto>().ReverseMap();
            CreateMap<LegalInformationDto, LegalInformation>().ReverseMap();
        }
    }
}
