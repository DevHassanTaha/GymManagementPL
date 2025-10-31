﻿using GymManagementBLL.ViewModels.TrainerViewModels;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using GymManagementSystemBLL.Services.Interfaces;

namespace GymManagementSystemBLL.Services.Classes
{
	public class TrainerService : ITrainerService
	{
		private readonly IUnitOfWork _unitOfWork;

		public TrainerService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public bool CreateTrainer(CreateTrainerViewModel createdTrainer)
		{
			try
			{
				var Repo = _unitOfWork.GetRepository<Trainer>();

				if (IsEmailExists(createdTrainer.Email) || IsPhoneExists(createdTrainer.Phone)) return false;
				var Trainer = new Trainer()
				{
					Name = createdTrainer.Name,
					Email = createdTrainer.Email,
					Phone = createdTrainer.Phone,
					DateOfBirth = createdTrainer.DateOfBirth,
					Specialties = createdTrainer.Specialties,
					Gender = createdTrainer.Gender,
					Address = new Address()
					{
						BuildingNumber = createdTrainer.BuildingNumber,
						City = createdTrainer.City,
						Street = createdTrainer.Street,
					}
				};


				Repo.Add(Trainer);

				return _unitOfWork.SaveChanges() > 0;


			}
			catch (Exception)
			{
				return false;
			}
		}

		public IEnumerable<TrainerViewModel> GetAllTrainers()
		{
			var Trainers = _unitOfWork.GetRepository<Trainer>().GetAll();
			if (Trainers is null || !Trainers.Any()) return [];

			return Trainers.Select(X => new TrainerViewModel
			{
				Id = X.Id,
				Name = X.Name,
				Email = X.Email,
				Phone = X.Phone,
				Specialties = X.Specialties.ToString()
			});
		}

		public TrainerViewModel? GetTrainerDetails(int trainerId)
		{
			var Trainer = _unitOfWork.GetRepository<Trainer>().GetById(trainerId);
			if (Trainer is null) return null;


			return new TrainerViewModel
			{
				Email = Trainer.Email,
				Name = Trainer.Name,
				Phone = Trainer.Phone,
				Specialties = Trainer.Specialties.ToString()
			};
		}
		public TrainerToUpdateViewModel? GetTrainerToUpdate(int trainerId)
		{
			var Trainer = _unitOfWork.GetRepository<Trainer>().GetById(trainerId);
			if (Trainer is null) return null;

			return new TrainerToUpdateViewModel()
			{
				Name = Trainer.Name, // For Display 
				Email = Trainer.Email,
				Phone = Trainer.Phone,
				Street = Trainer.Address.Street,
				BuildingNumber = Trainer.Address.BuildingNumber,
				City = Trainer.Address.City,
				Specialties = Trainer.Specialties
			};
		}
		public bool RemoveTrainer(int trainerId)
		{
			var Repo = _unitOfWork.GetRepository<Trainer>();
			var TrainerToRemove = Repo.GetById(trainerId);
			if (TrainerToRemove is null || HasActiveSessions(trainerId)) return false;
			Repo.Delete(TrainerToRemove);
			return _unitOfWork.SaveChanges() > 0;
		}

		public bool UpdateTrainerDetails(TrainerToUpdateViewModel updatedTrainer, int trainerId)
		{
			var Repo = _unitOfWork.GetRepository<Trainer>();
			var TrainerToUpdate = Repo.GetById(trainerId);

			if (TrainerToUpdate is null || IsEmailExists(updatedTrainer.Email) || IsPhoneExists(updatedTrainer.Phone)) return false;

			TrainerToUpdate.Email = updatedTrainer.Email;
			TrainerToUpdate.Phone = updatedTrainer.Phone;
			TrainerToUpdate.Address.BuildingNumber = updatedTrainer.BuildingNumber;
			TrainerToUpdate.Address.Street = updatedTrainer.Street;
			TrainerToUpdate.Address.City = updatedTrainer.City;
			TrainerToUpdate.Specialties = updatedTrainer.Specialties;
			TrainerToUpdate.UpdatedAt = DateTime.Now;
			Repo.Update(TrainerToUpdate);
			return _unitOfWork.SaveChanges() > 0;
		}

		#region Helper Methods

		private bool IsEmailExists(string email)
		{
			var existing = _unitOfWork.GetRepository<Trainer>().GetAll(
				m => m.Email == email).Any();
			return existing;
		}

		private bool IsPhoneExists(string phone)
		{
			var existing = _unitOfWork.GetRepository<Trainer>().GetAll(
				m => m.Phone == phone).Any();
			return existing;
		}

		private bool HasActiveSessions(int Id)
		{
			var activeSessions = _unitOfWork.GetRepository<Session>().GetAll(
			   s => s.TrainerId == Id && s.StartDate > DateTime.Now).Any();
			return activeSessions;
		}
		#endregion
	}
}
