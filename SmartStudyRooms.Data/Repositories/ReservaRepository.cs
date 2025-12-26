using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SmartStudyRooms.Data.Models;

namespace SmartStudyRooms.Data.Repositories
{
    public class ReservaRepository
    {
        private readonly string _conn;

        public ReservaRepository(IConfiguration config)
        {
            _conn = config.GetConnectionString("DefaultConnection");
        }

        public bool SalaDisponivel(int salaId, DateTime inicio, DateTime fim)
        {
            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(
                @"SELECT COUNT(*) FROM Reservas
                  WHERE SalaId = @salaId
                  AND Ativa = 1
                  AND (
                        (@inicio BETWEEN Inicio AND Fim)
                     OR (@fim BETWEEN Inicio AND Fim)
                     OR (Inicio BETWEEN @inicio AND @fim)
                  )", conn))
            {
                cmd.Parameters.Add("@salaId", SqlDbType.Int).Value = salaId;
                cmd.Parameters.Add("@inicio", SqlDbType.DateTime).Value = inicio;
                cmd.Parameters.Add("@fim", SqlDbType.DateTime).Value = fim;

                conn.Open();
                return (int)cmd.ExecuteScalar() == 0;
            }
        }

        public int CriarReserva(Reserva r)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();

                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        var cmdReserva = new SqlCommand(
                            @"INSERT INTO Reservas (SalaId, Inicio, Fim, Ativa)
                              OUTPUT INSERTED.ReservaId
                              VALUES (@sala, @inicio, @fim, 1)", conn, tran);

                        cmdReserva.Parameters.Add("@sala", SqlDbType.Int).Value = r.SalaId;
                        cmdReserva.Parameters.Add("@inicio", SqlDbType.DateTime).Value = r.Inicio;
                        cmdReserva.Parameters.Add("@fim", SqlDbType.DateTime).Value = r.Fim;

                        int id = (int)cmdReserva.ExecuteScalar();

                        var cmdSala = new SqlCommand(
                            @"UPDATE Salas
                              SET Ocupada = 1,
                                  ReservadaAte = @fim
                              WHERE SalaId = @id", conn, tran);

                        cmdSala.Parameters.Add("@fim", SqlDbType.DateTime).Value = r.Fim;
                        cmdSala.Parameters.Add("@id", SqlDbType.Int).Value = r.SalaId;

                        cmdSala.ExecuteNonQuery();

                        tran.Commit();
                        return id;
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
        public int ReservasExpiradas()
        {
            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(
                @"UPDATE Reservas
                  SET Ativa = 0
                  WHERE Ativa = 1
                  AND Fim < GETDATE() ", conn))
            {
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }
        public IEnumerable<Reserva> ListarReservas()
        {
            var list = new List<Reserva>();

            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(
                "SELECT ReservaId, SalaId, Inicio, Fim, Ativa FROM Reservas", conn))
            {
                conn.Open();
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        list.Add(new Reserva
                        {
                            ReservaId = rdr.GetInt32(0),
                            SalaId = rdr.GetInt32(1),
                            Inicio = rdr.GetDateTime(2),
                            Fim = rdr.GetDateTime(3),
                            Ativa = rdr.GetBoolean(4)
                        });
                    }
                }
            }
            return list;
        }
        public void CancelarReserva(int reservaId)
        {
            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(
                @"UPDATE Reservas
          SET Ativa = 0
          WHERE ReservaId = @id", conn))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = reservaId;
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public Reserva ObterReserva(int id)
        {
            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(
                @"SELECT ReservaId, SalaId, Inicio, Fim, Ativa
          FROM Reservas WHERE ReservaId = @id", conn))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                conn.Open();

                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        return new Reserva
                        {
                            ReservaId = rdr.GetInt32(0),
                            SalaId = rdr.GetInt32(1),
                            Inicio = rdr.GetDateTime(2),
                            Fim = rdr.GetDateTime(3),
                            Ativa = rdr.GetBoolean(4)
                        };
                    }
                }
            }
            return null;
        }

    }
}
