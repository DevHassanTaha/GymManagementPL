using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.TrainerViewModels;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TrainerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool CreateTrainer(CreateTrainerViewModel model)
        {
            try
            {
                if (IsEmailExists(model.Email))
                    return false;
                if (IsPhoneExists(model.Phone))
                    return false;

                var trainer = new Trainer
                {
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    Address = new Address
                    {
                        BuildingNumber = model.BuildingNumber,
                        Street = model.Street,
                        City = model.City
                    },
                    Specialties = model.Specialties,
                    CreatedAt = DateTime.Now
                };

                _unitOfWork.GetRepository<Trainer>().Add(trainer);
                return true;
            }
            catch
            {
                return false;
            }

        }

        public IEnumerable<TrainerViewModel> GetAllTrainers()
        {
            var trainers = _unitOfWork.GetRepository<Trainer>().GetAll() ?? [];
            if (trainers == null || !trainers.Any())
                return [];

            var trainerViewModels = trainers.Select(x => new TrainerViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                Phone = x.Phone,
                Specialties = x.Specialties.ToString(),
            });

            return trainerViewModels;
        }

        public TrainerViewModel? GetTrainerDetails(int trainerId)
        {

            var trainer = _unitOfWork.GetRepository<Trainer>().GetById(trainerId);
            if (trainer == null)
                return null;

            var trainerViewModel = new TrainerViewModel
            {
                Id = trainer.Id,
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                DateOfBirth = trainer.DateOfBirth.ToShortDateString(),
                Gender = trainer.Gender.ToString(),
                Address = FormatAddress(trainer.Address),
                Specialties = trainer.Specialties.ToString(),
                HireDate = trainer.CreatedAt.ToShortDateString()
            };

            return trainerViewModel;
        }

        public TrainerToUpdateViewModel? GetTrainerToUpdate(int trainerId)
        {
            var trainer = _unitOfWork.GetRepository<Trainer>().GetById(trainerId);
            if (trainer == null)
                return null;

            var trainerToUpdateViewModel = new TrainerToUpdateViewModel
            {
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                BuildingNumber = trainer.Address.BuildingNumber,
                City = trainer.Address.City,
                Street = trainer.Address.Street,
                Specialties = trainer.Specialties
            };

            return trainerToUpdateViewModel;
        }

        public bool RemoveTrainer(int trainerId)
        {
            var trainer = _unitOfWork.GetRepository<Trainer>().GetById(trainerId);
            if (trainer == null)
                return false;

            var futureSessions = _unitOfWork.GetRepository<Session>()
                .GetAll(x => x.TrainerId == trainerId && x.StartDate > DateTime.UtcNow);
            if (futureSessions != null && futureSessions.Any())
                return false;
            try
            {
                _unitOfWork.GetRepository<Trainer>().Delete(trainer);
                            return true;
            }
            catch (Exception)
            {
                throw;
            }


        }

        public bool UpdateTrainerDetails(int trainerId, TrainerToUpdateViewModel model)
        {
            var trainer = _unitOfWork.GetRepository<Trainer>().GetById(trainerId);
            if (trainer == null)
                return false;


            if (IsEmailExists(model.Email))
                return false;
            if (IsPhoneExists(model.Phone))
                return false;

            trainer.Name = model.Name;
            trainer.Email = model.Email;
            trainer.Phone = model.Phone;
            trainer.Address.BuildingNumber = model.BuildingNumber;
            trainer.Address.City = model.City;
            trainer.Address.Street = model.Street;
            trainer.Specialties = model.Specialties;
            trainer.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Trainer>().Update(trainer);
            return true;
        }


        #region Helper Method
        private string FormatAddress(Address address)
        {
            if (address == null)
                return string.Empty;
            return $"{address.BuildingNumber}, {address.Street}, {address.City}";
        }
        private bool IsEmailExists(string email)
        {
            var existingTrainer = _unitOfWork.GetRepository<Trainer>().GetAll(x => x.Email == email);
            return existingTrainer is not null && existingTrainer.Any();
        }

        private bool IsPhoneExists(string phone)
        {
            var existingTrainer = _unitOfWork.GetRepository<Trainer>().GetAll(x => x.Phone == phone);
            return existingTrainer is not null && existingTrainer.Any();
        }
        #endregion
    }
}
