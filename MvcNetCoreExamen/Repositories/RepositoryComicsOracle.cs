using MvcNetCoreExamen.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;

#region PROCEDIMIENTOS ALMACENADOS

/*
create or replace procedure sp_delete_comic
(p_idcomic COMICS.IDCOMIC%TYPE)
as
begin
  delete from COMICS where IDCOMIC=p_idcomic;
  commit;
end;

create or replace procedure sp_insert_comic
(p_nombre COMICS.NOMBRE%TYPE, p_imagen COMICS.IMAGEN%TYPE, p_descripcion COMICS.DESCRIPCION%TYPE)
as 
cntId number;
begin
  select Nvl(max(idcomic), 0) + 1 into cntId from COMICS;
  
  insert into COMICS (idcomic,nombre,imagen, descripcion) values (cntId, p_nombre, p_imagen, p_descripcion);
  commit;
 end;

 */

#endregion

namespace MvcNetCoreExamen.Repositories
{
    public class RepositoryComicsOracle : IRepositoryComics
    {
        private DataTable tablaComics;
        private OracleConnection cn;
        private OracleCommand com;

        public RepositoryComicsOracle()
        {
            string connectionString = @"Data Source= LOCALHOST:1521/XE; Persist Security Info=True; User id=SYSTEM; Password=oracle";
            this.cn = new OracleConnection(connectionString);
            this.com = new OracleCommand();
            this.com.Connection = this.cn;
            string sql = "select * from COMICS";
            OracleDataAdapter ad = new OracleDataAdapter(sql, this.cn);
            this.tablaComics = new DataTable();
            ad.Fill(tablaComics);
        }

        public void DeleteComic(int idComic,string nombre)
        {
            OracleParameter pamIdComic = new OracleParameter(":p_idcomic", idComic);
            this.com.Parameters.Add(pamIdComic);
            OracleParameter pamNombre = new OracleParameter(":p_nombre", nombre);
            this.com.Parameters.Add(pamNombre);
            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = "sp_delete_comic";
            this.cn.Open();
            int af = this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }

        public Comic GetComicID(int idComic)
        {
            var consulta = from datos in this.tablaComics.AsEnumerable()
                           where datos.Field<int>("IDCOMIC") == idComic
                           select datos;
            if (consulta.Count() == 0)
            {
                return null;
            }
            else
            {
                var row = consulta.First();
                Comic comic = new Comic
                {
                    IdComic = row.Field<int>("IDCOMIC"),
                    Nombre = row.Field<string>("NOMBRE"),
                    Imagen = row.Field<string>("IMAGEN"),
                    Descripcion = row.Field<string>("DESCRIPCION")
                };
                return comic;
            }
        }

        public Comic GetComicNombre(string nombre)
        {
            var consulta = from datos in this.tablaComics.AsEnumerable()
                           where datos.Field<string>("NOMBRE") == nombre
                           select datos;

            var row = consulta.FirstOrDefault(); //Coje el primer resultado si hay

            if (row != null)
            {
                Comic comic = new Comic
                {
                    Imagen = row.Field<string>("IMAGEN"),
                    Descripcion = row.Field<string>("DESCRIPCION")
                };

                return comic;
            }
            else
            {
                return null;
            }
        }

        public List<Comic> GetComics()
        {
            var consulta = from datos in this.tablaComics.AsEnumerable()
                           select datos;
            List<Comic> comics = new List<Comic>();
            foreach (var row in consulta)
            {
                Comic com = new Comic
                {
                    IdComic = row.Field<int>("IDCOMIC"),
                    Nombre = row.Field<string>("NOMBRE"),
                    Imagen = row.Field<string>("IMAGEN"),
                    Descripcion = row.Field<string>("DESCRIPCION")
                };
                comics.Add(com);
            }
            return comics;
        }

        public List<string> GetNombresComics()
        {
            var consulta = (from datos in this.tablaComics.AsEnumerable()
                            select datos.Field<string>("NOMBRE")).Distinct();
            List<string> nombres = new List<string>();
            foreach (string nombre in consulta)
            {
                nombres.Add(nombre);
            }
            return nombres;
        }

        public void InsertComic(string nombre, string imagen, string descripcion)
        {
            int maxId = this.tablaComics.AsEnumerable().Max(row => row.Field<int>("IDCOMIC"));
            int nuevoID = maxId + 1;

            string sql = "insert into COMICS values(:idcomic, :nombre, :imagen, :descripcion)";
            OracleParameter pamIdComic = new OracleParameter(":idcomic", nuevoID);
            this.com.Parameters.Add(pamIdComic);
            OracleParameter pamNombre = new OracleParameter(":nombre", nombre);
            this.com.Parameters.Add(pamNombre);
            OracleParameter pamImagen = new OracleParameter(":imagen", imagen);
            this.com.Parameters.Add(pamImagen);
            OracleParameter pamDescripcion = new OracleParameter(":descripcion", descripcion);
            this.com.Parameters.Add(pamDescripcion);
            this.com.CommandType = CommandType.Text;
            this.com.CommandText = sql;
            this.cn.Open();
            int af = this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }

        public void InsertComicProcedure(string nombre, string imagen, string descripcion)
        {
            OracleParameter pamNombre = new OracleParameter(":p_nombre", nombre);
            this.com.Parameters.Add(pamNombre);
            OracleParameter pamImagen = new OracleParameter(":p_imagen", imagen);
            this.com.Parameters.Add(pamImagen);
            OracleParameter pamDescripcion = new OracleParameter(":p_descripcion", descripcion);
            this.com.Parameters.Add(pamDescripcion);

            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = "sp_insert_comic";
            this.cn.Open();
            int af = this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();

        }
    }
}
