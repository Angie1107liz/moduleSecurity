﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dto
{
    public class ViewDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Route {  get; set; }
        public int Module_id {  get; set; }
        public string Modulo {  get; set; }
        public Boolean State {  get; set; }
    }
}