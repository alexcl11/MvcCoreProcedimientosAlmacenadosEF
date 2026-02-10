using Microsoft.EntityFrameworkCore;
using MvcCoreProcedimientosAlmacenadosEF.Data;
using MvcCoreProcedimientosAlmacenadosEF.Models;
using System.Data;
using System.Data.Common;

namespace MvcCoreProcedimientosAlmacenadosEF.Repositories
{
    public class RepositoryEnfermos
    {
        private EnfermosContext context;

        public RepositoryEnfermos(EnfermosContext context)
        {
            this.context = context;
        }

        public async Task<List<Enfermo>> GetEnfermosAsync()
        {
            using (DbCommand com = this.context.Database.GetDbConnection().CreateCommand())
            {
                string sql = "SP_ALL_ENFERMOS";
                com.CommandType = CommandType.StoredProcedure;
                com.CommandText = sql;
                await com.Connection.OpenAsync();
                DbDataReader reader = await com.ExecuteReaderAsync();
                List<Enfermo> enfermos = new List<Enfermo>();
                while (await reader.ReadAsync())
                {
                    Enfermo enf = new Enfermo();
                    enf.Inscripcion = reader["INSCRIPCION"].ToString();
                    enf.Apellido = reader["APELLIDO"].ToString();
                    enf.Direccion = reader["DIRECCION"].ToString();
                    enf.FechaNacimiento = DateTime.Parse(reader["FECHA_NAC"].ToString());
                    enf.Genero = reader["S"].ToString();
                    enf.NumSegSocial = reader["NSS"].ToString();
                    enfermos.Add(enf);
                }
                await reader.CloseAsync();
                await com.Connection.CloseAsync();
                return enfermos;
            }
        }
    }
}
