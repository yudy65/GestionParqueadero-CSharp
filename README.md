# Sistema de Gestión de Parqueadero (ParkControl C#)

Aplicación de escritorio desarrollada en **C# con Windows Forms (100% por código)** para administrar el ingreso, ubicación y cobro de vehículos en un parqueadero.

## Integrantes
* [Tu Nombre Completo]
* [Nombre de tu Compañero/a] *(eliminar si es individual)*

## Problema a resolver
En parqueaderos pequeños que llevan su control de forma manual, es frecuente asignar por error un mismo espacio a dos vehículos, perder trazabilidad de los registros al cerrar el turno y cometer errores al calcular tarifas diferenciadas entre carros y motos. 

Esta aplicación automatiza la asignación de celdas mediante un mapa visual, valida duplicados y cupos en tiempo real, calcula tarifas mediante polimorfismo y guarda los registros en un archivo de texto plano para evitar pérdida de información.

## Diagrama UML de Clases

```mermaid
classDiagram
    class Vehiculo {
        <<abstract>>
        +string Placa
        +string Propietario
        +int Fila
        +int Columna
        +int Horas
        +Vehiculo(placa, propietario, fila, columna, horas)
        +virtual double CalcularTarifa()
        +abstract string ObtenerTipo()
        +string AFormatoArchivo()
    }

    class Carro {
        +Carro(placa, propietario, fila, columna, horas)
        +override double CalcularTarifa()
        +override string ObtenerTipo()
    }

    class Moto {
        +Moto(placa, propietario, fila, columna, horas)
        +override double CalcularTarifa()
        +override string ObtenerTipo()
    }

    class GestorParqueadero {
        +List~Vehiculo~ HistorialVehiculos
        +string[,] MapaCeldas
        +int IntentosRechazados
        -string rutaArchivo
        +GestorParqueadero()
        +string RegistrarIngreso(Vehiculo v)
        +string GenerarInforme()
        +void GuardarEnArchivo()
        +void CargarDesdeArchivo()
    }

    Vehiculo <|-- Carro
    Vehiculo <|-- Moto
    GestorParqueadero o-- Vehiculo : administra
