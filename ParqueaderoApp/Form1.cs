using System;
using System.Drawing;
using System.Windows.Forms;
using ParqueaderoApp.Logica;
using ParqueaderoApp.Modelos;

namespace ParqueaderoApp
{
    public partial class Form1 : Form
    {
        private GestorParqueadero gestor;
        private TabControl tabControl;
        private ErrorProvider errorProvider;

        // Controles Pestaña 1 (Registro)
        private TextBox txtPlaca, txtPropietario, txtHoras;
        private ComboBox cmbTipo, cmbFila, cmbColumna;
        private Button btnRegistrar;
        private DataGridView dgvVehiculos;

        // Controles Pestaña 2 (Búsqueda rápida y Mapa de celdas)
        private TextBox txtBuscarPlaca;
        private Button btnBuscar;
        private Label lblResultadoBusqueda;
        private DataGridView dgvMatriz;

        // Controles Pestaña 3 (Informe)
        private TextBox txtInforme;
        private Button btnActualizarInforme;

        public Form1()
        {
            gestor = new GestorParqueadero();
            errorProvider = new ErrorProvider();
            ConfigurarVentana();
            ConstruirInterfaz();
            ActualizarVistas();
        }

        private void ConfigurarVentana()
        {
            this.Text = "ParkControl C# - Gestión de Parqueadero";
            this.Size = new Size(800, 540);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void ConstruirInterfaz()
        {
            tabControl = new TabControl { Dock = DockStyle.Fill };

            // 3 Pestañas (Requisito 1)
            TabPage tabRegistro = new TabPage("1. Registro de Vehículos");
            ConstruirTabRegistro(tabRegistro);

            TabPage tabConsulta = new TabPage("2. Búsqueda y Mapa de Celdas");
            ConstruirTabConsultaYMapa(tabConsulta);

            TabPage tabInforme = new TabPage("3. Informe de Resultados");
            ConstruirTabInforme(tabInforme);

            tabControl.TabPages.AddRange(new[] { tabRegistro, tabConsulta, tabInforme });
            this.Controls.Add(tabControl);
        }

        private void ConstruirTabRegistro(TabPage tab)
        {
            Label lblTipo = new Label { Text = "Tipo:", Location = new Point(20, 20), Width = 50 };
            cmbTipo = new ComboBox { Location = new Point(75, 17), Width = 110, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTipo.Items.AddRange(new[] { "Carro", "Moto" });
            cmbTipo.SelectedIndex = 0;

            Label lblPlaca = new Label { Text = "Placa:", Location = new Point(210, 20), Width = 45 };
            txtPlaca = new TextBox { Location = new Point(260, 17), Width = 100 };

            Label lblProp = new Label { Text = "Propietario:", Location = new Point(390, 20), Width = 75 };
            txtPropietario = new TextBox { Location = new Point(470, 17), Width = 220 };

            Label lblFila = new Label { Text = "Piso (0-2):", Location = new Point(20, 60), Width = 65 };
            cmbFila = new ComboBox { Location = new Point(90, 57), Width = 60, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbFila.Items.AddRange(new object[] { 0, 1, 2 });
            cmbFila.SelectedIndex = 0;

            Label lblCol = new Label { Text = "Puesto (0-3):", Location = new Point(175, 60), Width = 80 };
            cmbColumna = new ComboBox { Location = new Point(260, 57), Width = 60, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbColumna.Items.AddRange(new object[] { 0, 1, 2, 3 });
            cmbColumna.SelectedIndex = 0;

            Label lblHoras = new Label { Text = "Horas:", Location = new Point(345, 60), Width = 50 };
            txtHoras = new TextBox { Location = new Point(400, 57), Width = 60 };

            btnRegistrar = new Button
            {
                Text = "Registrar Ingreso",
                Location = new Point(490, 55),
                Width = 200,
                Height = 28,
                BackColor = Color.SteelBlue,
                ForeColor = Color.White
            };
            btnRegistrar.Click += BtnRegistrar_Click;

            dgvVehiculos = new DataGridView
            {
                Location = new Point(20, 105),
                Size = new Size(730, 340),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvVehiculos.Columns.Add("Tipo", "Tipo");
            dgvVehiculos.Columns.Add("Placa", "Placa");
            dgvVehiculos.Columns.Add("Propietario", "Propietario");
            dgvVehiculos.Columns.Add("Ubicacion", "Ubicación [Piso, Puesto]");
            dgvVehiculos.Columns.Add("Horas", "Horas");
            dgvVehiculos.Columns.Add("Tarifa", "Total a Pagar");

            tab.Controls.AddRange(new Control[] {
                lblTipo, cmbTipo, lblPlaca, txtPlaca, lblProp, txtPropietario,
                lblFila, cmbFila, lblCol, cmbColumna, lblHoras, txtHoras,
                btnRegistrar, dgvVehiculos
            });
        }

        private void ConstruirTabConsultaYMapa(TabPage tab)
        {
            // Sección superior: Búsqueda rápida por placa
            Label lblBuscar = new Label { Text = "Buscar vehículo por Placa:", Location = new Point(20, 20), AutoSize = true };
            txtBuscarPlaca = new TextBox { Location = new Point(185, 17), Width = 120 };
            btnBuscar = new Button { Text = "Buscar", Location = new Point(320, 15), Width = 90, Height = 26 };
            btnBuscar.Click += BtnBuscar_Click;

            lblResultadoBusqueda = new Label
            {
                Text = "Ingrese una placa para consultar su ubicación y valor a pagar.",
                Location = new Point(20, 52),
                Size = new Size(730, 25),
                ForeColor = Color.DarkBlue,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            // Sección inferior: Matriz de ocupación
            Label lblInfoMatriz = new Label
            {
                Text = "Mapa de Celdas Activas (3 Pisos x 4 Puestos):",
                Location = new Point(20, 90),
                AutoSize = true
            };

            dgvMatriz = new DataGridView
            {
                Location = new Point(20, 115),
                Size = new Size(730, 320),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowTemplate = { Height = 55 }
            };

            for (int c = 0; c < 4; c++)
                dgvMatriz.Columns.Add($"Col{c}", $"Puesto {c}");

            tab.Controls.AddRange(new Control[] {
                lblBuscar, txtBuscarPlaca, btnBuscar, lblResultadoBusqueda,
                lblInfoMatriz, dgvMatriz
            });
        }

        private void ConstruirTabInforme(TabPage tab)
        {
            btnActualizarInforme = new Button
            {
                Text = "Actualizar Informe",
                Location = new Point(20, 20),
                Width = 180,
                Height = 32
            };
            btnActualizarInforme.Click += (s, e) => txtInforme.Text = gestor.GenerarInforme();

            txtInforme = new TextBox
            {
                Location = new Point(20, 65),
                Size = new Size(730, 370),
                Multiline = true,
                ReadOnly = true,
                Font = new Font("Consolas", 11)
            };

            tab.Controls.AddRange(new Control[] { btnActualizarInforme, txtInforme });
        }

        // Validaciones con ErrorProvider, MessageBox e if/switch (Requisito 2)
        private void BtnRegistrar_Click(object sender, EventArgs e)
        {
            errorProvider.Clear();
            bool valido = true;

            if (string.IsNullOrWhiteSpace(txtPlaca.Text) || txtPlaca.Text.Trim().Length < 5)
            {
                errorProvider.SetError(txtPlaca, "Ingrese una placa válida (mínimo 5 caracteres).");
                valido = false;
            }

            if (string.IsNullOrWhiteSpace(txtPropietario.Text))
            {
                errorProvider.SetError(txtPropietario, "El propietario es obligatorio.");
                valido = false;
            }

            if (!int.TryParse(txtHoras.Text, out int horas) || horas <= 0)
            {
                errorProvider.SetError(txtHoras, "Ingrese un número de horas mayor a 0.");
                valido = false;
            }

            if (!valido) return;

            int fila = (int)cmbFila.SelectedItem;
            int col = (int)cmbColumna.SelectedItem;
            Vehiculo nuevoVehiculo;

            switch (cmbTipo.SelectedItem.ToString())
            {
                case "Carro":
                    nuevoVehiculo = new Carro(txtPlaca.Text.Trim(), txtPropietario.Text.Trim(), fila, col, horas);
                    break;
                case "Moto":
                    nuevoVehiculo = new Moto(txtPlaca.Text.Trim(), txtPropietario.Text.Trim(), fila, col, horas);
                    break;
                default:
                    return;
            }

            string resultado = gestor.RegistrarIngreso(nuevoVehiculo);

            if (resultado == "OK")
            {
                MessageBox.Show("Vehículo registrado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtPlaca.Clear();
                txtPropietario.Clear();
                txtHoras.Clear();
                ActualizarVistas();
            }
            else
            {
                MessageBox.Show(resultado, "Registro Rechazado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtInforme.Text = gestor.GenerarInforme();
            }
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscarPlaca.Text))
            {
                MessageBox.Show("Escribe la placa que deseas buscar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Vehiculo encontrado = gestor.BuscarPorPlaca(txtBuscarPlaca.Text);

            if (encontrado != null)
            {
                lblResultadoBusqueda.ForeColor = Color.DarkGreen;
                lblResultadoBusqueda.Text = $"ENCONTRADO: {encontrado.ObtenerTipo()} | Propietario: {encontrado.Propietario} | Ubicación: [Piso {encontrado.Fila}, Puesto {encontrado.Columna}] | Total: ${encontrado.CalcularTarifa():N0}";
            }
            else
            {
                lblResultadoBusqueda.ForeColor = Color.Crimson;
                lblResultadoBusqueda.Text = $"No se encontró ningún vehículo activo con la placa '{txtBuscarPlaca.Text.ToUpper()}'.";
            }
        }

        private void ActualizarVistas()
        {
            dgvVehiculos.Rows.Clear();
            foreach (var v in gestor.HistorialVehiculos)
            {
                dgvVehiculos.Rows.Add(
                    v.ObtenerTipo(),
                    v.Placa,
                    v.Propietario,
                    $"[{v.Fila}, {v.Columna}]",
                    v.Horas,
                    $"${v.CalcularTarifa():N0}"
                );
            }

            dgvMatriz.Rows.Clear();
            for (int f = 0; f < 3; f++)
            {
                int rowIndex = dgvMatriz.Rows.Add();
                dgvMatriz.Rows[rowIndex].HeaderCell.Value = $"Piso {f}";
                for (int c = 0; c < 4; c++)
                {
                    string valor = gestor.MapaCeldas[f, c];
                    dgvMatriz.Rows[rowIndex].Cells[c].Value = valor;
                    dgvMatriz.Rows[rowIndex].Cells[c].Style.BackColor =
                        valor == "LIBRE" ? Color.LightGreen : Color.LightCoral;
                }
            }

            txtInforme.Text = gestor.GenerarInforme();
        }
    }
}
