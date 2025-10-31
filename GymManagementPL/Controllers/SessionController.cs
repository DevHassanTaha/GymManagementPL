using GymManagementBLL.Services.Classes;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.PlanViewModels;
using GymManagementBLL.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementPL.Controllers
{
    public class SessionController : Controller
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }
        public IActionResult Index()
        {
            var sessions = _sessionService.GetAllSessions();
            return View(sessions);
        }
        public IActionResult Create()
        {
            LoadCategoriesDropDown();
            LoadTrainersDropDown();
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateSessionViewModel input)
        {
            if (!ModelState.IsValid)
            {
                LoadCategoriesDropDown();
                LoadTrainersDropDown();
                return View(input);
            }
            var result = _sessionService.CreateSession(input);

            if (result)
            {
                TempData["SuccessMessage"] = "Session created successfully!";

                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Failed to create session. Please verify trainer and category exist.");
                LoadCategoriesDropDown();
                LoadTrainersDropDown();
                return View(input);
            }
        }
        public IActionResult Details(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Id Of Session Can Not B e0Or Negative";
                return RedirectToAction(nameof(Index));
            }
            var session = _sessionService.GetSessionById(id);
            if (session == null)
            {
                TempData["ErrorMessage"] = "Session not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(session);
        }
        public IActionResult Edit(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Id Of Session Can Not Be 0 Or Negative";
                return RedirectToAction(nameof(Index));
            }
            var session = _sessionService.GetSessionToUpdate(id);
            if (session == null)
            {
                TempData["ErrorMessage"] = "Session Can Not Be Update";
                return RedirectToAction(nameof(Index));
            }
            LoadTrainersDropDown();
            return View(session);
        }
        [HttpPost]
        public IActionResult Edit([FromRoute] int id, UpdateSessionViewModel input)
        {
            if (!ModelState.IsValid)
            {
                LoadTrainersDropDown();
                return View(input);
            }
            var result = _sessionService.UpdateSession(id, input);
            if (result)
                TempData["SuccessMessage"] = "Session updated successfully!";
            else
                TempData["ErrorMessage"] = "Failed to update Session.";

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete([FromRoute] int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Id Of Session Can Not Be Zero Or Negative";
                return RedirectToAction(nameof(Index));
            }
            var session = _sessionService.GetSessionById(id);
            if (session is null)
            {
                TempData["ErrorMessage"] = "Session Not Found";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SessionId = id;
            return View();
        }
        [HttpPost]
        public IActionResult DeleteConfirmed([FromForm] int id)
        {
            var result = _sessionService.RemoveSession(id);
            if (result)
                TempData["SuccessMessage"] = "Session Deleted Successfully!";
            else
                TempData["ErrorMessage"] = "Session Failed To Delete!";
            return RedirectToAction(nameof(Index));
        }
        #region Helper Methods
        public void LoadCategoriesDropDown()
        {
          var categories = _sessionService.GetCategoriesDropDown();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
        }
        public void LoadTrainersDropDown()
        {
            var trainers = _sessionService.GetTrainersDropDown();
            ViewBag.Trainers = new SelectList(trainers, "Id", "Name");
        }
        #endregion
    }
}
