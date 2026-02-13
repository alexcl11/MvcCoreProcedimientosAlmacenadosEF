using Microsoft.EntityFrameworkCore;
using MvcCoreProcedimientosAlmacenadosEF.Data;
using MvcCoreProcedimientosAlmacenadosEF.Models;
using System.Runtime;

namespace MvcCoreProcedimientosAlmacenadosEF.Repositories
{

    #region VISTA EMPLEADOS
    /*
        ALTER VIEW V_EMPLEADOS_DEPARTAMENTOS
        AS
	        SELECT CAST(ISNULL( ROW_NUMBER() OVER (ORDER BY EMP.APELLIDO), 0)AS INT) AS ID,
	        EMP.APELLIDO, EMP.OFICIO, EMP.SALARIO, 
	        DEPT.DNOMBRE AS DEPARTAMENTO, DEPT.LOC AS LOCALIDAD
	        FROM EMP 
	        INNER JOIN DEPT ON EMP.DEPT_NO=DEPT.DEPT_NO
        GO

        CREATE VIEW V_TRABAJADORES
        AS
	        SELECT EMP_NO AS IDTRABAJADOR, APELLIDO, OFICIO, SALARIO FROM EMP
	        UNION 
	        SELECT DOCTOR_NO, APELLIDO, ESPECIALIDAD, SALARIO FROM DOCTOR
	        UNION SELECT EMPLEADO_NO, APELLIDO, FUNCION, SALARIO FROM PLANTILLA
        GO

        CREATE PROCEDURE SP_TRABAJADORES_OFICIO
        (@oficio NVARCHAR(50), @personas INT OUT, @media INT OUT, @suma INT OUT)
        AS
	        SELECT * FROM V_TRABAJADORES
	        WHERE OFICIO=@oficio
	        SELECT @personas=COUNT(IDTRABAJADOR), @media = AVG(SALARIO), @suma = SUM(SALARIO) 
	        FROM V_TRABAJADORES WHERE OFICIO=@oficio
        GO
     */
    #endregion

    public class RepositoryEmpleados
    {
        private HospitalContext context;
        public RepositoryEmpleados(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<List<VistaEmpleado>> GetVistaEmpleadosAsync()
        {
            var consulta = from datos in this.context.VistaEmpleados
                           select datos;
            return await consulta.ToListAsync();
        } 
    }
}
