using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MvcCoreProcedimientosAlmacenadosEF.Data;
using MvcCoreProcedimientosAlmacenadosEF.Models;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;

namespace MvcCoreProcedimientosAlmacenadosEF.Repositories
{

    #region PROCEDIMIENTOS ALMACENADOS
    /*
     
        ALTER PROCEDURE SP_ESPECIALIDADES_DOCTORES
        AS
	        SELECT DISTINCT ESPECIALIDAD FROM DOCTOR
        GO
        ALTER PROCEDURE SP_DOCTORES_ESPECIALIDAD
        (@especialidad NVARCHAR(50))
        AS
	        SELECT * FROM DOCTOR WHERE ESPECIALIDAD=@especialidad
        GO
        ALTER PROCEDURE SP_INCREMENTAR_SALARIO_ESPECIALIDAD
        (@especialidad NVARCHAR(50), @incremento int)
        AS
	        UPDATE DOCTOR SET SALARIO=SALARIO+@incremento WHERE ESPECIALIDAD=@especialidad
        GO

     */
    #endregion
    public class RespositoryDoctores
    {
        private HospitalContext context;
        public RespositoryDoctores(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<List<string>> GetEspecialidadesAsync()
        {
            string sql = "SP_ESPECIALIDADES_DOCTORES";
            using (DbCommand com = this.context.Database.GetDbConnection().CreateCommand())
            {
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.CommandText = sql;
                await com.Connection.OpenAsync();
                DbDataReader reader = await com.ExecuteReaderAsync();
                List<string> especialidades = new List<string>();

                while (await reader.ReadAsync())
                {
                    string especialidad = reader["ESPECIALIDAD"].ToString();
                    especialidades.Add(especialidad);
                }

                await reader.CloseAsync();
                await com.Connection.CloseAsync();
                return especialidades;
            }
        }

        public async Task<List<Doctor>> GetDoctoresEspecialidadAsync(string especialidad)
        {
            string sql = "SP_DOCTORES_ESPECIALIDAD @especialidad";
            SqlParameter paramEspecialidad = new SqlParameter("@especialidad", especialidad);
            List<Doctor> doctores = new List<Doctor>();
            doctores = await this.context.Doctores.FromSqlRaw(sql, paramEspecialidad).ToListAsync();            
            return doctores;
        }
        
        public async Task IncrementarSalarioDoctoresEspecialidadAsync(string especialidad, int incremento)
        {
            string sql = "SP_INCREMENTAR_SALARIO_ESPECIALIDAD @especialidad, @incremento";
            SqlParameter paramEspecialidad = new SqlParameter("@especialidad", especialidad);
            SqlParameter paramIncremento = new SqlParameter("@incremento", incremento);
            await this.context.Database.ExecuteSqlRawAsync(sql, paramEspecialidad, paramIncremento);
        }
        public async Task IncrementarSalarioDoctoresEspecialidadSinSPAsync(string especialidad, int incremento)
        {
            var consulta = from datos in this.context.Doctores
                           where datos.Especialidad == especialidad
                           select datos;

            List<Doctor> doctores = await consulta.ToListAsync();

            foreach (Doctor doctor in doctores)
            {
                doctor.Salario += incremento;
                await this.context.SaveChangesAsync();
            }
        }

    }
}
