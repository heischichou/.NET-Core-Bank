using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CIS2103_ASP.NET.Models
{
    public class BankAccount
    {
        [Required(ErrorMessage = "Account number is required.")]
        [StringLength(12, ErrorMessage = "Account number must not exceed 12 digits long.")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Account number must consist of whole numbers only.")]
        [Display(Name = "Account Number")]
        public string Account_No { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]
        [Display(Name = "First Name")]
        public string Holder_Fname { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]
        [Display(Name = "Last Name")]
        public string Holder_Lname { get; set; }

        public decimal Balance;

        [Required(ErrorMessage = "PIN is required.")]
        [StringLength(6, ErrorMessage = "PIN must not exceed 6 digits long.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "PIN must consist of whole numbers only.")]
        public string PIN { get; set; }

        [Compare("PIN", ErrorMessage = "PINs must match.")]
        [Display(Name = "PIN")]
        public string ConfirmPIN { get; set; }
    }
    public enum Type
    {
        Deposit,
        Withdraw,
        Transfer
    }
    public class TransactionEntry
    {
        [Required(ErrorMessage = "PIN is required.")]
        [StringLength(6, ErrorMessage = "PIN must not exceed 6 digits long.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "PIN must consist of whole numbers only.")]
        public string PIN { get; set; }

        [Compare("PIN", ErrorMessage = "PINs must match.")]
        [Display(Name = "PIN")]
        public string ConfirmPIN { get; set; }

        [Required]
        public DateTime Date => DateTime.Now;

        [Required(ErrorMessage = "Amount is required.")]
        public float Amount { get; set; }

        [Required]
        public Type Type { get; set; }
    }
    public class TransferEntry
    {
        [Required(ErrorMessage = "PIN is required.")]
        [StringLength(6, ErrorMessage = "PIN must not exceed 6 digits long.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "PIN must consist of whole numbers only.")]
        public string PIN { get; set; }

        [Compare("PIN", ErrorMessage = "PINs must match.")]
        [Display(Name = "PIN")]
        public string ConfirmPIN { get; set; }

        [Required(ErrorMessage = "Receiver account number is required.")]
        [StringLength(12, ErrorMessage = "Receiver account number must not exceed 12 digits long.")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Receiver account number must consist of whole numbers only.")]
        [Display(Name = "Receiver Account Number")]
        public string Receiver_No { get; set; }

        [Required]
        public DateTime Date => DateTime.Now;

        [Required(ErrorMessage = "Amount is required.")]
        public float Amount { get; set; }

        [Required]
        public Type Type { get; set; }
    }
}