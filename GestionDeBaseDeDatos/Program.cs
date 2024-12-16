using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Security.Cryptography;


namespace GestionDeBaseDeDatos
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DatabaseManager dbManager = new DatabaseManager();

            while (true)
            {
                Console.Clear(); // Limpia la consola para una mejor visualización
                Console.WriteLine("Seleccione una opción:");
                Console.WriteLine("1. Crear usuario");
                Console.WriteLine("2. Iniciar sesión");
                Console.WriteLine("3. Crear entrenamiento");
                Console.WriteLine("4. Mostrar entrenamientos");
                Console.WriteLine("5. Crear ejercicio");
                Console.WriteLine("6. Mostrar ejercicios");
                Console.WriteLine("7. Actualizar ejercicio");
                Console.WriteLine("8. Eliminar ejercicio");
                string oppcion = Console.ReadLine();

                if (oppcion == "4")
                {
                    // Mostrar entrenamientos
                    List<Entrenamiento> entrenamientos = dbManager.MostrarEntrenamientos();

                    foreach (var entrenamiento in entrenamientos)
                    {
                        Console.WriteLine($"ID: {entrenamiento.ID_entrenamiento}, Powerlifter ID: {entrenamiento.ID_powerlifter}, " +
                            $"Rutina ID: {entrenamiento.id_rutina}, Duración: {entrenamiento.duracion}, Sensaciones: {entrenamiento.sensaciones}");
                    }
                }
                else if (oppcion == "0")
                {
                    break;
                }

                if (oppcion == "5")
                {
                    // Crear ejercicio
                    Console.WriteLine("Ingrese ID de entrenamiento:");
                    int idEntrenamiento = int.Parse(Console.ReadLine());
                    Console.WriteLine("Ingrese tipo de movimiento:");
                    string tipoMovimiento = Console.ReadLine();
                    Console.WriteLine("Ingrese sets:");
                    int sets = int.Parse(Console.ReadLine());
                    Console.WriteLine("Ingrese reps:");
                    int reps = int.Parse(Console.ReadLine());
                    Console.WriteLine("Ingrese carga utilizada:");
                    decimal cargaUtilizada = decimal.Parse(Console.ReadLine());
                    Console.WriteLine("Ingrese descanso entre series:");
                    int descansoEntreSeries = int.Parse(Console.ReadLine());
                    Console.WriteLine("Ingrese tempo:");
                    string tempo = Console.ReadLine();
                    Console.WriteLine("Ingrese RPE:");
                    int rpe = int.Parse(Console.ReadLine());
                    Console.WriteLine("Ingrese RIR:");
                    int rir = int.Parse(Console.ReadLine());
                    Console.WriteLine("Ingrese notas:");
                    string notas = Console.ReadLine();

                    dbManager.CrearEjercicio(idEntrenamiento, tipoMovimiento, sets, reps, cargaUtilizada, descansoEntreSeries, tempo, rpe, rir, notas);
                }

                else if (oppcion == "6")
                {
                    // Mostrar ejercicios
                    List<Ejercicio> ejercicios = dbManager.MostrarEjercicios();
                    foreach (var ejercicio in ejercicios)
                    {
                        Console.WriteLine($"ID: {ejercicio.ID_ejercicio}, Tipo de Movimiento: {ejercicio.tipoMovimiento}, Sets: {ejercicio.setss}, Reps: {ejercicio.reps}");
                    }
                }

                else if (oppcion == "7")
                {
                    // Actualizar ejercicio
                    Console.WriteLine("Ingrese ID del ejercicio a actualizar:");
                    int idEjercicio = int.Parse(Console.ReadLine());
                    Console.WriteLine("Ingrese tipo de movimiento actualizado:");
                    string tipoMovimiento = Console.ReadLine();
                    Console.WriteLine("Ingrese sets:");
                    int sets = int.Parse(Console.ReadLine());
                    Console.WriteLine("Ingrese reps:");
                    int reps = int.Parse(Console.ReadLine());
                    Console.WriteLine("Ingrese carga utilizada:");
                    decimal cargaUtilizada = decimal.Parse(Console.ReadLine());
                    Console.WriteLine("Ingrese descanso entre series:");
                    int descansoEntreSeries = int.Parse(Console.ReadLine());
                    Console.WriteLine("Ingrese tempo:");
                    string tempo = Console.ReadLine();
                    Console.WriteLine("Ingrese RPE:");
                    int rpe = int.Parse(Console.ReadLine());
                    Console.WriteLine("Ingrese RIR:");
                    int rir = int.Parse(Console.ReadLine());
                    Console.WriteLine("Ingrese notas:");
                    string notas = Console.ReadLine();

                    dbManager.ActualizarEjercicio(idEjercicio, tipoMovimiento, sets, reps, cargaUtilizada, descansoEntreSeries, tempo, rpe, rir, notas);
                }

                else if (oppcion == "8")
                {
                    // Eliminar ejercicio
                    Console.WriteLine("Ingrese ID del ejercicio a eliminar:");
                    int idEjercicio = int.Parse(Console.ReadLine());
                    dbManager.EliminarEjercicio(idEjercicio);
                }


                else if (oppcion == "0")
                {
                    break; // Salir del programa
                }

                // Leer la opción elegida
                string opcion = Console.ReadLine();

                // Asegurarse de que la opción es válida
                switch (opcion)
                {
                    case "1":
                        CrearUsuario(dbManager);
                        break;
                    case "2":
                        IniciarSesion(dbManager);
                        break;
                    case "3":
                        CrearEntrenamiento(dbManager);
                        break;
                    case "4":
                        MostrarEntrenamientos(dbManager);
                        break;
                    case "5":
                        CrearEjercicio(dbManager);
                        break;
                    case "6":
                        MostrarEjercicios(dbManager);
                        break;
                    case "0":
                        Console.WriteLine("Saliendo...");
                        return; // Salir del programa
                    default:
                        Console.WriteLine("Opción no válida, intente nuevamente.");
                        break;
                }
            }
        }

        // Funciones para manejar las opciones
        private static void CrearUsuario(DatabaseManager dbManager)
        {
            Console.WriteLine("Ingrese nombre de usuario:");
            string usuario = Console.ReadLine();
            Console.WriteLine("Ingrese la contraseña:");
            string contraseña = Console.ReadLine();
            Console.WriteLine("Ingrese el rol del usuario:");
            string rol = Console.ReadLine();
            dbManager.CrearUsuario(usuario, contraseña, rol);
        }

        private static void IniciarSesion(DatabaseManager dbManager)
        {
            Console.WriteLine("Ingrese nombre de usuario:");
            string usuario = Console.ReadLine();
            Console.WriteLine("Ingrese la contraseña:");
            string contraseña = Console.ReadLine();
            dbManager.IniciarSesion(usuario, contraseña);
        }

        private static void CrearEntrenamiento(DatabaseManager dbManager)
        {
            Console.WriteLine("Ingrese ID de Powerlifter:");
            int idPowerlifter = int.Parse(Console.ReadLine());
            Console.WriteLine("Ingrese ID de Rutina:");
            int idRutina = int.Parse(Console.ReadLine());
            Console.WriteLine("Ingrese duración (en minutos):");
            decimal duracion = decimal.Parse(Console.ReadLine());
            Console.WriteLine("Ingrese sensaciones:");
            string sensaciones = Console.ReadLine();
            Console.WriteLine("Ingrese notas:");
            string notas = Console.ReadLine();

            dbManager.CrearEntrenamiento(idPowerlifter, idRutina, duracion, sensaciones, notas);
        }

        private static void MostrarEntrenamientos(DatabaseManager dbManager)
        {
            dbManager.MostrarEntrenamientos();
        }

        private static void CrearEjercicio(DatabaseManager dbManager)
        {
            Console.WriteLine("Ingrese ID de entrenamiento:");
            int idEntrenamiento = int.Parse(Console.ReadLine());
            Console.WriteLine("Ingrese tipo de movimiento:");
            string tipoMovimiento = Console.ReadLine();
            Console.WriteLine("Ingrese número de sets:");
            int sets = int.Parse(Console.ReadLine());
            Console.WriteLine("Ingrese número de reps:");
            int reps = int.Parse(Console.ReadLine());
            Console.WriteLine("Ingrese carga utilizada:");
            decimal cargaUtilizada = decimal.Parse(Console.ReadLine());
            Console.WriteLine("Ingrese descanso entre series (en segundos):");
            int descanso = int.Parse(Console.ReadLine());
            Console.WriteLine("Ingrese tempo:");
            string tempo = Console.ReadLine();
            Console.WriteLine("Ingrese RPE:");
            int rpe = int.Parse(Console.ReadLine());
            Console.WriteLine("Ingrese RIR:");
            int rir = int.Parse(Console.ReadLine());
            Console.WriteLine("Ingrese notas:");
            string notas = Console.ReadLine();

            dbManager.CrearEjercicio(idEntrenamiento, tipoMovimiento, sets, reps, cargaUtilizada, descanso, tempo, rpe, rir, notas);
        }

        private static void MostrarEjercicios(DatabaseManager dbManager)
        {
            dbManager.MostrarEjercicios();
        }

    }



}



