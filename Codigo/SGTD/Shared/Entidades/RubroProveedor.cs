﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Entidades
{
    public class RubroProveedor : EntidadBase
    {
        public Rubro Rubro { get; set; }
        public Proveedor Proveedor { get; set; }
    }
}
