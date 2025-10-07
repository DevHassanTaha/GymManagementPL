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
    public class TrainerRepository : ITrainerRepository
    {
        private readonly GymDbContext _context;

        public TrainerRepository(GymDbContext context)
        {
            _context = context;
        }
        public int Add(Trainer trainer)
        {
            _context.Add(trainer);
            return _context.SaveChanges();
        }

        public int Delete(int id)
        {
            var trainer = GetById(id);
            if (trainer == null)
                return 0;
            _context.Remove(trainer);
            return _context.SaveChanges();
        }

        public IEnumerable<Trainer> GetAll() => _context.trainers.ToList();


        public Trainer? GetById(int id) => _context.trainers.Find(id);
        

        public int Update(Trainer trainer)
        {
            _context.Update(trainer);
            return _context.SaveChanges();
        }
    }
}
