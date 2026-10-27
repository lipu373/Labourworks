using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labourworks.Domain.Entities
{
    public class PlantMaster
    {
        [Key]
        public int PlantId { get; set; }
      
        public string PlantName { get; set; } = string.Empty;
       
        public bool IsActive { get; set; } = true;
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public int? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
