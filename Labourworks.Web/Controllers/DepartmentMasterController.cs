using Labourworks.Application.DTOs;
using Labourworks.Application.Interfaces;
using Labourworks.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Labourworks.Web.Controllers
{
    public class DepartmentMasterController : Controller
    {
        private readonly IDepartmentMasterRepository _repository;

        public DepartmentMasterController(
            IDepartmentMasterRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var departments = await _repository.GetAllAsync();

            var data = departments.Select(d => new
            {
                d.DepartmentId,
                d.DepartmentName,
                d.IsActive,
                d.CreatedDate
            }).ToList();

            return Json(new
            {
                status = true,
                data
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Invalid department ID."
                });
            }

            var department = await _repository.GetByIdAsync(id);

            if (department == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Department not found."
                });
            }

            return Json(new
            {
                status = true,
                data = new
                {
                    department.DepartmentId,
                    department.DepartmentName,
                    department.IsActive
                }
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [FromBody] DepartmentMasterDto model)
        {
            if (model == null)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Invalid request."
                });
            }

            model.DepartmentName = model.DepartmentName?.Trim()
                ?? string.Empty;

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = false,
                    message = GetValidationMessage()
                });
            }

            if (await _repository.DepartmentNameExistsAsync(
                model.DepartmentName))
            {
                return Conflict(new
                {
                    status = false,
                    message = "Department name already exists."
                });
            }

            var currentUserId = GetCurrentUserId();

            var department = new DepartmentMaster
            {
                DepartmentName = model.DepartmentName,
                IsActive = model.IsActive,
                CreatedBy = currentUserId,
                CreatedDate = DateTime.Now
            };

            await _repository.AddAsync(department);
            await _repository.SaveChangesAsync();

            return Ok(new
            {
                status = true,
                message = "Department created successfully."
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(
            [FromBody] DepartmentMasterDto model)
        {
            if (model == null || model.DepartmentId <= 0)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Invalid department details."
                });
            }

            model.DepartmentName = model.DepartmentName?.Trim()
                ?? string.Empty;

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = false,
                    message = GetValidationMessage()
                });
            }

            var department = await _repository.GetByIdAsync(
                model.DepartmentId);

            if (department == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Department not found."
                });
            }

            if (await _repository.DepartmentNameExistsAsync(
                model.DepartmentName,
                model.DepartmentId))
            {
                return Conflict(new
                {
                    status = false,
                    message = "Department name already exists."
                });
            }

            department.DepartmentName = model.DepartmentName;
            department.IsActive = model.IsActive;
            department.UpdatedBy = GetCurrentUserId();
            department.UpdatedDate = DateTime.Now;

            await _repository.UpdateAsync(department);
            await _repository.SaveChangesAsync();

            return Ok(new
            {
                status = true,
                message = "Department updated successfully."
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Invalid department ID."
                });
            }

            var department = await _repository.GetByIdAsync(id);

            if (department == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Department not found."
                });
            }

            department.DeletedBy = GetCurrentUserId();
            department.DeletedDate = DateTime.Now;
            department.IsActive = false;

            await _repository.UpdateAsync(department);
            await _repository.SaveChangesAsync();

            return Ok(new
            {
                status = true,
                message = "Department deleted successfully."
            });
        }

        private int GetCurrentUserId()
        {
            var userId = User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userId, out var id))
            {
                return id;
            }

            throw new UnauthorizedAccessException(
                "Logged-in user ID was not found.");
        }

        private string GetValidationMessage()
        {
            return ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault(m =>
                    !string.IsNullOrWhiteSpace(m))
                ?? "Please enter valid department details.";
        }
    }
}
