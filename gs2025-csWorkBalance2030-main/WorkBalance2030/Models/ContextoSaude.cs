using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBalance2030.Models
{
    public class ContextoSaude
    {
        public bool TemFilhosPequenos { get; set; }
        public bool TrabalhaEmTurnosNoturnos { get; set; }
        public int NivelPressaoPorMetas { get; set; } // 0 a 10
        public bool TemApoioPsicologico { get; set; }

        public ContextoSaude()
        {
            NivelPressaoPorMetas = 0;
        }
    }
}
