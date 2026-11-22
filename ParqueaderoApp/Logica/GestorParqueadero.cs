using System;
using System.Collections.Generic;
using System.IO;
using ParqueaderoApp.Modelos;

namespace ParqueaderoApp.Logica
{
    public class GestorParqueadero
    {
        // Requisito 5: Lista y Matriz usadas con sentido en el problema
        public List<Vehiculo> HistorialVehiculos { get; private set; }
        public string[,] MapaCeldas { get; private set; } // 3 pisos x 4 puestos
        public int IntentosRechazados { get; private set; }
        private readonly string rutaArchivo = "datos_parqueadero.txt";

        public GestorParqueadero()
        {
            HistorialVehiculos = new List<Vehiculo>();
            MapaCeldas = new string[3, 4];
            InicializarMatriz();
            CargarDesdeArchivo();
        }

        private void InicializarMatriz()
        {
            for (int f = 0; f < 3; f++)
            {
                for (int c = 0; c < 4; c++)
                {
                    MapaCeldas[f, c] = "LIBRE";
                }
            }
        }

        public string RegistrarIngreso(Vehiculo v)
        {
            if (MapaCeldas[v.Fila, v.Columna] != "LIBRE")
            {
                IntentosRechazados++;
                return $"La celda [Piso {v.Fila}, Puesto {v.Columna}] ya está ocupada por {MapaCeldas[v.Fila, v.Columna]}.";
            }

            // Requisito 6: Uso de bandera para validar placa duplicada
            bool placaDuplicada = false;
            foreach (var item in HistorialVehiculos)
            {
                if (item.Placa == v.Placa)
                {
                    placaDuplicada = true;
                    break;
                }
            }

            if (placaDuplicada)
            {
                IntentosRechazados++;
                return "Ya existe un vehículo activo registrado con esa placa.";
            }

            MapaCeldas[v.Fila, v.Columna] = v.Placa;
            HistorialVehiculos.Add(v);
            GuardarEnArchivo();
            return "OK";
        }

        // Funcionalidad 2: Búsqueda rápida por placa usando ciclo y bandera
        public Vehiculo BuscarPorPlaca(string placaBuscada)
        {
            string placaLimpia = placaBuscada.Trim().ToUpper();
            bool encontrado = false;
            Vehiculo resultado = null;

            foreach (var v in HistorialVehiculos)
            {
                if (v.Placa == placaLimpia)
                {
                    encontrado = true;
                    resultado = v;
                    break;
                }
            }

            return encontrado ? resultado : null;
        }

        // Requisito 6: Informe usando ciclos, contadores, acumuladores y banderas
        public string GenerarInforme()
        {
            int contCarros = 0;          // Contador
            int contMotos = 0;           // Contador
            double totalRecaudado = 0;   // Acumulador
            bool hayCuposLibres = false; // Bandera

            foreach (Vehiculo v in HistorialVehiculos)
            {
                totalRecaudado += v.CalcularTarifa(); // Polimorfismo
                if (v is Carro) contCarros++;
                else if (v is Moto) contMotos++;
            }

            for (int f = 0; f < 3; f++)
            {
                for (int c = 0; c < 4; c++)
                {
                    if (MapaCeldas[f, c] == "LIBRE")
                    {
                        hayCuposLibres = true;
                    }
                }
            }

            string estadoCupos = hayCuposLibres ? "Disponible (aún hay puestos libres)" : "¡PARQUEADERO LLENO!";

            return "=== INFORME GENERAL DEL PARQUEADERO ===\r\n\r\n" +
                   $"• Total vehículos válidos: {HistorialVehiculos.Count}\r\n" +
                   $"   - Carros registrados: {contCarros}\r\n" +
                   $"   - Motos registradas: {contMotos}\r\n" +
                   $"• Registros rechazados (cupo/duplicado): {IntentosRechazados}\r\n" +
                   $"• Total dinero recaudado: ${totalRecaudado:N0}\r\n" +
                   $"• Estado de ocupación: {estadoCupos}";
        }

        // Requisito 7: Guardar y cargar en archivo de texto
        public void GuardarEnArchivo()
        {
            using (StreamWriter sw = new StreamWriter(rutaArchivo, false))
            {
                foreach (var v in HistorialVehiculos)
                {
                    sw.WriteLine(v.AFormatoArchivo());
                }
            }
        }

        public void CargarDesdeArchivo()
        {
            if (!File.Exists(rutaArchivo)) return;

            HistorialVehiculos.Clear();
            InicializarMatriz();

            foreach (string linea in File.ReadAllLines(rutaArchivo))
            {
                string[] partes = linea.Split(';');
                if (partes.Length == 6)
                {
                    string tipo = partes[0];
                    string placa = partes[1];
                    string prop = partes[2];
                    int fila = int.Parse(partes[3]);
                    int col = int.Parse(partes[4]);
                    int horas = int.Parse(partes[5]);

                    Vehiculo v = tipo == "Carro"
                        ? new Carro(placa, prop, fila, col, horas)
                        : new Moto(placa, prop, fila, col, horas);

                    HistorialVehiculos.Add(v);
                    MapaCeldas[fila, col] = placa;
                }
            }
        }
    }
}
