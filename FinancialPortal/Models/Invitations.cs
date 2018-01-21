using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinancialPortal.Models
{
    public class Invitations
    {
        public int Id { get; set; }
        public int? HouseholdId { get; set; }

        [Required(ErrorMessage = "Email for invited member required!")]
        public string InviteEmail { get; set; }

        [Display(Name = "Invite Code")]
        public string InvCd { get; set; }
        public bool IsRegistered { get; set; }

        [Display(Name ="Invite Created")]
        public DateTimeOffset InviteCreated { get; set; }

        [Display(Name ="Invited Modified")]
        public DateTimeOffset InviteModified { get; set; }

        [Display(Name ="Invite Expiration")]
        public DateTimeOffset InviteExpiration { get; set; }

        //-- Navigation Property --//
        public virtual Households Household { get; set; }
    }
}