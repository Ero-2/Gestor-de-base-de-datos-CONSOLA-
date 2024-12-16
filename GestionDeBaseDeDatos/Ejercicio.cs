using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeBaseDeDatos
{
    internal class Ejercicio
    {
        public int ID_ejercicio { get; set; }
        public int id_entrenamiento { get; set; }
        public string tipoMovimiento { get; set; }
        public int setss { get; set; }
        public int reps { get; set; }
        public decimal cargaUtilizada { get; set; }
        public int descansoEntreSeries { get; set; }
        public string tempo { get; set; }
        public int RPE { get; set; }
        public int RIR { get; set; }
        public string notas { get; set; }
    }
}
