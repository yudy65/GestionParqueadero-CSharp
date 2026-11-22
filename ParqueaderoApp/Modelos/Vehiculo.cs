using System;

namespace ParqueaderoApp.Modelos
{
    public abstract class Vehiculo
    {
        public string Placa { get; set; }
        public string Propietario { get; set; }
        public int Fila { get; set; }
        public int Columna { get; set; }
        public int Horas { get; set; }

        public Vehiculo(string placa, string propietario, int fila, int columna, int horas)
        {
            Placa = placa.ToUpper();
            Propietario = propietario;
            Fila = fila;
            Columna = columna;
            Horas = horas;
        }

        // Método virtual que será sobrescrito (Polimorfismo - Requisito 3)
        public virtual double CalcularTarifa()
        {
            return Horas * 2000;
        }

        public abstract string ObtenerTipo();

        public string AFormatoArchivo()
        {
            return $"{ObtenerTipo()};{Placa};{Propietario};{Fila};{Columna};{Horas}";
        }
    }
}
