﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataSystem.Models.Analysis
{
    [Table("totalpivotcombinedp_dist_finalm")]
    public class totalpivotcombinedpdistm
    {
        [Key]
        public string Id { get; set; } 
        public string ProvinceId { get; set; }
        public string Province { get; set; }
        public string DistrictId { get; set; }
        public string District { get; set; }
        public int FacilityTypeId { get; set; }
        public string FacilityType { get; set; }
        public int IndicatorId { get; set; }
        public string IndicatorName { get; set; }
        public string Implementer { get; set; }
        public string Module { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Quarter { get; set; }
        public int Num { get; set; }
        public int Denom { get; set; }
    }
}
