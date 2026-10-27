using Labourworks.Application.DTOs;
using Labourworks.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Labourworks.Web.Controllers
{
    public class ContractorMasterController : Controller
    {
        private readonly IContractorMasterRepository _contractorRepository;

        public ContractorMasterController(
            IContractorMasterRepository contractorRepository)
        {
            _contractorRepository = contractorRepository;
        }

        #region Index

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        #endregion

        #region Get All

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await _contractorRepository.GetAllAsync();

                return Json(new
                {
                    status = true,
                    data
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    status = false,
                    message = "Unable to load contractors."
                });
            }
        }

        #endregion

        #region Get By Id

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var data = await _contractorRepository.GetByIdAsync(id);

                if (data == null)
                {
                    return NotFound(new
                    {
                        status = false,
                        message = "Contractor not found."
                    });
                }

                return Json(new
                {
                    status = true,
                    data
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    status = false,
                    message = "Unable to retrieve contractor."
                });
            }
        }

        #endregion

        #region Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [FromBody] ContractorMasterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Please enter valid contractor details.",
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

            if (!model.LicenceValidFrom.HasValue ||
                !model.LicenceValidTo.HasValue ||
                model.LicenceValidTo.Value < model.LicenceValidFrom.Value)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Licence end date must be on or after the start date."
                });
            }

            try
            {
                var result = await _contractorRepository.AddAsync(model);

                return Json(new
                {
                    status = result,
                    message = result
                        ? "Contractor saved successfully."
                        : "Unable to save contractor."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    status = false,
                    message = "An error occurred while saving the contractor."
                });
            }
        }

        #endregion

        #region Update

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(
            [FromBody] ContractorMasterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Please enter valid contractor details."
                });
            }

            if (model.ContractorId <= 0)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Invalid contractor ID."
                });
            }

            if (!model.LicenceValidFrom.HasValue ||
                !model.LicenceValidTo.HasValue ||
                model.LicenceValidTo.Value < model.LicenceValidFrom.Value)
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Licence end date must be on or after the start date."
                });
            }

            try
            {
                var result = await _contractorRepository.UpdateAsync(model);

                return Json(new
                {
                    status = result,
                    message = result
                        ? "Contractor updated successfully."
                        : "Contractor not found or unable to update."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    status = false,
                    message = "An error occurred while updating the contractor."
                });
            }
        }

        #endregion

        #region Delete (Soft Delete)

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _contractorRepository.DeleteAsync(id);

                return Json(new
                {
                    status = result,
                    message = result
                        ? "Contractor deleted successfully."
                        : "Contractor not found or already deleted."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    status = false,
                    message = "An error occurred while deleting the contractor."
                });
            }
        }

        #endregion
    }
}
