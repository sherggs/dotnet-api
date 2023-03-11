using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Net7.Data
{
    // Interface for AuthRepository
    public class AuthRepository : IAuthRepository
    {
         private readonly IConfiguration _configuration;
        private readonly DataContext _context;
       

        public AuthRepository(DataContext context, IConfiguration configuration)
        {
            _configuration = configuration;
           _context = context;
           
        }
        // Login
        public async Task<ServiceResponse<string>> Login(string username, string password)
        {
            var response = new ServiceResponse<string>();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower().Equals(username.ToLower()));
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
            }
            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Wrong password";
            }
            else
            {
                response.Data = CreateToken(user);
                
            }
            return response;
        }

        // Registering a new user
        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            var Response = new ServiceResponse<int>();
            if (await UserExists(user.Username)){
                Response.Success = false;
                Response.Message = "User already exists";
                return Response;
            }
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            Response.Data = user.Id;
            return Response;
        }

        //checking existence of username
        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(u => u.Username.ToLower().Equals(username.ToLower()))) 
            {
                return true;
            }
            return false;
        }

        // Password Verification with salt and hash
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512()) // using statement to dispose of the object
            {
                passwordSalt = hmac.Key; // Key is a property of HMACSHA256
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); // ComputeHash is a method of HMACSHA256
            }
            
          
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)) // using statement to dispose of the object
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); // ComputeHash is a method of HMACSHA256
                return computedHash.SequenceEqual(passwordHash); // SequenceEqual is a method of IEnumerable
            }
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var appSettingsToken = _configuration.GetSection("AppSettings:Token").Value;
            if(appSettingsToken is null)
            
                throw new Exception("AppSettings:Token is null");
            
           SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(appSettingsToken));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    
    }
}