using GymManagementBLL.ViewModels.TrainerViewModels;
using GymManagementSystemBLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymManagementPL.Controllers
{
    public class TrainerController : Controller
    {
        private readonly ITrainerService _trainerService;

        public TrainerController(ITrainerService trainerService)
        {
            _trainerService = trainerService;
        }
        public IActionResult Index()
        {
            var trainers = _trainerService.GetAllTrainers();
            return View(trainers);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateTrainer(CreateTrainerViewModel input)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("DataMissed", "Check Missing Data");
                return View("Create", input);
            }
            bool result = _trainerService.CreateTrainer(input);
            if (result)
                TempData["SuccessMessage"] = "Trainer Created Successfully!";
            else
                TempData["ErrorMessage"] = "Error Creating Trainer.";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var trainer = _trainerService.GetTrainerToUpdate(id);
            if (trainer == null)
            {
                TempData["ErrorMessage"] = "Trainer not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }
        [HttpPost]
        public IActionResult Edit([FromRoute] int id,TrainerToUpdateViewModel input)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("DataMissed", "Check Missing Data");
                return View(input);
            }
            bool result = _trainerService.UpdateTrainerDetails(input,id);
            if (result)
                TempData["SuccessMessage"] = "Trainer Updated Successfully!";
            else
                TempData["ErrorMessage"] = "Error Updating Trainer.";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Details(int id)
        {
            var trainer = _trainerService.GetTrainerDetails(id);
            if (trainer == null)
            {
                TempData["ErrorMessage"] = "Trainer not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }
        public IActionResult Delete(int id)
        {
            if(id <= 0)
            {
                TempData["ErrorMessage"] = "Id Of Trainer Can Not Be Zero Or Negative";
                return RedirectToAction(nameof(Index));
            }
            var trainer = _trainerService.GetTrainerDetails(id);
            if (trainer == null)
            {
                TempData["ErrorMessage"] = "Trainer not found.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.TrainerId = id;
            return View();
        }
        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            bool result = _trainerService.RemoveTrainer(id);
            if (result)
                TempData["SuccessMessage"] = "Trainer Deleted Successfully!";
            else
                TempData["ErrorMessage"] = "Error Deleting Trainer.";
            return RedirectToAction(nameof(Index));
        }  
    }
}
