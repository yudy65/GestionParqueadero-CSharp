namespace ParqueaderoApp.Modelos
{
    public class Carro : Vehiculo
    {
        public Carro(string placa, string propietario, int fila, int columna, int horas)
            : base(placa, propietario, fila, columna, horas) { }

        public override double CalcularTarifa()
        {
            // $3.500 por hora + $1.000 de recargo fijo
            return (Horas * 3500) + 1000;
        }

        public override string ObtenerTipo() => "Carro";
    }
}
