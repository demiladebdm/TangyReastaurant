using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tangy.Models
{
    public class Coupons
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string CouponType { get; set; }
        public enum ECouponType { Percentage = 0, Dollar = 1 }

        [Required]
        public double Discount { get; set; }

        [Required]
        [DisplayName("Minimum Amount")]
        public double MinimumAmount { get; set; }
        public byte[]? Picture { get; set; }
        public bool isActive { get; set; }
    }
}
