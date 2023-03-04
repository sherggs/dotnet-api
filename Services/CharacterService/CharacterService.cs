using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;


namespace Net7.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {  
         
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
            var dbCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id); // Get the character from the list
            serviceResponse.Data = _mapper.Map<GetCharacterDtos>(dbCharacter);
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
            _context.Characters.Add(character);
            await _context.SaveChangesAsync();
            serviceResponse.Data = await _context.Characters.Select(c => _mapper.Map<GetCharacterDtos>(c)).ToListAsync();
            return serviceResponse;
           
        }
        public async Task<ServiceResponse<GetCharacterDtos>> UpdateCharacter(UpdateCharacterDtos updatedCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDtos>();
            try
            {
                var character = 
                await _context.Characters.FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id); // Get the character from the database
                if(character is null) 
                throw new Exception($"Character with id '{updatedCharacter.Id}'not found.");

                _mapper.Map(updatedCharacter, character); // Map the updated character to the character in the database
    
                character.Name = updatedCharacter.Name;
                character.Class = updatedCharacter.Class;
                character.Defense = updatedCharacter.Defense;
                character.HitPoints = updatedCharacter.HitPoints;
                character.Intelligence = updatedCharacter.Intelligence;
                character.Strength = updatedCharacter.Strength;

                await _context.SaveChangesAsync(); // Save the changes to the database
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
            var serviceResponse = new ServiceResponse<List<GetCharacterDtos>>(); // Create a new service response
            try
            {
                var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
                if(character is null) 
                throw new Exception($"Character with id '{id}'not found.");

                _context.Characters.Remove(character); // Remove the character from the database
                await _context.SaveChangesAsync(); // Save the changes to the database


                serviceResponse.Data = await _context.Characters.Select(c => _mapper.Map<GetCharacterDtos>(c)).ToListAsync(); // Map the characters to a list of GetCharacterDtos
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