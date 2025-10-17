using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.PlanViewModels;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public bool Activate(int planId)
        {
            var plan = _unitOfWork.GetRepository<Plan>().GetById(planId);
            if (plan == null || HasActiveMemberships(planId))
                return false;
            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;
            _unitOfWork.GetRepository<Plan>().Update(plan);
            _unitOfWork.SaveChanges();
            return true;

        }

        public IEnumerable<PlanViewModel> GetAllPlans()
        {
            var plans = _unitOfWork.GetRepository<Plan>().GetAll();
            if (plans == null || !plans.Any())
                return [];
            return plans.Select(x => new PlanViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                DurationDays = x.DurationDays,
                IsActive = x.IsActive
            });
        }

        public PlanViewModel? GetPlanById(int planId)
        {
            var plan = _unitOfWork.GetRepository<Plan>().GetById(planId);
            if (plan == null)
                return null;
            return new PlanViewModel
            {
                Id = plan.Id,
                Name = plan.Name,
                Description = plan.Description,
                Price = plan.Price,
                DurationDays = plan.DurationDays,
                IsActive = plan.IsActive
            };
        }

        public UpdatePlanViewModel? GetPlanToUpdate(int planId)
        {
            var plan = _unitOfWork.GetRepository<Plan>().GetById(planId);
            if (plan == null || plan.IsActive == false)
                return null;
            return new UpdatePlanViewModel
            {
                PlanName = plan.Name,
                Description = plan.Description,
                Price = plan.Price,
                DurationDays = plan.DurationDays,
            };
        }

        public bool UpdatePlan(int id, UpdatePlanViewModel input)
        {
            try
            {
                var plan = _unitOfWork.GetRepository<Plan>().GetById(id);
                if (plan == null || HasActiveMemberships(id))
                    return false;
                plan.Name = input.PlanName;
                plan.Description = input.Description;
                plan.Price = input.Price;
                plan.DurationDays = input.DurationDays;

                _unitOfWork.GetRepository<Plan>().Update(plan);

                _unitOfWork.SaveChanges();

                return true;


            }
            catch (Exception)
            {
                throw;
            }
        }
        #region Helper Methods
        private bool HasActiveMemberships(int planId)
        {
            return _unitOfWork.GetRepository<Membership>()
                .GetAll(m => m.PlanId == planId && m.Status == "Active")
                .Any();
        }

        #endregion
    }
}
