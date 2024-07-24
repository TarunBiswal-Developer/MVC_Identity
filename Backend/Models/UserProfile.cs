using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class UserProfile
    {
        [Display(Name = "User Id")]
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        [Display(Name = "User Code")]
        public string UserCode { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
         public string Role { get; set; }
        public bool IsActive { get; set; }
     
        [Display(Name = "Roles")]
        public List<string> UserRoles { get; set; }
        public string Department { get; set; }
        public int? BatchId { get; set; }
        public int? EmployeeCategoryId { get; set; }
        public string EmployeeCategory { get; set; }
        public string Batch { get; set; }


        //My profile table

        public string OtherContactNo { get; set; }
        public string OtherEmailId { get; set; }
        public DateTime AdmissionYear { get; set; }
        public string FatherName { get; set; }
        public string FatherContact { get; set; }
        public string MotherName { get; set; }
        public string MotherContact { get; set; }
        public string LocalGuardian { get; set; }
        public string LocalGuardianContact { get; set; }
        public string ResidentialAddress { get; set; }
        public string HostelAddress { get; set; }
    }
    public class UserRegisterViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }

    }

    public class UserProfileEncrypt
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string UserCode { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Role { get; set; }
        public string IsActive { get; set; }
        public string EmployeeCategoryId { get; set; }
        public string BatchId { get; set; }
        public List<string> UserRoles { get; set; }
        public string Department { get; set; }
        //public List<DepartmentViewModel> UserDepartments { get; set; }


        //My profile table

        public string OtherContactNo { get; set; }
        public string OtherEmailId { get; set; }
        public string AdmissionYear { get; set; }
        public string FatherName { get; set; }
        public string FatherContact { get; set; }
        public string MotherName { get; set; }
        public string MotherContact { get; set; }
        public string LocalGuardian { get; set; }
        public string LocalGuardianContact { get; set; }
        public string ResidentialAddress { get; set; }
        public string HostelAddress { get; set; }
    }

    public class UserModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string UserCode { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }

    }


}