using AutoMapper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebAPIBudget.Models;
using WebAPIBudget.Models.Domain;

namespace WebAPIBudget.Controllers
{
    [Authorize]
    public class TransactionController : ApiController
    {
        private ApplicationDbContext Context;

        public TransactionController()
        {
            Context = new ApplicationDbContext();
        }

        [HttpGet]
        public IHttpActionResult Transaction(int? id)
        {
            var userId = User.Identity.GetUserId();

            var transaction = Context
                            .Transactions
                            .FirstOrDefault(p => p.Id == id);

            if (transaction.BankAccount.Household.OwnerId == userId || transaction.BankAccount.Household.Members.Any(x => x.Id == userId))
            {
                var model = new TransactionViewModel
                {
                    Title = transaction.Title,
                    Description = transaction.Description,
                    TransactionDate = transaction.TransactionDate,
                    Amount = transaction.Amount,
                    BankAccountId = transaction.BankAccountId,
                    IsVoided = transaction.IsVoided
                };
                return Ok(model);
            }
            return Ok();
        }
        [HttpPost]
        public IHttpActionResult Create(CreateTransactionBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();
            var a = Context.BankAccounts.FirstOrDefault(p => p.Id == model.BankAccountId);
            var bankAccount = Context
                .BankAccounts
                .FirstOrDefault(p => p.Id == model.BankAccountId &&
                (p.Household.OwnerId == userId ||
                p.Household.Members.Any(t => t.Id == userId)));

            if (bankAccount == null)
            {
                ModelState.AddModelError("",
                    "Bank account doesn't exist or you don't belong to this household");
                return BadRequest(ModelState);
            }

            var category = Context
                .Categories
                .FirstOrDefault(p => p.Id == model.CategoryId &&
                p.HouseholdId == bankAccount.HouseholdId);

            if (category == null)
            {
                ModelState.AddModelError("", "Category doesn't exist in this household");
                return BadRequest(ModelState);
            }

            var transaction = Mapper.Map<Transaction>(model);
            transaction.OwnerId = userId;
            transaction.DateCreated = DateTime.Now;
            bankAccount.Balance += transaction.Amount;

            Context.Transactions.Add(transaction);
            Context.SaveChanges();

            var result = Mapper.Map<TransactionViewModel>(transaction);

            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult Edit(int id, EditTransactionBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();

            var transaction = Context
                .Transactions
                .FirstOrDefault(p => p.Id == id);

            if (transaction == null)
            {
                return NotFound();
            }

            if (transaction.BankAccount.Household.OwnerId != userId &&
                transaction.OwnerId != userId)
            {
                ModelState.AddModelError("",
                    "You are not allowed to edit this transaction");
                return BadRequest(ModelState);
            }

            var category = Context
                .Categories
                .FirstOrDefault(p => p.Id == model.CategoryId &&
                p.HouseholdId == transaction.BankAccount.HouseholdId);

            if (category == null)
            {
                ModelState.AddModelError("", "Category doesn't exist in this household");
                return BadRequest(ModelState);
            }

            if (!transaction.IsVoided)
            {
                transaction.BankAccount.Balance -= transaction.Amount;
                transaction.BankAccount.Balance += model.Amount;
            }
            transaction.DateUpdated = DateTime.Now;
            Mapper.Map(model, transaction);

            Context.SaveChanges();

            var result = Mapper.Map<TransactionViewModel>(transaction);

            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult Delete(int id)
        {
            var userId = User.Identity.GetUserId();

            var transaction = Context
                .Transactions
                .FirstOrDefault(p => p.Id == id);

            if (transaction == null)
            {
                return NotFound();
            }

            if (transaction.BankAccount.Household.OwnerId != userId &&
                transaction.OwnerId != userId)
            {
                ModelState.AddModelError("",
                    "You are not allowed to delete this transaction");
                return BadRequest(ModelState);
            }

            if (!transaction.IsVoided)
            {
                transaction.BankAccount.Balance -= transaction.Amount;
            }

            Context.Transactions.Remove(transaction);
            Context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        public IHttpActionResult Void(int id)
        {
            var userId = User.Identity.GetUserId();

            var transaction = Context
                .Transactions
                .FirstOrDefault(p => p.Id == id);

            if (transaction == null)
            {
                return NotFound();
            }

            if (transaction.BankAccount.Household.OwnerId != userId &&
                transaction.OwnerId != userId)
            {
                ModelState.AddModelError("",
                    "You are not allowed to void this transaction");
                return BadRequest(ModelState);
            }

            if (transaction.IsVoided)
            {
                ModelState.AddModelError("",
                    "This transaction has already been voided");
                return BadRequest(ModelState);
            }

            transaction.BankAccount.Balance -= transaction.Amount;
            transaction.IsVoided = true;
            
            Context.SaveChanges();

            return Ok();
        }

        [HttpGet]
        public IHttpActionResult View(int id)
        {
            var userId = User.Identity.GetUserId();

            var bankAccount = Context
                .BankAccounts
                .FirstOrDefault(p => p.Id == id);

            if (bankAccount == null)
            {
                return NotFound();
            }

            if (bankAccount.Household.OwnerId != userId &&
                !bankAccount.Household.Members.Any(t => t.Id == userId))
            {
                ModelState.AddModelError("",
                    "You are not allowed to see transactions for this bank account");
                return BadRequest(ModelState);
            }

            var result = Mapper.Map<List<TransactionViewModel>>(bankAccount.Transactions);

            return Ok(result);
        }
    }
}
