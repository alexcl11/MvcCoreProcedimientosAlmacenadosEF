using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using MvcCoreProcedimientosAlmacenadosEF.Data;
using MvcCoreProcedimientosAlmacenadosEF.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace MvcCoreProcedimientosAlmacenadosEF.Repositories
{
    #region PROCEDIMIENTOS ALMACENADOS
    //CREATE PROCEDURE SP_ALL_ENFERMOS
    //AS

    //    SELECT* FROM ENFERMO
    //GO

    //CREATE PROCEDURE SP_FIND_ENFERMO
    //(@inscripcion nvarchar(50))
    //AS

    //    SELECT* FROM ENFERMO WHERE INSCRIPCION=@inscripcion
    //GO

    //CREATE PROCEDURE SP_DELETE_ENFERMO
    //(@inscripcion nvarchar(50))
    //AS
    //    DELETE FROM ENFERMO WHERE INSCRIPCION = @inscripcion
    //GO

    //CREATE PROCEDURE SP_INSERT_ENFERMO
    //(@apellido nvarchar(50), @direccion nvarchar(50), @fecha_nac datetime, @genero nvarchar(1), @num_seg_soc nvarchar(50))
    //AS
    //    DECLARE @last_inscripcion int
    //    SELECT @last_inscripcion=MAX(INSCRIPCION) FROM ENFERMO

    //    INSERT INTO ENFERMO VALUES(@last_inscripcion+1, @apellido, @direccion, @fecha_nac, @genero, @num_seg_soc)
    //GO
    #endregion

    public class RepositoryEnfermos
    {
        private HospitalContext context;

        public RepositoryEnfermos(HospitalContext context)
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

        public async Task<Enfermo> FindEnfermoAsync(string inscripcion)
        {
           
            //PARA LLAMAR A UN PROCEDIMIENTO QUE CONTIENE PARAMETROS LA LLAMADA
            //SE REALIZA MEDIANTE EL EL NOMBRE DEL PROCEDURE Y CADA PARAMETRO A
            //CONTINUACION DE LA DECLARACION DEL SQL: SP_PROCEDURE @PAM1, @PAM2
            string sql = "SP_FIND_ENFERMO @inscripcion";
            SqlParameter paramInscripcion = new SqlParameter("@inscripcion", inscripcion);
            //SI LOS DATOS QUE DEVUELVE EL PROCEDURE ESTAN MAPEADOS
            //CON UN MODEL, PODEMOS UTILIZAR EL METODO 
            //FromSqlRaw PARA RECUPERAR DIRECTAMENTE EL MODEL/S
            //NO PODMEOS CONSULTAR Y EXTRAER A LA VEZ CON LINQ, SE DEBE
            //REALIZAR SIEMPRE EN DOS PASOS
            var consulta = this.context.Enfermos.FromSqlRaw(sql, paramInscripcion);
            //DEBEMOS UTILIZAR AsEnumerable() PARA EXTRAER LOS DATOS 
            Enfermo enfermo = await consulta.AsAsyncEnumerable().FirstOrDefaultAsync();
            return enfermo;
                       
        }

        public async Task DeleteEmpleadoAsync(string inscripcion)
        {
            string sql = "SP_DELETE_ENFERMO";
            SqlParameter paramInscripcion = new SqlParameter("@inscripcion", inscripcion);
            using (DbCommand com = context.Database.GetDbConnection().CreateCommand())
            {
                com.Parameters.Add(paramInscripcion);
                com.CommandType = CommandType.StoredProcedure;
                com.CommandText = sql;

                await com.Connection.OpenAsync();
                await com.ExecuteNonQueryAsync();
                await com.Connection.CloseAsync();
                com.Parameters.Clear();
            }
        }

        public async Task DeleteEnfermoRawAsync(string inscripcion)
        {
            string sql = "SP_DELETE_ENFERMO @inscripcion";
            SqlParameter paramInscripcion = new SqlParameter("@inscripcion", inscripcion);
            await this.context.Database.ExecuteSqlRawAsync(sql, paramInscripcion);
        }

        public async Task CreateEnfermoAsync
            (string apellido, string direccion, DateTime fecha_nac, string genero, string num_seg_soc)
        {
            string sql = "SP_INSERT_ENFERMO @apellido, @direccion, @fecha_nac, @genero, @num_seg_soc";
            SqlParameter paramApellido = new SqlParameter("@apellido", apellido);
            SqlParameter paramDireccion = new SqlParameter("@direccion", direccion);
            SqlParameter paramFechaNac= new SqlParameter("@fecha_nac", fecha_nac);
            SqlParameter paramGenero= new SqlParameter("@genero", genero);
            SqlParameter paramNumSegSoc = new SqlParameter("@num_seg_soc", num_seg_soc);
            await this.context.Database.ExecuteSqlRawAsync
                (sql, paramApellido, paramDireccion, paramFechaNac, paramGenero, paramNumSegSoc);
            
        }
    }
}
