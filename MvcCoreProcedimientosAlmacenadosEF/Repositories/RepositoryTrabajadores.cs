using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCoreProcedimientosAlmacenadosEF.Data;
using MvcCoreProcedimientosAlmacenadosEF.Models;
using System.Runtime.CompilerServices;

namespace MvcCoreProcedimientosAlmacenadosEF.Repositories
{
    public class RepositoryTrabajadores
    {
        private HospitalContext context;
        
        public RepositoryTrabajadores(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<TrabajadoresModel> GetTrabajadoresAsync()
        {
            var consulta = from datos in this.context.Trabajadores
                           select datos;
            TrabajadoresModel model = new TrabajadoresModel();
            model.Trabajadores = await consulta.ToListAsync();
            model.Personas = await consulta.CountAsync();
            model.Suma = await consulta.SumAsync(x => x.Salario);
            model.Media = (int) await consulta.AverageAsync(x => x.Salario);
            return model;
        }

        public async Task<List<string>> GetOficiosAsync()
        {
            var consulta = (from datos in this.context.Trabajadores
                            select datos.Oficio).Distinct();
            return await consulta.ToListAsync();
        }

        public async Task<TrabajadoresModel> GetTrabajadoresOficioAsync(string oficio)
        {
            //YA QUE TENEMOS MODEL, VAMOS A LLAMARLO CON EF
            //LA UNICA DIFERENCIA CUANDO TENEMOS PARAMETROS DE SALIDA 
            //ES INDICAR LA PALABRA OUT EN LA DECLARACION DE LAS VARIABLES
            string sql = "SP_TRABAJADORES_OFICIO @oficio, @personas out, @media out, @suma out";
            SqlParameter pamOficio = new SqlParameter("@oficio", oficio);
            SqlParameter pamPersonas = new SqlParameter("@personas", -1);
            pamPersonas.Direction = System.Data.ParameterDirection.Output;
            SqlParameter pamMedia = new SqlParameter("@media", -1);
            pamMedia.Direction = System.Data.ParameterDirection.Output;
            SqlParameter pamSuma = new SqlParameter("@suma", -1);
            pamSuma.Direction = System.Data.ParameterDirection.Output;
            //EJECUTAMOS LA CONSULTA CON EL MODEL FromSqlRaw()
            var consulta = this.context.Trabajadores.FromSqlRaw(sql, pamOficio, pamPersonas, pamMedia, pamSuma);

            TrabajadoresModel model = new TrabajadoresModel();
            model.Trabajadores = await consulta.ToListAsync();
            model.Personas = int.Parse(pamPersonas.Value.ToString());
            model.Media = int.Parse(pamMedia.Value.ToString());
            model.Suma = int.Parse(pamSuma.Value.ToString());
            return model;
        }

    }
}
