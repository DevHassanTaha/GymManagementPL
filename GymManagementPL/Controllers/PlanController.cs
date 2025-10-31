using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.PlanViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GymManagementPL.Controllers
{
    public class PlanController : Controller
    {
        private readonly IPlanService _planService;

        public PlanController(IPlanService planService)
        {
            _planService = planService;
        }
        public IActionResult Index()
        {
            var plans = _planService.GetAllPlans();
            return View(plans);
            
        }
        public IActionResult Details(int id) 
        { 
            if(id <= 0) 
            {
                TempData["ErrorMessage"] = "Id Of Plan Can Not B e0Or Negative";
                return RedirectToAction(nameof(Index));
            }
            var plan = _planService.GetPlanById(id);
            if (plan == null)
            {
                TempData["ErrorMessage"] = "Plan not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }
        public IActionResult Edit(int id) 
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Id Of Plan Can Not Be 0 Or Negative";
                return RedirectToAction(nameof(Index));
            }
            var plan = _planService.GetPlanToUpdate(id);
            if (plan == null)
            {
                TempData["ErrorMessage"] = "Plan Can Not Be Update";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }
        [HttpPost]
        public IActionResult Edit([FromRoute] int id, UpdatePlanViewModel input)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("WrongData", "Check Data validation");
                return View(input);
            }
            var result = _planService.UpdatePlan(id, input);
            if (result)
                TempData["SuccessMessage"] = "Plan updated successfully!";
            else
                TempData["ErrorMessage"] = "Failed to update plan.";

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult Activate(int id) 
        { 
            var result = _planService.Activate(id);
            if (result)
                TempData["SuccessMessage"] = "Plan Status Changed";
            else
                TempData["ErrorMessage"] = "Failed to Change Plan Status";

            return RedirectToAction(nameof(Index));
        }


    }
}
