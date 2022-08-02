using CIS1203_ASP.NET;
using CIS2103_ASP.NET.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CIS2103_ASP.NET.Views.Operations
{
    public class BankController : Controller
    {
        CIS2103_BankEntities bank = new CIS2103_BankEntities();
        public ActionResult Login(BankAccount bankAccount)
        {
            if (Session["Holder"] == null)
            {
                Int64.TryParse(bankAccount.PIN, out long pin);
                Account account = new Account();

                using (bank)
                {
                    account = bank.Accounts.Where(b => b.Account_No == bankAccount.Account_No && b.PIN == pin).FirstOrDefault();
                }

                if(account != null)
                {
                    bankAccount.Balance = account.Balance;
                    BankAccount holder = new BankAccount();
                    holder.Account_No = account.Account_No;
                    holder.Holder_Fname = account.Holder_Fname;
                    holder.Holder_Lname = account.Holder_Lname;
                    holder.PIN = account.PIN.ToString();
                    holder.Balance = account.Balance;

                    Session["Holder"] = holder;
                    return RedirectToAction("Index");
                } else
                {
                    Session.Remove("Holder");
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Deposit()
        {
            BankAccount holder = (BankAccount)Session["Holder"];
            TransactionEntry transaction = new TransactionEntry();
            transaction.PIN = holder.PIN;
            return View(transaction);
        }

        public ActionResult Withdraw()
        {
            BankAccount holder = (BankAccount)Session["Holder"];
            TransactionEntry transaction = new TransactionEntry();
            transaction.PIN = holder.PIN;
            return View(transaction);
        }

        public ActionResult Transfer()
        {
            BankAccount holder = (BankAccount)Session["Holder"];
            TransferEntry transaction = new TransferEntry();
            transaction.PIN = holder.PIN;
            return View(transaction);
        }

        public ActionResult Logout()
        {
            Session.Remove("Holder");
            return RedirectToAction("Login");
        }

        public ActionResult TransactionHistory()
        {
            BankAccount holder = (BankAccount)Session["Holder"];
            var account_number = holder.Account_No;
            List<Transaction> transactions = (from t in bank.Transactions
                                where t.Account_No == account_number
                                orderby t.Date descending
                                select t).Take(10).ToList();
            return View(transactions);
        }

        /*
        [HttpPost]
        public ActionResult CreateAccount(Bank model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Account account = new Account();
                    Int64.TryParse(model.PIN, out long pin);
                    account.Account_No = model.Account_No;
                    account.Holder_Name = model.Holder_Name;
                    account.PIN = pin;
                    account.Balance = 0;

                    bank.Accounts.Add(account);
                    bank.SaveChanges();

                    return View("Deposit", model);
                } catch (Exception e)
                {
                    return View("Withdraw", model);
                }
            } else
            {
                return View("InputPIN", model);
            }            
        } */

        [HttpPost]
        public ActionResult DepositAmount(TransactionEntry model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    BankAccount holder = (BankAccount)Session["Holder"];
                    var amount = (decimal)model.Amount;
                    var account_number = holder.Account_No;

                    if (amount <= 0)
                    {
                        ViewBag.AmountError = "Deposit Amount should not be less than or equal to 0.";
                        throw new Exception(ViewBag.AmountError);
                    }

                    Account account = new Account();
                    Int64.TryParse(model.PIN, out long pin);
                    account.Account_No = account_number;
                    account.PIN = pin;
                    account.Holder_Fname = holder.Holder_Fname;
                    account.Holder_Lname = holder.Holder_Lname;

                    account.Balance = holder.Balance + amount;

                    Account updatedAccount = bank.Accounts.Where(a => a.Account_No == account_number).FirstOrDefault();
                    if (updatedAccount != null)
                    {
                        bank.Entry(updatedAccount).CurrentValues.SetValues(account);
                        holder.Balance = account.Balance;
                        Session["Holder"] = holder;

                        Transaction transaction = new Transaction();
                        transaction.Transaction_ID = Guid.NewGuid();
                        transaction.Date = DateTime.Now;
                        transaction.Name = "Deposit of " + model.Amount + " at " + transaction.Date;
                        transaction.Account_No = account_number;
                        transaction.Amount = amount;
                        transaction.Type = "Deposit";
                        bank.Transactions.Add(transaction);
                    }

                    bank.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = "An error occured with the server. Please try again.";
                    return RedirectToAction("Deposit");
                }
            }
            else
            {
                return RedirectToAction("Deposit");
            }
        }

        [HttpPost]
        public ActionResult WithdrawAmount(TransactionEntry model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    BankAccount holder = (BankAccount)Session["Holder"];
                    var amount = (decimal)model.Amount;
                    var account_number = holder.Account_No;

                    if (amount > holder.Balance)
                    {
                        ViewBag.AmountError = "Withdraw Amount should not exceed your remaining balance.";
                        throw new Exception(ViewBag.AmountError);
                    }

                    Account account = new Account();
                    Int64.TryParse(model.PIN, out long pin);
                    account.Account_No = account_number;
                    account.PIN = pin;
                    account.Holder_Fname = holder.Holder_Fname;
                    account.Holder_Lname = holder.Holder_Lname;

                    account.Balance = holder.Balance - amount;

                    Account updatedAccount = bank.Accounts.Where(a => a.Account_No == account_number).FirstOrDefault();
                    if (updatedAccount != null)
                    {
                        bank.Entry(updatedAccount).CurrentValues.SetValues(account);
                        holder.Balance = account.Balance;
                        Session["Holder"] = holder;

                        Transaction transaction = new Transaction();
                        transaction.Transaction_ID = Guid.NewGuid();
                        transaction.Date = DateTime.Now;
                        transaction.Name = "Withdrawal of " + model.Amount + " at " + transaction.Date;
                        transaction.Account_No = account_number;
                        transaction.Amount = amount;
                        transaction.Type = "Withdraw";
                        bank.Transactions.Add(transaction);
                    }

                    bank.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = "An error occured with the server. Please try again.";
                    return RedirectToAction("Withdraw");
                }
            }
            else
            {
                return RedirectToAction("Withdraw");
            }
        }

        [HttpPost]
        public ActionResult TransferAmount(TransferEntry model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Account receiver = bank.Accounts.Find(model.Receiver_No);
                    if (receiver != null)
                    {

                        BankAccount holder = (BankAccount)Session["Holder"];
                        var amount = (decimal)model.Amount;
                        var account_number = holder.Account_No;

                        if (amount > holder.Balance)
                        {
                            ViewBag.AmountError = "Transfer amount should not exceed your remaining balance.";
                            throw new Exception(ViewBag.AmountError);
                        }

                        Account updatedSender = new Account();
                        Int64.TryParse(model.PIN, out long pin);
                        updatedSender.Account_No = account_number;
                        updatedSender.PIN = pin;
                        updatedSender.Holder_Fname = holder.Holder_Fname;
                        updatedSender.Holder_Lname = holder.Holder_Lname;
                        updatedSender.Balance = holder.Balance - amount;

                        Account updatedReceiver = new Account();
                        updatedReceiver = receiver;
                        updatedReceiver.Balance = receiver.Balance + amount;

                        Account sender = bank.Accounts.Where(a => a.Account_No == account_number).FirstOrDefault();
                        if (sender != null)
                        {
                            bank.Entry(sender).CurrentValues.SetValues(updatedSender);
                            bank.Entry(receiver).CurrentValues.SetValues(updatedReceiver);
                            holder.Balance = updatedSender.Balance;
                            Session["Holder"] = holder;

                            Transaction transaction = new Transaction();
                            transaction.Transaction_ID = Guid.NewGuid();
                            transaction.Date = DateTime.Now;
                            transaction.Name = "Transfer of " + model.Amount + " to " + receiver.Holder_Fname + " " + receiver.Holder_Lname + " at " + transaction.Date;
                            transaction.Account_No = account_number;
                            transaction.Amount = amount;
                            transaction.Type = "Transfer";
                            bank.Transactions.Add(transaction);

                            Transfer transfer = new Transfer();
                            transfer.Transfer_ID = Guid.NewGuid();
                            transfer.Transaction_ID = transaction.Transaction_ID;
                            transfer.Receiver_Account_Number = receiver.Account_No;
                            bank.Transfers.Add(transfer);
                        }

                        bank.SaveChanges();
                        return RedirectToAction("Index");
                    }

                    else
                    {
                        ViewBag.ReceiverError = "Account with given number not found. Please try again.";
                        throw new Exception(ViewBag.ReceiverError);
                    }
                    
                }
                catch (Exception e)
                {
                    ViewBag.ErrorMessage = "An error occured with the server. Please try again.";
                    return RedirectToAction("Transfer");
                }
            }
            else
            {
                return RedirectToAction("Transfer");
            }
        }
    }
}