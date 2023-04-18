using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Net7.Dtos.Weapon;

namespace Net7.Services.WeaponService
{
    public class WeaponService : IWeaponService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WeaponService(DataContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ServiceResponse<GetCharacterDtos>> AddWeapon(AddWeaponDto newWeapon)
        {
            var response = new ServiceResponse<GetCharacterDtos>();
            try
            {
                var character = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == newWeapon.CharacterId &&
                    c.User!.Id == int.Parse(_httpContextAccessor.HttpContext!.User
                    .FindFirstValue(ClaimTypes.NameIdentifier)!));


                if (character == null)
                {
                    response.Success = false;
                    response.Message = "Character not found.";
                    return response;
                }

                var weapon = new Weapon
                {
                    Name = newWeapon.Name,
                    Damage = newWeapon.Damage,
                    Character = character
                };
            
            _context.Weapons.Add(weapon);
            await _context.SaveChangesAsync();
            response.Data = _mapper.Map<GetCharacterDtos>(character);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
    }
}
}