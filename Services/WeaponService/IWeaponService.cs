using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Net7.Dtos.Weapon;

namespace Net7.Services.WeaponService
{
    public interface IWeaponService
    {
        // Task<ServiceResponse<List<GetWeaponDtos>>> GetAllWeapons();
        // Task<ServiceResponse<GetWeaponDtos>> GetWeaponById(int id);
        // Task<ServiceResponse<List<GetWeaponDtos>>> AddWeapon(AddWeaponDtos newWeapon);
        // Task<ServiceResponse<GetWeaponDtos>> UpdateWeapon(UpdateWeaponDtos updatedWeapon);
        // Task<ServiceResponse<List<GetWeaponDtos>>> DeleteWeapon(int id);

        Task<ServiceResponse<GetCharacterDtos>> AddWeapon(AddWeaponDto newWeapon);  
        
        
        
    
    
    }
}