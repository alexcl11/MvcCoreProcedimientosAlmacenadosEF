using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using MvcCoreProcedimientosAlmacenadosEF.Models;
using MvcCoreProcedimientosAlmacenadosEF.Repositories;
using System.Globalization;
using System.Threading.Tasks;

namespace MvcCoreProcedimientosAlmacenadosEF.Controllers
{
    public class DoctoresController : Controller
    {
        private RespositoryDoctores repo;
        public DoctoresController(RespositoryDoctores repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            List<string> especialidades = await this.repo.GetEspecialidadesAsync();
            ViewData["ESPECIALIDADES"] = especialidades;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string especialidad,int incremento, string accion)
        {
            List<string> especialidades = await this.repo.GetEspecialidadesAsync();
            ViewData["ESPECIALIDADES"] = especialidades;
            if (especialidad == null)
            {

                ViewData["MENSAJE"] = "Seleccione la especialidad";
                return View();
            }
            if(incremento != null)
            {
                if (accion == "conEF")
                {
                    await this.repo.IncrementarSalarioDoctoresEspecialidadSinSPAsync(especialidad, incremento);
                    ViewData["MENSAJE"] = "Con Entity Framework";
                } else
                {
                    await this.repo.IncrementarSalarioDoctoresEspecialidadAsync(especialidad, incremento);
                    ViewData["MENSAJE"] = "Con procedimientos almacenados";
                }
            }
            List<Doctor> doctores = await this.repo.GetDoctoresEspecialidadAsync(especialidad);
            return View(doctores);
        }
    }
}
