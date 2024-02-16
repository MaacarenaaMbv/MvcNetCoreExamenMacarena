using MvcNetCoreExamen.Models;
using System.Data;
using System.Data.SqlClient;

#region PROCEDIMIENTOS ALMACENADOS

/*
 alter procedure SP_DELETE_COMIC
@idcomic int, @nombre nvarchar(255)
as
	delete from COMICS where IDCOMIC=@idcomic and NOMBRE=@nombre
go

create procedure SP_INSERT_COMIC(@nombre nvarchar(255), @imagen nvarchar(255), @descripcion nvarchar(255))
as
	declare @cntId int
	select @cntId = max(idcomic) + 1 from COMICS
	insert into COMICS values(@cntId, @nombre, @imagen, @descripcion)
go


 */

#endregion

namespace MvcNetCoreExamen.Repositories
{
    public class RepositoryComicsSqlServer : IRepositoryComics
    {
        private DataTable tablaComics;
        private SqlConnection cn;
        private SqlCommand com;

        public RepositoryComicsSqlServer()
        {
            string connectionString = @"Data Source=DESKTOP-ETQNDBL\SQLEXPRESS;Initial Catalog=HOSPITAL;Persist Security Info=True;User ID=sa;Password=MCSD2023;";
            this.cn = new SqlConnection(connectionString);
            this.com = new SqlCommand();
            this.com.Connection = this.cn;
            this.tablaComics = new DataTable();
            string sql = "select * from COMICS";
            SqlDataAdapter adapter = new SqlDataAdapter(sql, this.cn);
            adapter.Fill(tablaComics);
        }

        public void DeleteComic(int idComic, string nombre)
        {
            this.com.Parameters.AddWithValue("@idcomic", idComic);
            this.com.Parameters.AddWithValue("@nombre", nombre);
            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = "SP_DELETE_COMIC";
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
            foreach(string nombre in consulta)
            {
                nombres.Add(nombre);
            }
            return nombres;
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
        public void InsertComic(string nombre, string imagen, string descripcion)
        {
            int maxId = this.tablaComics.AsEnumerable().Max(row => row.Field<int>("IDCOMIC"));
            int nuevoID = maxId + 1;

            string sql = "insert into COMICS values(@idcomic, @nombre, @imagen, @descripcion)";
            this.com.Parameters.AddWithValue("@idcomic", nuevoID);
            this.com.Parameters.AddWithValue("@nombre", nombre);
            this.com.Parameters.AddWithValue("@imagen", imagen);
            this.com.Parameters.AddWithValue("@descripcion", descripcion);
            this.com.CommandType = CommandType.Text;
            this.com.CommandText = sql;
            this.cn.Open();
            int af = this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }

        public void InsertComicProcedure(string nombre, string imagen, string descripcion)
        {
            this.com.Parameters.AddWithValue("@nombre", nombre);
            this.com.Parameters.AddWithValue("@imagen", imagen);
            this.com.Parameters.AddWithValue("@descripcion", descripcion);

            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = "SP_INSERT_COMIC";
            this.cn.Open();
            int af = this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }
    }
}
