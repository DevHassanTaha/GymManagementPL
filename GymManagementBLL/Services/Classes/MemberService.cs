using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.MemberViewModels;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MemberService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool CreateMember(CreateMemberViewModel model)
        {
            try
            {
                if(IsEmailExists(model.Email))
                    return false;
                if (IsPhoneExists(model.Phone))
                    return false;
                var member = new Member
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
                    HealthRecord = new HealthRecord
                    {
                        Height = model.HealthRecordViewModel.Height,
                        Weight = model.HealthRecordViewModel.Weight,
                        BloodType = model.HealthRecordViewModel.BloodType,
                        Note = model.HealthRecordViewModel.Note
                    }

                };
                _unitOfWork.GetRepository<Member>().Add(member);
                _unitOfWork.SaveChanges();
                return true;
            }
            catch 
            {
                return false;
            }
        }

        public IEnumerable<MemberViewModel> GetAllMembers()
        {
            var members = _unitOfWork.GetRepository<Member>().GetAll() ?? [];
            if (members == null || !members.Any())
                return [];
            var memberViewModels = members.Select(x => new MemberViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                Photo = x.Photo,
                Phone = x.Phone,
                DateOfBirth = x.DateOfBirth.ToShortDateString(),
                Gender = x.Gender.ToString(),

            });
            return memberViewModels;

        }

        public MemberViewModel? GetMemberDetails(int memberId)
        {
            var member = _unitOfWork.GetRepository<Member>().GetById(memberId);
            if (member == null)
                return null;
            var memberViewModel = new MemberViewModel
            {
                Id = member.Id,
                Name = member.Name,
                Email = member.Email,
                Photo = member.Photo,
                Phone = member.Phone,
                DateOfBirth = member.DateOfBirth.ToShortDateString(),
                Gender = member.Gender.ToString(),
                Address = FormatAddress(member.Address),


            };
            var activeMembership = _unitOfWork.GetRepository<Membership>()
                                    .GetAll(x => x.MemberId == memberId && x.Status == "Active")
                                    .FirstOrDefault();

            if (activeMembership != null)
            {
                var plan = _unitOfWork.GetRepository<Plan>().GetById(activeMembership.PlanId);
                memberViewModel.PlanName = plan?.Name;
                memberViewModel.MembershipStartDate = activeMembership.CreatedAt.ToShortDateString();
                memberViewModel.MembershipEndDate = activeMembership.EndDate.ToShortDateString();
            }
            return memberViewModel;

        }

        public HealthRecordViewModel? GetMemberHealthRecord(int memberId)
        {
            var memberHealthRecord = _unitOfWork.GetRepository<HealthRecord>().GetById(memberId);
            if (memberHealthRecord == null)
                return null;
            var healthRecordViewModel = new HealthRecordViewModel
            {
                Height = memberHealthRecord.Height,
                Weight = memberHealthRecord.Weight,
                BloodType = memberHealthRecord.BloodType,
                Note = memberHealthRecord.Note
            };
            return healthRecordViewModel;
        }

        public MemberToUpdateViewModel? GetMemberToUpdate(int memberId)
        {
            var member = _unitOfWork.GetRepository<Member>().GetById(memberId);
            if (member == null)
                return null;
            var memberToUpdateViewModel = new MemberToUpdateViewModel
            {
                Name = member.Name,
                Email = member.Email,
                Photo = member.Photo,
                Phone = member.Phone,
                BuildingNumber = member.Address.BuildingNumber,
                City = member.Address.City,
                Street = member.Address.Street

            };
            return memberToUpdateViewModel;
        }

        public bool RemoveMember(int memberId)
        {
            var member = _unitOfWork.GetRepository<Member>().GetById(memberId);
            if (member == null)
                return false;
            var activeBookings = _unitOfWork.GetRepository<Booking>().GetAll(x => x.MemberId == memberId && x.Session.StartDate > DateTime.UtcNow);

            if (activeBookings != null && activeBookings.Any())
                return false;

            var membership = _unitOfWork.GetRepository<Membership>().GetAll(x => x.MemberId == memberId);

            try
            {
                if (membership != null && membership.Any())
                {
                    foreach (var mem in membership)
                    {
                        _unitOfWork.GetRepository<Membership>().Delete(mem);
                    }
                }
                _unitOfWork.GetRepository<Member>().Delete(member);
                _unitOfWork.SaveChanges();
                return true;

            }
            catch(Exception)
            {
                throw;
            }
        }

        public bool UpdateMemberDetails(int memberId, MemberToUpdateViewModel model)
        {
            var member = _unitOfWork.GetRepository<Member>().GetById(memberId);
            if (member == null)
                return false;
            var emailExist = _unitOfWork.GetRepository<Member>()
                            .GetAll(x => x.Email == model.Email && x.Id != memberId);

            var phoneExist = _unitOfWork.GetRepository<Member>()
                             .GetAll(x => x.Phone == model.Phone && x.Id != memberId);
            if (emailExist.Any() || phoneExist.Any())
                return false;

            member.Name = model.Name;
            member.Email = model.Email;
            member.Phone = model.Phone;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.Address.City = model.City;
            member.Address.Street = model.Street;
            member.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Member>().Update(member);
            _unitOfWork.SaveChanges();
            return true;

        }
        #region Helper Methods
        private string FormatAddress(Address address)
        {
            if (address == null)
                return string.Empty;
            return $"{address.BuildingNumber}, {address.Street}, {address.City}";
        }

        private bool IsEmailExists(string email)
        {

            var existingMember = _unitOfWork.GetRepository<Member>().GetAll(x => x.Email == email);

            return existingMember is not null && existingMember.Any();
        }

        private bool IsPhoneExists(string phone)
        {

            var existingMember = _unitOfWork.GetRepository<Member>().GetAll(x => x.Phone == phone);

            return existingMember is not null && existingMember.Any();

        }
        #endregion

    }
}