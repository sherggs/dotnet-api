using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Net7.Data
{
    // Interface for AuthRepository
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
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
                response.Data = user.Id.ToString();
                response.Message = "Login successful";
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
    }
}