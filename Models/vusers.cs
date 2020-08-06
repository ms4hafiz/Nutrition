﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models
{
    public class vusers
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        public int TenantId { get; set; }
    }
}
