
using Labourworks.Application.DTOs;
using Labourworks.Application.Interfaces;
using Labourworks.Domain.Entities;
using Labourworks.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Labourworks.Web.Controllers;

public class WorkOrderMasterController : Controller
{
    private readonly IWorkOrderMasterRepository _repository;
    private readonly ApplicationDbContext _context;

    public WorkOrderMasterController(
        IWorkOrderMasterRepository repository,
        ApplicationDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    // GET: /WorkOrderMaster/Index
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    // GET: /WorkOrderMaster/GetAll
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await (
            from wo in _context.WorkOrderMaster.AsNoTracking()
            join contractor in _context.ContractorMaster.AsNoTracking()
                on wo.ContractorId equals contractor.ContractorId
            join plant in _context.PlantMaster.AsNoTracking()
                on wo.PlantId equals plant.PlantId
            join department in _context.DepartmentMaster.AsNoTracking()
                on wo.DepartmentId equals department.DepartmentId
            where wo.DeletedDate == null
            orderby wo.WorkOrderId descending
            select new
            {
                wo.WorkOrderId,
                wo.WorkOrderNumber,

                wo.ContractorId,
                ContractorName = contractor.ContractorName,

                wo.PlantId,
                PlantName = plant.PlantName,

                wo.DepartmentId,
                DepartmentName = department.DepartmentName,

                wo.DescriptionOfWork,
                wo.ValidFrom,
                wo.ValidTo,
                wo.ApprovedCapacity,
                wo.DeployedCount,
                wo.RatePerDay,
                wo.ServiceCharge,
                wo.Status,
                wo.IsActive
            })
            .ToListAsync();

        return Json(new
        {
            success = true,
            data = result
        });
    }

    // GET: /WorkOrderMaster/GetById?id=1
    [HttpGet]
    public async Task<IActionResult> GetById(int id)
    {
        var workOrder = await _repository.GetByIdAsync(id);

        if (workOrder == null)
        {
            return NotFound(new
            {
                success = false,
                message = "Work order not found."
            });
        }

        var dto = new WorkOrderMasterDto
        {
            WorkOrderId = workOrder.WorkOrderId,
            WorkOrderNumber = workOrder.WorkOrderNumber,
            ContractorId = workOrder.ContractorId,
            PlantId = workOrder.PlantId,
            DepartmentId = workOrder.DepartmentId,
            DescriptionOfWork = workOrder.DescriptionOfWork,
            ValidFrom = workOrder.ValidFrom,
            ValidTo = workOrder.ValidTo,
            ApprovedCapacity = workOrder.ApprovedCapacity,
            DeployedCount = workOrder.DeployedCount,
            RatePerDay = workOrder.RatePerDay,
            ServiceCharge = workOrder.ServiceCharge,
            Status = workOrder.Status
        };

        return Json(new
        {
            success = true,
            data = dto
        });
    }

    // GET: /WorkOrderMaster/GetDropdowns
    [HttpGet]
    public async Task<IActionResult> GetDropdowns()
    {
        var contractors = await _context.ContractorMaster
            .AsNoTracking()
            .Where(x => x.IsActive && x.DeletedDate == null)
            .OrderBy(x => x.ContractorName)
            .Select(x => new
            {
                x.ContractorId,
                Name = x.ContractorName
            })
            .ToListAsync();

        var plants = await _context.PlantMaster
            .AsNoTracking()
            .Where(x => x.IsActive && x.DeletedDate == null)
            .OrderBy(x => x.PlantName)
            .Select(x => new
            {
                x.PlantId,
                Name = x.PlantName
            })
            .ToListAsync();

        var departments = await _context.DepartmentMaster
            .AsNoTracking()
            .Where(x => x.IsActive && x.DeletedDate == null)
            .OrderBy(x => x.DepartmentName)
            .Select(x => new
            {
                x.DepartmentId,
                Name = x.DepartmentName
            })
            .ToListAsync();

        return Json(new
        {
            success = true,
            contractors,
            plants,
            departments
        });
    }

