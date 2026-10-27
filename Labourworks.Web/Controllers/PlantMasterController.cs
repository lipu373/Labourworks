using Labourworks.Application.Interfaces;
using Labourworks.Domain.Entities;
using Labourworks.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Labourworks.Web.Controllers
{
    public class PlantMasterController : Controller
    {

        private readonly IPlantMasterRepository _plantRepository;
        private readonly ApplicationDbContext _context;


        public PlantMasterController(IPlantMasterRepository plantRepository, ApplicationDbContext context)
        {
            _plantRepository = plantRepository;
            _context = context;
        }

        #region Index

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
                var data = await _plantRepository.GetAllAsync();

                return Json(new
                {
                    status = true,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    message = ex.Message
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
                var data = await _plantRepository.GetByIdAsync(id);

                if (data == null)
                {
                    return Json(new
                    {
                        status = false,
                        message = "Plant not found."
                    });
                }

                return Json(new
                {
                    status = true,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    message = ex.Message
                });
            }
        }

        #endregion

        #region Create

        public IActionResult Create()
        {
          

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] PlantMaster model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new
                    {
                        status = false,
                        message = "Invalid Data."
                    });
                }

                bool result = await _plantRepository.AddAsync(model);

                if (result)
                {
                    return Json(new
                    {
                        status = true,
                        message = "Plant saved successfully."
                    });
                }

                return Json(new
                {
                    status = false,
                    message = "Unable to save plant."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    message = ex.Message
                });
            }
        }

        #endregion

        #region Update
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _plantRepository.GetByIdAsync(id);

            if (model == null)
                return NotFound();



            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromBody] PlantMaster model)
        {
            bool result = await _plantRepository.UpdateAsync(model);

            return Json(new
            {
                status = result,
                message = result ? "Plant updated successfully." : "Unable to update plant."
            });
        }

        #endregion

        #region Delete

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool result = await _plantRepository.DeleteAsync(id);

                if (result)
                {
                    return Json(new
                    {
                        status = true,
                        message = "Plant deleted successfully."
                    });
                }

                return Json(new
                {
                    status = false,
                    message = "Unable to delete plant."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = false,
                    message = ex.Message
                });
            }
        }

        #endregion
    }
}

