using GymManagementDAL.Data.Contexts;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementDAL.Repositories.Classes
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        private readonly GymDbContext _context;

        public SessionRepository(GymDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Session> GetAllSessionsWithTranierAndCategory()
        {
            return _context.sessions.Include(s => s.Trainer).Include(s => s.Category).ToList();
        }

        public int GetCountOfBookedSlots(int sessionId)
        {
            return _context.bookings.Where(b => b.SessionId == sessionId).Count();
        }

        public Session GetSessionsWithTranierAndCategory(int sessionId)
        {
            return _context.sessions
                .Include(s => s.Trainer)
                .Include(s => s.Category)
                .FirstOrDefault(s => s.Id == sessionId);
        }
    }
}
