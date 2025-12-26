using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using SmartStudyRooms.Data.Models;
using System.Data;
using System.Data.SqlClient;


namespace SmartStudyRooms.Data.Repositories
{
    public class SalaRepository
    {
        private readonly string _conn;

        public SalaRepository(IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration), "IConfiguration não foi injetado no SalaRepository. Verifica Startup.cs.");

            _conn = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(_conn))
                throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada. Verifica connection.json e o nome da chave.");
        }

        public IEnumerable<Sala> GetAll()
        {
            var salas = new List<Sala>();

            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();

                string sql = "SELECT SalaId, Nome, Capacidade, Ocupada, ReservadaAte FROM Salas";

                using (var cmd = new SqlCommand(sql, conn))
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        salas.Add(new Sala
                        {
                            SalaId = rdr.GetInt32(0),
                            Nome = rdr.GetString(1),
                            Capacidade = rdr.GetInt32(2),
                            Ocupada = rdr.GetBoolean(3),

                       
                            ReservadaAte = rdr.IsDBNull(4)
                                ? (DateTime?)null
                                : rdr.GetDateTime(4)
                        });
                    }
                }
            }

            return salas;
        }
        public Sala GetById(int id)
        {
            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand("SELECT SalaId, Nome, Capacidade, Ocupada, ReservadaAte FROM Salas WHERE SalaId = @id", conn))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        return new Sala
                        {
                            SalaId = (int)rdr["SalaId"],
                            Nome = rdr["Nome"].ToString(),
                            Capacidade = (int)rdr["Capacidade"],
                            Ocupada = (bool)rdr["Ocupada"],
                            ReservadaAte = rdr["ReservadaAte"] == DBNull.Value ? (DateTime?)null : (DateTime)rdr["ReservadaAte"]
                        };
                    }
                }
            }
            return null;
        }

        public int Create(Sala sala)
        {
            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(
                "INSERT INTO Salas (Nome, Capacidade, Ocupada, ReservadaAte) " +
                "OUTPUT INSERTED.SalaId VALUES (@nome, @cap, @ocup, @reservadaAte)", conn))
            {
                cmd.Parameters.Add("@nome", SqlDbType.VarChar, 50).Value = sala.Nome;
                cmd.Parameters.Add("@cap", SqlDbType.Int).Value = sala.Capacidade;
                cmd.Parameters.Add("@ocup", SqlDbType.Bit).Value = sala.Ocupada;
                cmd.Parameters.Add("@reservadaAte", SqlDbType.DateTime)
                   .Value = (object)sala.ReservadaAte ?? DBNull.Value;

                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public bool Update(Sala sala)
        {
            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand("UPDATE Salas SET Nome=@nome, Capacidade=@cap, Ocupada=@ocup, ReservadaAte=@reservadaAte WHERE SalaId=@id", conn))
            {
                cmd.Parameters.Add("@nome", SqlDbType.VarChar, 50).Value = sala.Nome;
                cmd.Parameters.Add("@cap", SqlDbType.Int).Value = sala.Capacidade;
                cmd.Parameters.Add("@ocup", SqlDbType.Bit).Value = sala.Ocupada;
                cmd.Parameters.Add("@reservadaAte", SqlDbType.DateTime).Value = (object)sala.ReservadaAte ?? DBNull.Value;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = sala.SalaId;

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        public bool Delete(int id)
        {
            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand("DELETE FROM Salas WHERE SalaId=@id", conn))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        public IEnumerable<Sala> GetDisponiveis()
        {
            var list = new List<Sala>();
            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand("SELECT SalaId, Nome, Capacidade, Ocupada, ReservadaAte FROM Salas WHERE Ocupada = 0 AND (ReservadaAte IS NULL OR ReservadaAte < GETDATE())", conn))
            {
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        list.Add(new Sala
                        {
                            SalaId = (int)rdr["SalaId"],
                            Nome = rdr["Nome"].ToString(),
                            Capacidade = (int)rdr["Capacidade"],
                            Ocupada = (bool)rdr["Ocupada"],
                            ReservadaAte = rdr["ReservadaAte"] == DBNull.Value ? (DateTime?)null : (DateTime)rdr["ReservadaAte"]
                        });
                    }
                }
            }
            return list;
        }

        public void LibertarSala(int salaId)
        {
            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(
                @"UPDATE Salas
          SET Ocupada = 0,
              ReservadaAte = NULL
          WHERE SalaId = @id", conn))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = salaId;
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


        public void AtualizarOcupacao(int salaId, bool ocupada)
        {
            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(
                "UPDATE Salas SET Ocupada = @ocupada WHERE SalaId = @id", conn))
            {
                cmd.Parameters.Add("@ocupada", SqlDbType.Bit).Value = ocupada;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = salaId;

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public int LibertarSalasPorFimReserva()
        {
            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(
                @"UPDATE Salas
                SET Ocupada = 0,
                ReservadaAte = NULL
                WHERE ReservadaAte IS NOT NULL
                AND ReservadaAte < GETDATE()", conn))
            {
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }
        public IEnumerable<int> SalasComReservaExpirada()
        {
            var salas = new List<int>();

            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(
                @"SELECT SalaId
          FROM Salas
          WHERE ReservadaAte IS NOT NULL
          AND ReservadaAte < GETDATE()", conn))
            {
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                        salas.Add(rdr.GetInt32(0));
                }
            }

            return salas;
        }

    }
}
