using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;


namespace Net7.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {  
         
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        private int  GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Get a character by id
        public async Task<ServiceResponse<GetCharacterDtos>> GetCharacterById(int id) 
        {
            var serviceResponse = new ServiceResponse<GetCharacterDtos>();

            // Get the character from the list
            var dbCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id); 
            serviceResponse.Data = _mapper.Map<GetCharacterDtos>(dbCharacter);
            return serviceResponse;
        } 
        public async Task<ServiceResponse<List<GetCharacterDtos>>> GetAllCharacters()
        {
            // Create a new service response 
            var serviceResponse = new ServiceResponse<List<GetCharacterDtos>>(); 

            // Get all characters from the database
            var dbCharacters = await _context.Characters.Where(c => c.User!.Id == GetUserId()).ToListAsync(); 

            // Map the characters to a list of GetCharacterDtos
            serviceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDtos>(c)).ToList(); 
            return serviceResponse; 
        }
        public async Task<ServiceResponse<List<GetCharacterDtos>>> AddCharacter(AddCharacterDtos newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDtos>>();
            var character = _mapper.Map<Character>(newCharacter);
            character.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());



            _context.Characters.Add(character);
            await _context.SaveChangesAsync();
            serviceResponse.Data = await _context.Characters
                .Where(c => c.User!.Id == GetUserId())
                .Select(c => _mapper.Map<GetCharacterDtos>(c)).ToListAsync();
            return serviceResponse;
           
        }
        public async Task<ServiceResponse<GetCharacterDtos>> UpdateCharacter(UpdateCharacterDtos updatedCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDtos>();
            try
            {
                // Get the character from the database
                var character = 
                await _context.Characters.FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id); 
                if(character is null) 
                throw new Exception($"Character with id '{updatedCharacter.Id}'not found.");

                // Map the updated character to the character in the database

                _mapper.Map(updatedCharacter, character); 
    
                character.Name = updatedCharacter.Name;
                character.Class = updatedCharacter.Class;
                character.Defense = updatedCharacter.Defense;
                character.HitPoints = updatedCharacter.HitPoints;
                character.Intelligence = updatedCharacter.Intelligence;
                character.Strength = updatedCharacter.Strength;

                // Save the changes to the database

                await _context.SaveChangesAsync(); 
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
            // Create a new service response

            var serviceResponse = new ServiceResponse<List<GetCharacterDtos>>(); 
            try
            {
                var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
                if(character is null) 
                throw new Exception($"Character with id '{id}'not found.");

                // Remove the character from the database

                _context.Characters.Remove(character); 
                
                // Save the changes to the database
    
                await _context.SaveChangesAsync(); 

                // Map the characters to a list of GetCharacterDtos

                serviceResponse.Data = await _context.Characters.Select(c => _mapper.Map<GetCharacterDtos>(c)).ToListAsync(); 
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