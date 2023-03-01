using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Console;

namespace Net7.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {  
         private static List<Character> characters = new List<Character> {
            new Character(),
            new Character { Id = 1, Name = "Mason", Class = RpgClass.Knight, HitPoints = 70}
        };
        public async Task<ServiceResponse<List<GetCharacterDtos>>> AddCharacter(AddCharacterDtos newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<Character>>();
            characters.Add(newCharacter);
            serviceResponse.Data = characters;
            WriteLine("Character added!");
            return serviceResponse;
           
        }

        public async Task<ServiceResponse<List<GetCharacterDtos>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<Character>>();
            serviceResponse.Data = characters;
            return serviceResponse; 
        }

        public async Task<ServiceResponse<GetCharacterDtos>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<Character>();
            var character = characters.FirstOrDefault(c => c.Id == id);
            serviceResponse.Data = character;
            return serviceResponse;
        }  
    }
}