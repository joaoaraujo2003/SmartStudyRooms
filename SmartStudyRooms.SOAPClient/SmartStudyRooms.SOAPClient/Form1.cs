using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartStudyRooms.SOAPClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            listBox1.DrawMode = DrawMode.OwnerDrawFixed;
            listBox1.ItemHeight = 20;

            listBox1.DrawItem += listBox1_DrawItem;

            CarregarReservas();
            CarregarSalas();       
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
        private void CarregarSalas()
        {
            var client = new SoapSalasReference.SalaSoapServiceClient();
            using (new System.ServiceModel.OperationContextScope(client.InnerChannel))
            {
                ApiKeyHelper.AddApiKey(ApiKeyConfig.ApiKey);

                var salas = client.ListarSalas();

                listBox1.Items.Clear();

                foreach (var sala in salas)
                {
                    string estado =
                        sala.Ocupada ? "Ocupada" :
                        sala.ReservadaAte != null ? $"Reservada até {sala.ReservadaAte:HH:mm}" :
                        "Livre";

                    listBox1.Items.Add(new SalaClass
                    {
                        SalaId = sala.SalaId,
                        Estado = estado,
                        Texto = $"{sala.SalaId} - {sala.Nome} | Cap: {sala.Capacidade} | {estado}"
                    });
                }
            }
        }

        private void CarregarReservas()
        {
            var client = new SoapReservasReference.ReservaSoapServiceClient();

            using (new System.ServiceModel.OperationContextScope(client.InnerChannel))
            {
                ApiKeyHelper.AddApiKey(ApiKeyConfig.ApiKey);

                var reservas = client.ListarReservas();

                listBoxReservas.Items.Clear();

                foreach (var r in reservas)
                {
                    listBoxReservas.Items.Add(new ReservaClass
                    {
                        ReservaId = r.ReservaId,
                        SalaId = r.SalaId,
                        Inicio = r.Inicio,
                        Fim = r.Fim,
                        Ativa = r.Ativa
                    });
                }
            }
        }


        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            var item = (SalaClass)listBox1.Items[e.Index];

            Color cor = Color.White;

            if (item.Estado.Contains("Livre"))
                cor = Color.LightGreen;
            else if (item.Estado.Contains("Ocupada"))
                cor = Color.LightCoral;
            else
                cor = Color.Khaki;

            e.DrawBackground();
            using (var brush = new SolidBrush(cor))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            TextRenderer.DrawText(
                e.Graphics,
                item.Texto,
                e.Font,
                e.Bounds,
                Color.Black,
                TextFormatFlags.Left
            );

            e.DrawFocusRectangle();
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            CarregarSalas();
            CarregarReservas();
        }

        private void btnCriarReserva_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Seleciona uma sala.");
                return;
            }

            var salaSelecionada = listBox1.SelectedItem as SalaClass;

            if (salaSelecionada == null)
            {
                MessageBox.Show("Seleção inválida de sala.");
                return;
            }
         
            int salaId = salaSelecionada.SalaId;

            var reserva = new SoapReservasReference.Reserva
            {
                SalaId = salaId,
                Inicio = dtInicio.Value,
                Fim = dtFim.Value
            };

            var client = new SoapReservasReference.ReservaSoapServiceClient();

            using (new System.ServiceModel.OperationContextScope(client.InnerChannel))
            {
                ApiKeyHelper.AddApiKey(ApiKeyConfig.ApiKey);
                client.CriarReserva(reserva);
            }

            MessageBox.Show("Reserva criada com sucesso.");
            CarregarReservas();
        }

        private void btnCancelarReserva_Click(object sender, EventArgs e)
        {
            var reserva = listBoxReservas.SelectedItem as ReservaClass;
            if (reserva == null)
            {
                MessageBox.Show("Seleção inválida de sala.");
                return;
            }
            var client = new SoapReservasReference.ReservaSoapServiceClient();

            using (new System.ServiceModel.OperationContextScope(client.InnerChannel))
            {
                ApiKeyHelper.AddApiKey(ApiKeyConfig.ApiKey);
                client.CancelarReserva(reserva.ReservaId);
            }

            MessageBox.Show("Reserva cancelada.");
            CarregarReservas();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
