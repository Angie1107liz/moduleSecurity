﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dto
{
    public class ModuleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Boolean State { get; set; }
        public string Description { get; set; }
    }
}