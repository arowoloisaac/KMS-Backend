using Key_Management_System.Data;
using Key_Management_System.Models;

namespace Key_Management_System.Services.UserServices.TokenService
{
    public class TokenMemoryStorageService : ITokenStorageService
    {
        private readonly HashSet<Guid> _tokens;

        public TokenMemoryStorageService()
        {
            _tokens = new HashSet<Guid>();
        }

        public bool CheckIfTokenIsLoggedOut(Guid identifier)
        {
            return _tokens.Contains(identifier);
        }

        public void LogoutToken(Guid identifier)
        {
            _tokens.Add(identifier);
        }
    }

    public class TokenDbStorageService : ITokenStorageService
    {
        private readonly ApplicationDbContext _context;

        public TokenDbStorageService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool CheckIfTokenIsLoggedOut(Guid identifier)
        {
            return _context.LogoutTokens.Any(x => x.Identifier == identifier && !x.DeleteDate.HasValue);
        }

        public void LogoutToken(Guid identifier)
        {
            _context.LogoutTokens.Add(new LogoutToken
            {
                CreateDateTime = DateTime.UtcNow,
                Identifier = identifier
            });
            _context.SaveChanges();
        }
    }
}
