
using Labourworks.Application.DTOs;
using Labourworks.Application.Interfaces;
using Labourworks.Domain.Entities;
using Labourworks.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Labourworks.Web.Controllers
{
    public class UserMasterController : Controller
    {
        private readonly IUserMasterRepository _userRepository;
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<UserMaster> _passwordHasher;

        public UserMasterController(
      IUserMasterRepository userRepository,
      ApplicationDbContext context,
      IPasswordHasher<UserMaster> passwordHasher)
        {
            _userRepository = userRepository;
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // GET: /UserMaster
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // GET: /UserMaster/GetAll
        // Returns user records without exposing the Password column.
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _context.UserMaster
                    .AsNoTracking()
                    .Where(u => u.DeletedDate == null)
                    .OrderByDescending(u => u.UserMasterId)
                    .Select(u => new
                    {
                        userMasterId = u.UserMasterId,
                        userID = u.UserID,
                        fullName = u.FullName,
                        roleId = u.RoleId,
                        plantId = u.PlantId,
                        contractorId = u.ContractorId,
                        email = u.Email,
                        mobileNumber = u.MobileNumber,
                        status = u.Status,
                        isActive = u.IsActive,
                        createdDate = u.CreatedDate
                    })
                    .ToListAsync();

                return Json(new
                {
                    status = true,
                    data = users
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    status = false,
                    message = "Unable to load users."
                });
            }
        }

        // GET: /UserMaster/GetById?id=1
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var user = await _context.UserMaster
                    .AsNoTracking()
                    .Where(u =>
                        u.UserMasterId == id &&
                        u.DeletedDate == null)
                    .Select(u => new
                    {
                        userMasterId = u.UserMasterId,
                        userID = u.UserID,
                        fullName = u.FullName,
                        roleId = u.RoleId,
                        plantId = u.PlantId,
                        contractorId = u.ContractorId,
                        email = u.Email,
                        mobileNumber = u.MobileNumber,
                        status = u.Status,
                        isActive = u.IsActive
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound(new
                    {
                        status = false,
                        message = "User record not found."
                    });
                }

                return Json(new
                {
                    status = true,
                    data = user
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    status = false,
                    message = "Unable to retrieve the user."
                });
            }
        }

        // GET: /UserMaster/GetDropdowns
        [HttpGet]
        public async Task<IActionResult> GetDropdowns()
        {
            try
            {
                var roles = await _context.RoleMaster
                    .AsNoTracking()
                    .Where(r => r.IsActive)
                    .OrderBy(r => r.RoleName)
                    .Select(r => new
                    {
                        id = r.RoleId,
                        name = r.RoleName
                    })
                    .ToListAsync();

                var plants = await _context.PlantMaster
                    .AsNoTracking()
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.PlantName)
                    .Select(p => new
                    {
                        id = p.PlantId,
                        name = p.PlantName
                    })
                    .ToListAsync();

                var contractors = await _context.ContractorMaster
                    .AsNoTracking()
                    .Where(c => c.IsActive &&
                                c.DeletedDate == null)
                    .OrderBy(c => c.ContractorName)
                    .Select(c => new
                    {
                        id = c.ContractorId,
                        name = c.ContractorName
                    })
                    .ToListAsync();

                return Json(new
                {
                    status = true,
                    data = new
                    {
                        roles,
                        plants,
                        contractors
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    status = false,
                    message = "Unable to load dropdown data. Check the RoleMaster, PlantMaster and ContractorMaster entity properties."
                });
            }
        }

        // POST: /UserMaster/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [FromBody] UserMasterDto model)
        {
            if (model == null)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Invalid user data."
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Please check the required fields.",
                    errors = ModelState
                        .Where(x => x.Value != null &&
                                    x.Value.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors
                                .Select(e => e.ErrorMessage)
                                .ToArray())
                });
            }

            try
            {
                model.UserID = model.UserID.Trim();
                model.FullName = model.FullName.Trim();
                model.Email = model.Email.Trim();

                if (!await ValidateReferencesAsync(model))
                {
                    return BadRequest(new
                    {
                        status = false,
                        message = "Select a valid active role, plant and contractor."
                    });
                }

                if (await _userRepository.UserIdExistsAsync(model.UserID))
                {
                    return Conflict(new
                    {
                        status = false,
                        message = "This User ID already exists."
                    });
                }

                // Password is mandatory when creating a new user.
                if (string.IsNullOrWhiteSpace(model.Password))
                {
                    return BadRequest(new
                    {
                        status = false,
                        message = "Password is required when creating a user."
                    });
                }

                if (model.Password.Length < 12)
                {
                    return BadRequest(new
                    {
                        status = false,
                        message = "Password must be at least 12 characters long."
                    });
                }

                var user = new UserMaster
                {
                    UserID = model.UserID,
                    FullName = model.FullName,
                    RoleId = model.RoleId,
                    PlantId = model.PlantId,
                    ContractorId = model.ContractorId,
                    Email = model.Email,
                    MobileNumber = model.MobileNumber?.Trim(),
                    Status = model.Status,
                    IsActive = model.IsActive,
                    CreatedDate = DateTime.Now
                };

                // Store a secure password hash, never the plain-text password.
                user.Password = _passwordHasher.HashPassword(
                    user,
                    model.Password
                );

                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();

                return Json(new
                {
                    status = true,
                    message = "User created successfully.",
                    id = user.UserMasterId
                });
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    status = false,
                    message = "Unable to save the user. Check for duplicate values and database constraints."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    status = false,
                    message = "An error occurred while creating the user."
                });
            }
        }

        // POST: /UserMaster/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(
            [FromBody] UserMasterDto model)
        {
            if (model == null || model.UserMasterId <= 0)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "A valid user ID is required."
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Please check the required fields.",
                    errors = ModelState
                        .Where(x => x.Value != null &&
                                    x.Value.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors
                                .Select(e => e.ErrorMessage)
                                .ToArray())
                });
            }

            try
            {
                var user = await _userRepository.GetByIdAsync(
                    model.UserMasterId);

                if (user == null || user.DeletedDate != null)
                {
                    return NotFound(new
                    {
                        status = false,
                        message = "User record not found."
                    });
                }

                model.UserID = model.UserID.Trim();
                model.FullName = model.FullName.Trim();
                model.Email = model.Email.Trim();

                if (!await ValidateReferencesAsync(model))
                {
                    return BadRequest(new
                    {
                        status = false,
                        message = "Select a valid active role, plant and contractor."
                    });
                }

                if (await _userRepository.UserIdExistsAsync(
                    model.UserID,
                    model.UserMasterId))
                {
                    return Conflict(new
                    {
                        status = false,
                        message = "This User ID already exists."
                    });
                }

                user.UserID = model.UserID;
                user.FullName = model.FullName;
                user.RoleId = model.RoleId;
                user.PlantId = model.PlantId;
                user.ContractorId = model.ContractorId;
                user.Email = model.Email;
                user.MobileNumber = model.MobileNumber?.Trim();
                user.Status = model.Status;
                user.IsActive = model.IsActive;
                user.UpdatedDate = DateTime.Now;

                // Do not overwrite the existing password here.

                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();

                return Json(new
                {
                    status = true,
                    message = "User updated successfully."
                });
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    status = false,
                    message = "Unable to update the user. Check database constraints."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    status = false,
                    message = "An error occurred while updating the user."
                });
            }
        }

        // POST: /UserMaster/Delete?id=1
        // Soft delete; does not physically remove the database row.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "A valid user ID is required."
                });
            }

            try
            {
                var deleted = await _userRepository.DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound(new
                    {
                        status = false,
                        message = "User record not found."
                    });
                }

                return Json(new
                {
                    status = true,
                    message = "User deleted successfully."
                });
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    status = false,
                    message = "Unable to delete this user because of a database constraint."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    status = false,
                    message = "An error occurred while deleting the user."
                });
            }
        }

        private async Task<bool> ValidateReferencesAsync(
            UserMasterDto model)
        {
            var validRole = await _context.RoleMaster.AnyAsync(r =>
                r.RoleId == model.RoleId && r.IsActive);

            if (!validRole)
                return false;

            var validPlant = await _context.PlantMaster.AnyAsync(p =>
                p.PlantId == model.PlantId && p.IsActive);

            if (!validPlant)
                return false;

            if (model.ContractorId.HasValue)
            {
                var validContractor =
                    await _context.ContractorMaster.AnyAsync(c =>
                        c.ContractorId == model.ContractorId.Value &&
                        c.IsActive &&
                        c.DeletedDate == null);

                if (!validContractor)
                    return false;
            }

            if (model.Status != "Active" &&
                model.Status != "Inactive" &&
                model.Status != "Blocked")
            {
                return false;
            }

            return true;
        }
    }
}
