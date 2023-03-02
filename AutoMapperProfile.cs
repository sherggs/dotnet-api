using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace Net7
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character, GetCharacterDtos>();
            CreateMap<AddCharacterDtos, Character>();
            CreateMap<UpdateCharacterDtos, Character>();
        }
    }
}