using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Net7.Dtos.Weapon;

namespace Net7.Services.WeaponService
{
    public class WeaponService : IWeaponService
    {
        public Task<ServiceResponse<GetCharacterDtos>> AddWeapon(AddWeaponDto newWeapon)
        {
            throw new NotImplementedException();
        }
    }
}