using SpeedoModels.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SpeedoModels.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        // Controllers
        /// <summary>
        /// GET: /Admin/
        /// Sets the items to equal the UserViewModel equivalents
        /// </summary>
        /// <returns>
        /// View(userViewModelList)
        /// </returns>

        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {

            List<UserViewModel> userViewModelList = new List<UserViewModel>();
            var result = UserManager.Users.OrderBy(x => x.UserName).ToList();

            foreach (var item in result)
            {
                //Creates a new userViewModel
                UserViewModel userViewModel = new UserViewModel();
                userViewModel.FirstName = item.FirstName;
                userViewModel.LastName = item.LastName;
                userViewModel.UserName = item.UserName;
                userViewModel.Email = item.Email;
                userViewModel.LockoutEndDateUtc = item.LockoutEndDateUtc;

                userViewModelList.Add(userViewModel);
            }

            return View(userViewModelList);

        }


        // Users *****************************
        /// <summary>
        /// GET: /Admin/Edit/Create
        /// </summary>
        /// <returns>
        /// View(userViewModel)
        /// </returns>

        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            UserViewModel userViewModel = new UserViewModel();

            ViewBag.Roles = GetAllRolesAsSelectList();

            return View(userViewModel);
        }

        /// <summary>
        /// PUT: /Admin/Create
        /// </summary>
        /// <param name="paramUserViewModel">The parameter user view model.</param>
        /// <returns>
        /// Redirect("~/Admin"), View(paramUserViewModel), View("Create")
        /// </returns>
        /// <exception cref="Exception">
        /// No First Name
        /// or
        /// No Last Name
        /// or
        /// No Email
        /// or
        /// No Password
        /// </exception>
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Create(UserViewModel paramUserViewModel)
        {
            try
            {
                if (paramUserViewModel == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var FirstName = paramUserViewModel.FirstName.Trim();
                var LastName = paramUserViewModel.LastName.Trim();
                var Email = paramUserViewModel.Email.Trim();
                var UserName = paramUserViewModel.Email.Trim();
                var Password = paramUserViewModel.Password.Trim();


                if (FirstName == "")
                {
                    throw new Exception("No First Name");
                }
                if (LastName == "")
                {
                    throw new Exception("No Last Name");
                }
                if (Email == "")
                {
                    throw new Exception("No Email");
                }

                if (Password == "")
                {
                    throw new Exception("No Password");
                }

                // UserName is LowerCase of the Email
                UserName = Email.ToLower();

                // Create user

                var objNewAdminUser = new ApplicationUser { UserName = UserName, Email = Email, FirstName = FirstName, LastName = LastName };
                var AdminUserCreateResult = UserManager.Create(objNewAdminUser, Password);

                if (AdminUserCreateResult.Succeeded == true)
                {
                    string strNewRole = Convert.ToString(Request.Form["Roles"]);

                    if (strNewRole != "0")
                    {
                        // Put user in role
                        UserManager.AddToRole(objNewAdminUser.Id, strNewRole);
                    }

                    return Redirect("~/Admin");
                }
                else
                {
                    ViewBag.Roles = GetAllRolesAsSelectList();
                    ModelState.AddModelError(string.Empty,
                        "Error: Failed to create the user. Check password requirements.");
                    return View(paramUserViewModel);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Roles = GetAllRolesAsSelectList();
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("Create");
            }
        }

        /// <summary>
        /// GET: /Admin/Edit/TestUser
        /// </summary>
        /// <param name="UserName">Name of the user.</param>
        /// <returns>
        /// new HttpStatusCodeResult(HttpStatusCode.BadRequest),HttpNotFound(),View(objUserViewModel)
        /// </returns>
        [Authorize(Roles = "Administrator")]

        public ActionResult EditUser(string UserName)
        {
            if (UserName == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserViewModel objUserViewModel = GetUser(UserName);
            if (objUserViewModel == null)
            {
                return HttpNotFound();
            }
            return View(objUserViewModel);
        }

        /// <summary>
        /// PUT: /Admin/EditUser
        /// </summary>
        /// <param name="paramUserViewModel">The parameter user view model.</param>
        /// <returns>
        /// new HttpStatusCodeResult(HttpStatusCode.BadRequest,HttpNotFound(),Redirect("~/Admin"), View("EditUser", GetUser(paramUserViewModel.UserName))
        /// </returns>
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult EditUser(UserViewModel paramUserViewModel)
        {
            try
            {
                if (paramUserViewModel == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                UserViewModel objUserViewModel = UpdateUserViewModel(paramUserViewModel);

                if (objUserViewModel == null)
                {
                    return HttpNotFound();
                }

                return Redirect("~/Admin");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("EditUser", GetUser(paramUserViewModel.UserName));
            }
        }

        /// <summary>
        /// Deletes an user if the UserName doesnt equal null
        /// </summary>
        /// <param name="UserName">Name of the user.</param>
        /// <returns>
        /// new HttpStatusCodeResult(HttpStatusCode.BadRequest), View("EditUser"),HttpNotFound(), View("EditUser", GetUser(UserName))
        /// </returns>
        // DELETE: /Admin/DeleteUser
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteUser(string UserName)
        {
            try
            {
                if (UserName == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (UserName.ToLower() == this.User.Identity.Name.ToLower())
                {
                    ModelState.AddModelError(
                        string.Empty, "Error: Cannot delete the current user");

                    return View("EditUser");
                }

                UserViewModel objUserViewModel = GetUser(UserName);

                if (objUserViewModel == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    DeleteUser(objUserViewModel);
                }

                return Redirect("~/Admin");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("EditUser", GetUser(UserName));
            }
        }

        /// <summary>
        /// checks if the username if null, gets the user and roles
        /// </summary>
        /// <param name="UserName">Name of the user.</param>
        /// <returns>
        /// new HttpStatusCodeResult(HttpStatusCode.BadRequest),return HttpNotFound(),View(objUserAndRolesViewModel)
        /// </returns>
        // GET: /Admin/EditRoles/TestUser 
        [Authorize(Roles = "Administrator")]
        public ActionResult EditRoles(string UserName)
        {
            if (UserName == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UserName = UserName.ToLower();

            // Check that we have an actual user
            UserViewModel objUserViewModel = GetUser(UserName);

            if (objUserViewModel == null)
            {
                return HttpNotFound();
            }

            UserAndRolesViewModel objUserAndRolesViewModel =
                GetUserAndRoles(UserName);

            return View(objUserAndRolesViewModel);
        }

        // PUT: /Admin/EditRoles/TestUser 
        /// <summary>
        /// Allows the admin to edit the current roles
        /// </summary>
        /// <param name="paramUserAndRolesViewModel">The parameter user and roles view model.</param>
        /// <returns>
        /// new HttpStatusCodeResult(HttpStatusCode.BadRequest),View(objUserAndRolesViewModel),View("EditRoles")
        /// </returns>
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult EditRoles(UserAndRolesViewModel paramUserAndRolesViewModel)
        {
            try
            {
                if (paramUserAndRolesViewModel == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                string UserName = paramUserAndRolesViewModel.UserName;
                string strNewRole = Convert.ToString(Request.Form["AddRole"]);

                if (strNewRole != "No Roles Found")
                {
                    // Go get the User
                    ApplicationUser user = UserManager.FindByName(UserName);

                    // Put user in role
                    UserManager.AddToRole(user.Id, strNewRole);
                }

                ViewBag.AddRole = new SelectList(RolesUserIsNotIn(UserName));

                UserAndRolesViewModel objUserAndRolesViewModel =
                    GetUserAndRoles(UserName);

                return View(objUserAndRolesViewModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("EditRoles");
            }
        }


        // DELETE: /Admin/DeleteRole?UserName="TestUser&RoleName=Administrator
        /// <summary>
        /// Allows the admin to delete an existing role
        /// </summary>
        /// <param name="UserName">Name of the user.</param>
        /// <param name="RoleName">Name of the role.</param>
        /// <returns>
        /// new HttpStatusCodeResult(HttpStatusCode.BadRequest),HttpNotFound(),RedirectToAction("EditRoles", new { UserName = UserName }),View("EditRoles", objUserAndRolesViewModel)
        /// </returns>
        [Authorize(Roles = "Administrator")]

        public ActionResult DeleteRole(string UserName, string RoleName)
        {
            try
            {
                if ((UserName == null) || (RoleName == null))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                UserName = UserName.ToLower();

                // Check that we have an actual user
                UserViewModel objExpandedUserDTO = GetUser(UserName);

                if (objExpandedUserDTO == null)
                {
                    return HttpNotFound();
                }

                if (UserName.ToLower() ==
                    this.User.Identity.Name.ToLower() && RoleName == "Administrator")
                {
                    ModelState.AddModelError(string.Empty,
                        "Error: Cannot delete Administrator Role for the current user");
                }

                // Go get the User
                ApplicationUser user = UserManager.FindByName(UserName);
                // Remove User from role
                UserManager.RemoveFromRoles(user.Id, RoleName);
                UserManager.Update(user);

                ViewBag.AddRole = new SelectList(RolesUserIsNotIn(UserName));

                return RedirectToAction("EditRoles", new { UserName = UserName });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);

                ViewBag.AddRole = new SelectList(RolesUserIsNotIn(UserName));

                UserAndRolesViewModel objUserAndRolesViewModel =
                    GetUserAndRoles(UserName);

                return View("EditRoles", objUserAndRolesViewModel);
            }
        }


        // Roles *****************************
        /// <summary>
        /// Gets all the roles and displays them in a list
        /// </summary>
        /// <returns>
        /// View(colRoleViewModel)
        /// </returns>
        // GET: /Admin/ViewAllRoles
        [Authorize(Roles = "Administrator")]

        public ActionResult ViewAllRoles()
        {
            var roleManager =
                new RoleManager<IdentityRole>
                (
                    new RoleStore<IdentityRole>(new ApplicationDbContext())
                    );

            //queries the roleManager.Roles to display all roles
            List<RoleViewModel> colRoleViewModel = (from objRole in roleManager.Roles
                                                    select new RoleViewModel
                                                    {
                                                        Id = objRole.Id,
                                                        RoleName = objRole.Name
                                                    }).ToList();

            return View(colRoleViewModel);
        }

        /// <summary>
        /// Adds a role created by the admin to the roles lists
        /// </summary>
        /// <returns>
        /// View(objRoleViewModel)
        /// </returns>

        // GET: /Admin/AddRole
        [Authorize(Roles = "Administrator")]

        public ActionResult AddRole()
        {
            RoleViewModel objRoleViewModel = new RoleViewModel();

            return View(objRoleViewModel);
        }


        /// <summary>
        /// Adds a role that the admin has created if the role doesnt trigger an error
        /// </summary>
        /// <param name="paramRoleViewModel">The parameter role view model.</param>
        /// <returns>
        /// new HttpStatusCodeResult(HttpStatusCode.BadRequest),new Exception("No RoleName"), Redirect("~/Admin/ViewAllRoles"), View("AddRole")
        /// </returns>
        /// <exception cref="Exception">No RoleName</exception>

        // PUT: /Admin/AddRole

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult AddRole(RoleViewModel paramRoleViewModel)
        {
            try
            {
                if (paramRoleViewModel == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var RoleName = paramRoleViewModel.RoleName.Trim();

                if (RoleName == "")
                {
                    throw new Exception("No RoleName");
                }

                // Create Role
                var roleManager =
                    new RoleManager<IdentityRole>(
                        new RoleStore<IdentityRole>(new ApplicationDbContext())
                        );

                if (!roleManager.RoleExists(RoleName))
                {
                    roleManager.Create(new IdentityRole(RoleName));
                }

                return Redirect("~/Admin/ViewAllRoles");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("AddRole");
            }
        }

        /// <summary>
        /// Deletes an existing role if there are no users apart of the role
        /// </summary>
        /// <param name="RoleName">Name of the role.</param>
        /// <returns>
        /// new HttpStatusCodeResult(HttpStatusCode.BadRequest),new Exception(String.Format("Cannot delete {0} Role.", RoleName)),View("ViewAllRoles", colRoleDTO),View("ViewAllRoles", colRoleViewModel)
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        // DELETE: /Admin/DeleteUserRole?RoleName=TestRole
        [Authorize(Roles = "Administrator")]

        public ActionResult DeleteUserRole(string RoleName)
        {
            try
            {
                if (RoleName == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (RoleName.ToLower() == "administrator")
                {
                    throw new Exception(String.Format("Cannot delete {0} Role.", RoleName));
                }

                var roleManager =
                    new RoleManager<IdentityRole>(
                        new RoleStore<IdentityRole>(new ApplicationDbContext()));

                var UsersInRole = roleManager.FindByName(RoleName).Users.Count();
                if (UsersInRole > 0)
                {
                    throw new Exception(
                        String.Format(
                            "Canot delete {0} Role because it still has users.",
                            RoleName)
                            );
                }
                //Queries the roleManager.Roles to find the role that is to be deleted
                var objRoleToDelete = (from objRole in roleManager.Roles
                                       where objRole.Name == RoleName
                                       select objRole).FirstOrDefault();
                if (objRoleToDelete != null)
                {
                    roleManager.Delete(objRoleToDelete);
                }
                else
                {
                    throw new Exception(
                        String.Format(
                            "Canot delete {0} Role does not exist.",
                            RoleName)
                            );
                }
                //queries the roleManager.Roles to display all roles


                List<RoleViewModel> colRoleDTO = (from objRole in roleManager.Roles
                                                  select new RoleViewModel
                                                  {
                                                      Id = objRole.Id,
                                                      RoleName = objRole.Name
                                                  }).ToList();

                return View("ViewAllRoles", colRoleDTO);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);

                var roleManager =
                    new RoleManager<IdentityRole>(
                        new RoleStore<IdentityRole>(new ApplicationDbContext()));

                List<RoleViewModel> colRoleViewModel = (from objRole in roleManager.Roles
                                                        select new RoleViewModel
                                                        {
                                                            Id = objRole.Id,
                                                            RoleName = objRole.Name
                                                        }).ToList();

                return View("ViewAllRoles", colRoleViewModel);
            }
        }


        // Utility

        /// <summary>
        /// Gets the user manager.
        /// </summary>
        /// <value>
        /// The user manager.
        /// </value>
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ??
                    HttpContext.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }



        /// <summary>
        /// Gets the role manager.
        /// </summary>
        /// <value>
        /// The role manager.
        /// </value>
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ??
                    HttpContext.GetOwinContext()
                    .GetUserManager<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }


        /// <summary>
        /// gets the roles as a select list
        /// </summary>
        /// <returns>
        /// SelectRoleListItems
        /// </returns>
        private List<SelectListItem> GetAllRolesAsSelectList()
        {
            List<SelectListItem> SelectRoleListItems = new List<SelectListItem>();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));

            var colRoleSelectList = roleManager.Roles.OrderBy(x => x.Name).ToList();

            SelectRoleListItems.Add(
                new SelectListItem
                {
                    Text = "Select",
                    Value = "0"
                });

            foreach (var item in colRoleSelectList)
            {
                SelectRoleListItems.Add(
                    new SelectListItem
                    {
                        Text = item.Name.ToString(),
                        Value = item.Name.ToString()
                    });
            }

            return SelectRoleListItems;
        }



        /// <summary>
        /// User View Model for GetUser
        /// </summary>
        /// <param name="paramUserName">Name of the parameter user.</param>
        /// <returns>
        /// objUserViewModel
        /// </returns>
        /// <exception cref="Exception">Could not find the User</exception>
        private UserViewModel GetUser(string paramUserName)
        {
            UserViewModel objUserViewModel = new UserViewModel();

            var result = UserManager.FindByName(paramUserName);

            // If we could not find the user, throw an exception
            if (result == null) throw new Exception("Could not find the User");
            objUserViewModel.FirstName = result.FirstName;
            objUserViewModel.LastName = result.LastName;
            objUserViewModel.UserName = result.UserName;
            objUserViewModel.Email = result.Email;
            objUserViewModel.LockoutEndDateUtc = result.LockoutEndDateUtc;
            objUserViewModel.AccessFailedCount = result.AccessFailedCount;
            objUserViewModel.PhoneNumber = result.PhoneNumber;

            return objUserViewModel;
        }

        /// <summary>
        /// User View Model for updateUserViewModel
        /// </summary>
        /// <param name="paramUserViewModel">The parameter user view model.</param>
        /// <returns>
        /// paramUserViewModel
        /// </returns>
        /// <exception cref="Exception">
        /// Could not find the User
        /// or
        /// </exception>

        private UserViewModel UpdateUserViewModel(UserViewModel paramUserViewModel)
        {
            ApplicationUser result =
                UserManager.FindByName(paramUserViewModel.UserName);

            // If we could not find the user, throw an exception
            if (result == null)
            {
                throw new Exception("Could not find the User");
            }

            result.Email = paramUserViewModel.Email;

            // Lets check if the account needs to be unlocked
            if (UserManager.IsLockedOut(result.Id))
            {
                // Unlock user
                UserManager.ResetAccessFailedCountAsync(result.Id);
            }

            UserManager.Update(result);

            // Was a password sent across?
            if (!string.IsNullOrEmpty(paramUserViewModel.Password))
            {
                // Remove current password
                var removePassword = UserManager.RemovePassword(result.Id);
                if (removePassword.Succeeded)
                {
                    // Add new password
                    var AddPassword =
                        UserManager.AddPassword(
                            result.Id,
                            paramUserViewModel.Password
                            );

                    if (AddPassword.Errors.Count() > 0)
                    {
                        throw new Exception(AddPassword.Errors.FirstOrDefault());
                    }
                }
            }

            return paramUserViewModel;
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="paramUserViewModel">The parameter user view model.</param>
        /// <exception cref="Exception">Could not find the User</exception>

        private void DeleteUser(UserViewModel paramUserViewModel)
        {
            ApplicationUser user =
                UserManager.FindByName(paramUserViewModel.UserName);

            // If we could not find the user, throw an exception
            if (user == null)
            {
                throw new Exception("Could not find the User");
            }

            UserManager.RemoveFromRoles(user.Id, UserManager.GetRoles(user.Id).ToArray());
            UserManager.Update(user);
            UserManager.Delete(user);
        }

        /// <summary>
        /// User And Roles View Model for GetUserAndRoles
        /// </summary>
        /// <param name="UserName">Name of the user.</param>
        /// <returns>
        /// objUserAndRolesDTO
        /// </returns>

        private UserAndRolesViewModel GetUserAndRoles(string UserName)
        {
            // Go get the User
            ApplicationUser user = UserManager.FindByName(UserName);

            List<UserRoleViewModel> objUserRoleViewModel =
                (from objRole in UserManager.GetRoles(user.Id)
                 select new UserRoleViewModel
                 {
                     RoleName = objRole,
                     UserName = UserName
                 }).ToList();

            if (objUserRoleViewModel.Count() == 0)
            {
                objUserRoleViewModel.Add(new UserRoleViewModel { RoleName = "No Roles Found" });
            }

            ViewBag.AddRole = new SelectList(RolesUserIsNotIn(UserName));

            // Create UserRolesAndPermissionsDTO
            UserAndRolesViewModel objUserAndRolesDTO =
                new UserAndRolesViewModel();
            objUserAndRolesDTO.UserName = UserName;
            objUserAndRolesDTO.UserRoles = objUserRoleViewModel;
            return objUserAndRolesDTO;
        }

        /// <summary>
        /// creates a list for user is not in
        /// </summary>
        /// <param name="UserName">Name of the user.</param>
        /// <returns>
        /// colRolesUserInNotIn
        /// </returns>
        /// <exception cref="Exception">Could not find the User</exception>

        private List<string> RolesUserIsNotIn(string UserName)
        {
            // Get roles the user is not in
            var colAllRoles = RoleManager.Roles.Select(x => x.Name).ToList();

            // Go get the roles for an individual
            ApplicationUser user = UserManager.FindByName(UserName);

            // If we could not find the user, throw an exception
            if (user == null)
            {
                throw new Exception("Could not find the User");
            }

            var colRolesForUser = UserManager.GetRoles(user.Id).ToList();
            var colRolesUserInNotIn = (from objRole in colAllRoles
                                       where !colRolesForUser.Contains(objRole)
                                       select objRole).ToList();

            if (colRolesUserInNotIn.Count() == 0)
            {
                colRolesUserInNotIn.Add("No Roles Found");
            }

            return colRolesUserInNotIn;
        }

    }
}