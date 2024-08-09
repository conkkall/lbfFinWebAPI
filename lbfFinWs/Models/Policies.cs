using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace lbfFinWs.Models
{
    public class PoliciesModel
    {
        [Key]
        public int PolicyId { get; set; }
        public string PolicyName { get; set; }
        public string PolicyAmount { get; set; }
        public int MaturityMonths { get; set; }
        public string MonthlyReturn { get; set; }
        public int MonthlyPoints { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
    }
}
