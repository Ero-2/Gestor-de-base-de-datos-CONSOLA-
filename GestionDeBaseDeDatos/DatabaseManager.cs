using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeBaseDeDatos
{
    internal class DatabaseManager
    {
        private SqlConnection connection;

        private string connectionString = "Server=DESKTOP-T61VTOI\\SQLEXPRESS; Database=PowerliftingAtletass; Integrated Security=True;";

        public DatabaseManager()
        {
            // Inicializa la conexión con la cadena de conexión
            connection = new SqlConnection(connectionString);
        }

        private string EncryptPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Calcula el hash de la contraseña
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convierte el hash a un string hexadecimal
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString(); // Retorna la contraseña encriptada
            }
        }

        public void CrearUsuario(string usuario, string contraseña, string rol)
        {
            // Encriptamos la contraseña
            string encryptedPassword = EncryptPassword(contraseña);

            // Consultas SQL para insertar un nuevo usuario
            string query = "INSERT INTO usuario (usuario, contraseña, rol, fecha_creacion) " +
                           "VALUES (@usuario, @contraseña, @rol, @fecha_creacion)";

            try
            {
                // Conexión a la base de datos
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Parametrizamos la consulta para evitar inyección SQL
                        command.Parameters.AddWithValue("@usuario", usuario);
                        command.Parameters.AddWithValue("@contraseña", encryptedPassword);
                        command.Parameters.AddWithValue("@rol", rol);
                        command.Parameters.AddWithValue("@fecha_creacion", DateTime.Now);

                        // Ejecutamos la consulta
                        int result = command.ExecuteNonQuery();

                        // Verificamos si la inserción fue exitosa
                        if (result > 0)
                        {
                            Console.WriteLine("Usuario creado con éxito.");
                        }
                        else
                        {
                            Console.WriteLine("Hubo un problema al crear el usuario.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        // Método principal para recibir datos desde la consola
        public void CrearUsuarioDesdeConsola()
        {
            Console.Write("Introduce el nombre de usuario: ");
            string usuario = Console.ReadLine();

            Console.Write("Introduce la contraseña: ");
            string contraseña = Console.ReadLine();

            Console.Write("Introduce el rol (admin, user, etc.): ");
            string rol = Console.ReadLine();

            // Crear usuario
            CrearUsuario(usuario, contraseña, rol);
        }


        public bool IniciarSesion(string usuario, string contraseña)
        {
            // Encripta la contraseña ingresada para compararla con la almacenada en la base de datos
            string encryptedPassword = EncryptPassword(contraseña);

            // Consulta SQL para verificar las credenciales
            string query = "SELECT COUNT(1) FROM usuario WHERE usuario = @usuario AND contraseña = @contraseña";

            try
            {
                // Conexión a la base de datos
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Parametrizamos la consulta
                        command.Parameters.AddWithValue("@usuario", usuario);
                        command.Parameters.AddWithValue("@contraseña", encryptedPassword);

                        // Ejecutamos la consulta y verificamos si existe un usuario con esas credenciales
                        int count = Convert.ToInt32(command.ExecuteScalar());

                        if (count == 1)
                        {
                            Console.WriteLine("Inicio de sesión exitoso.");
                            return true; // Credenciales válidas
                        }
                        else
                        {
                            Console.WriteLine("Usuario o contraseña incorrectos.");
                            return false; // Credenciales inválidas
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false; // Manejo de errores
            }
        }

        public void CrearEntrenamiento(int idPowerlifter, int idRutina, decimal duracion, string sensaciones, string notas)
        {

            try
            {
                // Abre la conexión antes de ejecutar cualquier comando
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }



                // Ahora que sabemos que el id_powerlifter existe, procedemos con la inserción del entrenamiento
                string query = "INSERT INTO entrenamiento (id_powerlifter, id_rutina, duracion, sensaciones, notas) " +
                      "VALUES (@idPowerlifter, @idRutina, @duracion, @sensaciones, @notas)";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@idPowerlifter", idPowerlifter);
                    cmd.Parameters.AddWithValue("@idRutina", idRutina);
                    cmd.Parameters.AddWithValue("@duracion", duracion);
                    cmd.Parameters.AddWithValue("@sensaciones", sensaciones);
                    cmd.Parameters.AddWithValue("@notas", notas);

                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Entrenamiento creado exitosamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al crear el entrenamiento: " + ex.Message);
            }
            finally
            {
                // Cerrar la conexión si está abierta
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public List<Entrenamiento> MostrarEntrenamientos()
        {
            List<Entrenamiento> entrenamientos = new List<Entrenamiento>();

            try
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }

                string query = "SELECT ID_entrenamiento, id_powerlifter, id_rutina, duracion, sensaciones, notas FROM entrenamiento";

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Entrenamiento entrenamiento = new Entrenamiento
                    {
                        ID_entrenamiento = reader.GetInt32(0),
                        id_powerlifter = reader.GetInt32(1),
                        id_rutina = reader.GetInt32(2),
                        duracion = reader.GetDecimal(3),
                        sensaciones = reader.GetString(4),
                        notas = reader.GetString(5)
                    };
                    entrenamientos.Add(entrenamiento);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return entrenamientos;
        }

        public void ActualizarEntrenamiento(int idEntrenamiento, decimal duracion, string sensaciones, string notas)
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }

                string query = "UPDATE entrenamiento SET duracion = @duracion, sensaciones = @sensaciones, notas = @notas WHERE ID_entrenamiento = @idEntrenamiento";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@idEntrenamiento", idEntrenamiento);
                command.Parameters.AddWithValue("@duracion", duracion);
                command.Parameters.AddWithValue("@sensaciones", sensaciones);
                command.Parameters.AddWithValue("@notas", notas);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void EliminarEntrenamiento(int idEntrenamiento)
        {
            try
            {
                // Abre la conexión si no está abierta
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }

                // Eliminar los registros de ejercicio relacionados con el entrenamiento
                string deleteEjerciciosQuery = "DELETE FROM ejercicio WHERE id_entrenamiento = @idEntrenamiento";
                using (SqlCommand cmd = new SqlCommand(deleteEjerciciosQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@idEntrenamiento", idEntrenamiento);
                    cmd.ExecuteNonQuery();  // Ejecutar la eliminación de los ejercicios
                }

                // Ahora eliminar el entrenamiento
                string deleteEntrenamientoQuery = "DELETE FROM entrenamiento WHERE ID_entrenamiento = @idEntrenamiento";
                using (SqlCommand cmd = new SqlCommand(deleteEntrenamientoQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@idEntrenamiento", idEntrenamiento);
                    cmd.ExecuteNonQuery();  // Ejecutar la eliminación del entrenamiento
                }

                Console.WriteLine("Entrenamiento y ejercicios eliminados correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                // Cerrar la conexión
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public void EliminarEjercicioYEntrenamiento(int idEntrenamiento)
        {
            try
            {
                // Asegurarse de que la conexión esté abierta
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }

                // Primero eliminar los ejercicios relacionados con el entrenamiento
                string deleteEjercicioQuery = "DELETE FROM ejercicio WHERE id_entrenamiento = @idEntrenamiento";
                SqlCommand cmdDeleteEjercicio = new SqlCommand(deleteEjercicioQuery, connection);
                cmdDeleteEjercicio.Parameters.AddWithValue("@idEntrenamiento", idEntrenamiento);
                int rowsAffectedEjercicio = cmdDeleteEjercicio.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffectedEjercicio} ejercicio(s) eliminado(s).");

                // Ahora eliminar el entrenamiento
                string deleteEntrenamientoQuery = "DELETE FROM entrenamiento WHERE ID_entrenamiento = @idEntrenamiento";
                SqlCommand cmdDeleteEntrenamiento = new SqlCommand(deleteEntrenamientoQuery, connection);
                cmdDeleteEntrenamiento.Parameters.AddWithValue("@idEntrenamiento", idEntrenamiento);
                int rowsAffectedEntrenamiento = cmdDeleteEntrenamiento.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffectedEntrenamiento} entrenamiento(s) eliminado(s).");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                // Cerrar la conexión
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }


        public void CrearEjercicio(int idEntrenamiento, string tipoMovimiento, int sets, int reps, decimal cargaUtilizada, int descansoEntreSeries, string tempo, int rpe, int rir, string notas)
        {
            // Asegúrate de que la conexión esté abierta
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }

            try
            {
                // Consulta SQL para insertar un nuevo ejercicio
                string query = "INSERT INTO ejercicio (id_entrenamiento, tipoMovimiento, setss, reps, cargaUtilizada, descansoEntreSeries, tempo, RPE, RIR, notas) " +
                               "VALUES (@idEntrenamiento, @tipoMovimiento, @sets, @reps, @cargaUtilizada, @descansoEntreSeries, @tempo, @rpe, @rir, @notas)";

                // Ejecutar el comando
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idEntrenamiento", idEntrenamiento);
                    command.Parameters.AddWithValue("@tipoMovimiento", tipoMovimiento);
                    command.Parameters.AddWithValue("@sets", sets);
                    command.Parameters.AddWithValue("@reps", reps);
                    command.Parameters.AddWithValue("@cargaUtilizada", cargaUtilizada);
                    command.Parameters.AddWithValue("@descansoEntreSeries", descansoEntreSeries);
                    command.Parameters.AddWithValue("@tempo", tempo);
                    command.Parameters.AddWithValue("@rpe", rpe);
                    command.Parameters.AddWithValue("@rir", rir);
                    command.Parameters.AddWithValue("@notas", notas);

                    // Ejecutar el comando
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al crear el ejercicio: " + ex.Message);
            }
            finally
            {
                // Asegurarse de cerrar la conexión
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public List<Ejercicio> MostrarEjercicios()
        {
            List<Ejercicio> ejercicios = new List<Ejercicio>();

            try
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }

                string query = "SELECT ID_ejercicio, id_entrenamiento, tipoMovimiento, setss, reps, cargaUtilizada, descansoEntreSeries, tempo, RPE, RIR, notas FROM ejercicio";
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Ejercicio ejercicio = new Ejercicio
                    {
                        ID_ejercicio = reader.GetInt32(0),
                        id_entrenamiento = reader.GetInt32(1),
                        tipoMovimiento = reader.GetString(2),
                        setss = reader.GetInt32(3),
                        reps = reader.GetInt32(4),
                        cargaUtilizada = reader.GetDecimal(5),
                        descansoEntreSeries = reader.GetInt32(6),
                        tempo = reader.GetString(7),
                        RPE = reader.GetInt32(8),
                        RIR = reader.GetInt32(9),
                        notas = reader.GetString(10)
                    };
                    ejercicios.Add(ejercicio);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al mostrar los ejercicios: " + ex.Message);
            }

            return ejercicios;
        }

        public void ActualizarEjercicio(int idEjercicio, string tipoMovimiento, int sets, int reps, decimal cargaUtilizada, int descansoEntreSeries, string tempo, int rpe, int rir, string notas)
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }

                string query = "UPDATE ejercicio SET tipoMovimiento = @tipoMovimiento, setss = @sets, reps = @reps, cargaUtilizada = @cargaUtilizada, descansoEntreSeries = @descansoEntreSeries, tempo = @tempo, RPE = @rpe, RIR = @rir, notas = @notas WHERE ID_ejercicio = @idEjercicio";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@tipoMovimiento", tipoMovimiento);
                cmd.Parameters.AddWithValue("@sets", sets);
                cmd.Parameters.AddWithValue("@reps", reps);
                cmd.Parameters.AddWithValue("@cargaUtilizada", cargaUtilizada);
                cmd.Parameters.AddWithValue("@descansoEntreSeries", descansoEntreSeries);
                cmd.Parameters.AddWithValue("@tempo", tempo);
                cmd.Parameters.AddWithValue("@rpe", rpe);
                cmd.Parameters.AddWithValue("@rir", rir);
                cmd.Parameters.AddWithValue("@notas", notas);
                cmd.Parameters.AddWithValue("@idEjercicio", idEjercicio);

                cmd.ExecuteNonQuery();  // Ejecutar el UPDATE
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar el ejercicio: " + ex.Message);
            }
        }

        public void EliminarEjercicio(int idEjercicio)
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }

                string query = "DELETE FROM ejercicio WHERE ID_ejercicio = @idEjercicio";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@idEjercicio", idEjercicio);

                cmd.ExecuteNonQuery();  // Ejecutar el DELETE
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al eliminar el ejercicio: " + ex.Message);
            }
        }






    }
}
