using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;


namespace Net7.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {  
         private static List<Character> characters = new List<Character> {
            new Character(),
            new Character { Id = 1, Name = "Mason", Class = RpgClass.Knight, HitPoints = 70},
            new Character { Id = 2, Name = "Ademola", Class = RpgClass.Mage, HitPoints = 100}
        };
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public CharacterService(IMapper mapper, DataContext context)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<GetCharacterDtos>> GetCharacterById(int id) // Get a character by id
        {
            var serviceResponse = new ServiceResponse<GetCharacterDtos>();
            var character = characters.FirstOrDefault(c => c.Id == id); // Get the character from the list
            serviceResponse.Data = _mapper.Map<GetCharacterDtos>(character);
            return serviceResponse;
        } 
            public async Task<ServiceResponse<List<GetCharacterDtos>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDtos>>(); // Create a new service response
            var dbCharacters = await _context.Characters.ToListAsync(); // Get all characters from the database
            serviceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDtos>(c)).ToList(); // Map the characters to a list of GetCharacterDtos
            return serviceResponse; 
        }
        public async Task<ServiceResponse<List<GetCharacterDtos>>> AddCharacter(AddCharacterDtos newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDtos>>();
            var character = _mapper.Map<Character>(newCharacter);
            character.Id = characters.Max(c => c.Id) + 1;
            character.HitPoints = 100;
            characters.Add(character);
            serviceResponse.Data = characters.Select(c => _mapper.Map<GetCharacterDtos>(c)).ToList();
            return serviceResponse;
           
        }
        public async Task<ServiceResponse<GetCharacterDtos>> UpdateCharacter(UpdateCharacterDtos updatedCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDtos>();
            try
            {
                var character = characters.FirstOrDefault(c => c.Id == updatedCharacter.Id);
                if(character is null) 
                throw new Exception($"Character with id '{updatedCharacter.Id}'not found.");

                _mapper.Map(updatedCharacter, character);
    
                character.Name = updatedCharacter.Name;
                character.Class = updatedCharacter.Class;
                character.Defense = updatedCharacter.Defense;
                character.HitPoints = updatedCharacter.HitPoints;
                character.Intelligence = updatedCharacter.Intelligence;
                character.Strength = updatedCharacter.Strength;
                serviceResponse.Data = _mapper.Map<GetCharacterDtos>(character);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDtos>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDtos>>();
            try
            {
                Character character = characters.First(c => c.Id == id);
                characters.Remove(character);
                serviceResponse.Data = characters.Select(c => _mapper.Map<GetCharacterDtos>(c)).ToList();
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
    }
}