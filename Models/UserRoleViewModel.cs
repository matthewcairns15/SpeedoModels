using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SpeedoModels.Models
{
    /// <summary>
    /// View Model for User
    /// </summary>
    public class UserViewModel
    {
        [Key]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        [Display(Name = "Lockout End Date Utc")]
        public DateTime? LockoutEndDateUtc { get; set; }
        public int AccessFailedCount { get; set; }
        public string PhoneNumber { get; set; }
        public IEnumerable<UserRolesViewModel> Roles { get; set; }
    }
    /// <summary>
    /// View Model UserRoles
    /// </summary>
    public class UserRolesViewModel
    {
        [Key]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }
    /// <summary>
    /// View Model UserRole
    /// </summary>
    public class UserRoleViewModel
    {
        [Key]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }
    /// <summary>
    /// View Model for Role
    /// </summary>
    public class RoleViewModel
    {
        [Key]
        public string Id { get; set; }
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }
    /// <summary>
    /// View Model for UserAndRoles
    /// </summary>
    public class UserAndRolesViewModel
    {
        [Key]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        public List<UserRoleViewModel> UserRoles { get; set; }
    }
}