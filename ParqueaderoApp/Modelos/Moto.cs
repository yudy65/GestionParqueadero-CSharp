namespace ParqueaderoApp.Modelos
{
    public class Moto : Vehiculo
    {
        public Moto(string placa, string propietario, int fila, int columna, int horas)
            : base(placa, propietario, fila, columna, horas) { }

        public override double CalcularTarifa()
        {
            // $1.800 por hora sin recargo
            return Horas * 1800;
        }

        public override string ObtenerTipo() => "Moto";
    }
}

