using Microsoft.AspNetCore.Mvc;
using MvcCoreProcedimientosAlmacenadosEF.Models;
using MvcCoreProcedimientosAlmacenadosEF.Repositories;

namespace MvcCoreProcedimientosAlmacenadosEF.Controllers
{
    public class TrabajadoresController : Controller
    {
        private RepositoryTrabajadores repo;

        public TrabajadoresController(RepositoryTrabajadores repo)
        {
            this.repo = repo;
        }
        public async Task<IActionResult> Index()
        {
            TrabajadoresModel trabajadores = await this.repo.GetTrabajadoresAsync();
            ViewData["OFICIOS"] = await this.repo.GetOficiosAsync();
            return View(trabajadores);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string oficio)
        {
            TrabajadoresModel trabajadores = await this.repo.GetTrabajadoresOficioAsync(oficio);
            ViewData["OFICIOS"] = await this.repo.GetOficiosAsync();
            return View(trabajadores);
        }
    }
}
