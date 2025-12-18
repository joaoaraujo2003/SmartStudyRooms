using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SmartStudyRooms.Data.Repositories
{
    public class SensorRepository
    {
        private readonly string _conn;

        public SensorRepository(IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _conn = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(_conn))
                throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' não encontrada.");
        }

        public IEnumerable<int> SalasInativasHaMaisDe15Min()
        {
            var salas = new List<int>();

            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(
                @"SELECT SalaId
                  FROM Sensores
                  WHERE Ocupada = 0
                  AND UltimaAtualizacao < DATEADD(MINUTE, -15, GETDATE())", conn))
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

        public void AtualizarSensor(int salaId, bool ocupada)
        {
            using (var conn = new SqlConnection(_conn))
            using (var cmd = new SqlCommand(
                @"IF EXISTS (SELECT 1 FROM Sensores WHERE SalaId = @salaId)
          BEGIN
              UPDATE Sensores
              SET Ocupada = @ocupada,
                  UltimaAtualizacao = GETDATE()
              WHERE SalaId = @salaId
          END
          ELSE
          BEGIN
              INSERT INTO Sensores (SalaId, Ocupada, UltimaAtualizacao)
              VALUES (@salaId, @ocupada, GETDATE())
          END", conn))
            {
                cmd.Parameters.Add("@salaId", SqlDbType.Int).Value = salaId;
                cmd.Parameters.Add("@ocupada", SqlDbType.Bit).Value = ocupada;

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
   
    }
}
