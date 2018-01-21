using System;
using System.ComponentModel.DataAnnotations;

namespace FinancialPortal.Models
{
    public class Transactions
    {
        public int Id { get; set; }
        public int AccountId { get; set; }

        [Display(Name="Transaction Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset CreateDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? ModifyDate { get; set; }

        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
        public int TransactionTypeId { get; set; }
        public int CategoryId { get; set; }

        [MaxLength(50)]
        public string Description { get; set; }
        public string EnteredById { get; set; }
        public bool IsReconciled { get; set; }

        //[DataType(DataType.Currency)]
        //public decimal ReconciledAmount { get; set; }
        public bool ExcludeBudgetItem { get; set; }
        public bool IsVoid { get; set; }
        [DataType(DataType.Currency)]
        public decimal VoidAmount { get; set; }
        public bool Deleted { get; set; }
       
        // -- Navigation properties -- //
        public virtual Accounts Account { get; set; }  
        public virtual Categories Category { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual TransactionTypes TransactionType { get; set; }
        
    }
}