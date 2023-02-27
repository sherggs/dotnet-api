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
        public async Task<List<Character>> AddCharacter(Character newCharacter)
        {
            
           characters.Add(newCharacter);
           WriteLine("Character added!");
           return (characters);
           
        }

        public async Task<List<Character>> GetAllCharacters()
        {
              return characters;
        }

        public async Task<Character> GetCharacterById(int id)
        {
            var character = characters.FirstOrDefault(c => c.Id == id);
            if(character is not null)
            return character;
            throw new Exception("Character not found");
        }  
    }
}