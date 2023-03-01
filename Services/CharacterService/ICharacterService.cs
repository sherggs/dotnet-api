using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Net7.Services.CharacterService
{
    public interface ICharacterService
    {
        Task<ServiceResponse<List<GetCharacterDtos>>> GetAllCharacters();
        Task<ServiceResponse<GetCharacterDtos>> GetCharacterById(int id);
        Task<ServiceResponse<List<GetCharacterDtos>>> AddCharacter(AddCharacterDtos newCharacter);        

    }
}