    // POST: /WorkOrderMaster/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [FromBody] WorkOrderMasterDto model)
    {
        if (model == null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid request."
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                message = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault()
                    ?? "Please check the submitted data."
            });
        }

        if (model.ValidTo.Date < model.ValidFrom.Date)
        {
            return BadRequest(new
            {
                success = false,
                message = "Valid To must be on or after Valid From."
            });
        }

        if (await _repository.WorkOrderNumberExistsAsync(
                model.WorkOrderNumber.Trim()))
        {
            return BadRequest(new
            {
                success = false,
                message = "This Work Order Number already exists."
            });
        }

        // Validate selected master records.
        var referencesValid = await ValidateReferencesAsync(model);

        if (!referencesValid)
        {
            return BadRequest(new
            {
                success = false,
                message = "Please select valid Contractor, Plant and Department."
            });
        }

        var workOrder = new WorkOrderMaster
        {
            WorkOrderNumber = model.WorkOrderNumber.Trim(),
            ContractorId = model.ContractorId,
            PlantId = model.PlantId,
            DepartmentId = model.DepartmentId,
            DescriptionOfWork = model.DescriptionOfWork.Trim(),
            ValidFrom = model.ValidFrom.Date,
            ValidTo = model.ValidTo.Date,
            ApprovedCapacity = model.ApprovedCapacity,

            // DeployedCount is maintained separately; new records start at zero.
            DeployedCount = 0,

            RatePerDay = model.RatePerDay,
            ServiceCharge = model.ServiceCharge,
            Status = model.Status,
            CreatedDate = DateTime.Now,
            IsActive = model.Status == "Active"
        };

        await _repository.AddAsync(workOrder);
        await _repository.SaveChangesAsync();

        return Json(new
        {
            success = true,
            message = "Work order created successfully."
        });
    }

    // POST: /WorkOrderMaster/Update
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(
        [FromBody] WorkOrderMasterDto model)
    {
        if (model == null)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid request."
            });
        }

        if (model.WorkOrderId <= 0)
        {
            return BadRequest(new
            {
                success = false,
                message = "Invalid Work Order ID."
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                success = false,
                message = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault()
                    ?? "Please check the submitted data."
            });
        }

        if (model.ValidTo.Date < model.ValidFrom.Date)
        {
            return BadRequest(new
            {
                success = false,
                message = "Valid To must be on or after Valid From."
            });
        }

        var workOrder = await _repository.GetByIdAsync(
            model.WorkOrderId);

        if (workOrder == null)
        {
            return NotFound(new
            {
                success = false,
                message = "Work order not found."
            });
        }

        if (await _repository.WorkOrderNumberExistsAsync(
                model.WorkOrderNumber.Trim(),
                model.WorkOrderId))
        {
            return BadRequest(new
            {
                success = false,
                message = "This Work Order Number already exists."
            });
        }

        var referencesValid = await ValidateReferencesAsync(model);

        if (!referencesValid)
        {
            return BadRequest(new
            {
                success = false,
                message = "Please select valid Contractor, Plant and Department."
            });
        }

        workOrder.WorkOrderNumber = model.WorkOrderNumber.Trim();
        workOrder.ContractorId = model.ContractorId;
        workOrder.PlantId = model.PlantId;
        workOrder.DepartmentId = model.DepartmentId;
        workOrder.DescriptionOfWork = model.DescriptionOfWork.Trim();
        workOrder.ValidFrom = model.ValidFrom.Date;
        workOrder.ValidTo = model.ValidTo.Date;
        workOrder.ApprovedCapacity = model.ApprovedCapacity;
        workOrder.RatePerDay = model.RatePerDay;
        workOrder.ServiceCharge = model.ServiceCharge;
        workOrder.Status = model.Status;
        workOrder.IsActive = model.Status == "Active";
        workOrder.UpdatedDate = DateTime.Now;

        // Do not overwrite DeployedCount during normal editing.

        await _repository.UpdateAsync(workOrder);
        await _repository.SaveChangesAsync();

        return Json(new
        {
            success = true,
            message = "Work order updated successfully."
        });
    }

    // POST: /WorkOrderMaster/Delete?id=1
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repository.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                success = false,
                message = "Work order not found."
            });
        }

        await _repository.SaveChangesAsync();

        return Json(new
        {
            success = true,
            message = "Work order deleted successfully."
        });
    }

    // Validate that selected master records are active and not deleted.
    private async Task<bool> ValidateReferencesAsync(
        WorkOrderMasterDto model)
    {
        var contractorExists = await _context.ContractorMaster
            .AnyAsync(x =>
                x.ContractorId == model.ContractorId &&
                x.IsActive &&
                x.DeletedDate == null);

        var plantExists = await _context.PlantMaster
            .AnyAsync(x =>
                x.PlantId == model.PlantId &&
                x.IsActive &&
                x.DeletedDate == null);

        var departmentExists = await _context.DepartmentMaster
            .AnyAsync(x =>
                x.DepartmentId == model.DepartmentId &&
                x.IsActive &&
                x.DeletedDate == null);

        return contractorExists && plantExists && departmentExists;
    }
}
