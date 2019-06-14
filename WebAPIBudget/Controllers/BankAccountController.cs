using AutoMapper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPIBudget.Models;
using WebAPIBudget.Models.Domain;

namespace WebAPIBudget.Controllers
{
    [Authorize]
    public class BankAccountController : ApiController
    {
        private ApplicationDbContext Context;

        public BankAccountController()
        {
            Context = new ApplicationDbContext();
        }

        [HttpPost]
        public IHttpActionResult Create(CreateBankAccountBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();

            var household = Context
                .Households
                .FirstOrDefault(p => p.Id == model.HouseholdId
                && p.OwnerId == userId);

            if (household == null)
            {
                ModelState.AddModelError("", 
                    "Household doesn't exist or you're not the owner");

                return BadRequest(ModelState);
            }

            var bankAccount = Mapper.Map<BankAccount>(model);

            Context.BankAccounts.Add(bankAccount);
            Context.SaveChanges();

            var result = Mapper.Map<BankAccountViewModel>(bankAccount);

            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult Edit(int id, EditBankAccountBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();

            var bankAccount = Context
                .BankAccounts
                .FirstOrDefault(p => p.Id == id);
            
            if (bankAccount == null)
            {
                return NotFound();
            }

            if (bankAccount.Household.OwnerId != userId)
            {
                ModelState.AddModelError("", "You're not the owner of this household");
                return BadRequest(ModelState);
            }

            Mapper.Map(model, bankAccount);
            bankAccount.DateUpdated = DateTime.Now;
            
            Context.SaveChanges();

            var result = Mapper.Map<BankAccountViewModel>(bankAccount);

            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult Delete(int id)
        {
            var userId = User.Identity.GetUserId();

            var bankAccount = Context
                .BankAccounts
                .FirstOrDefault(p => p.Id == id);

            if (bankAccount == null)
            {
                return NotFound();
            }

            if (bankAccount.Household.OwnerId != userId)
            {
                ModelState.AddModelError("", "You're not the owner of this household");
                return BadRequest(ModelState);
            }

            Context.BankAccounts.Remove(bankAccount);
            Context.SaveChanges();

            return Ok();
        }

        public IHttpActionResult GetBankAccount(int? id)
        {
            var userId = User.Identity.GetUserId();

            var category = Context
                            .BankAccounts
                            .FirstOrDefault(p => p.Id == id);

            if (category.Household.OwnerId == userId /*|| category.Household.HouseMembers.Any(x => x.Id == userId*/)
            {
                var model = new BankAccountViewModel
                {
                    Name = category.Name,
                    Description = category.Description,
                    Id = category.Id,
                    DateCreated = category.DateCreated,
                    DateUpdated = category.DateUpdated,
                    HouseholdId = category.HouseholdId
                };
                return Ok(model);
            }
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult View(int id)
        {
            var userId = User.Identity.GetUserId();

            var household = Context
                .Households
                .FirstOrDefault(p => p.Id == id &&
                (p.OwnerId == userId || p.Members.Any(t => t.Id == userId)));

            if (household == null)
            {
                return NotFound();
            }

            var result = Mapper
                .Map<List<BankAccountViewModel>>(household.BankAccounts);

            /* Querying for the bank accounts directly

            var bankAccounts = Context
                .BankAccounts
                .Where(p => p.Household.Id == id &&
                p.Household.OwnerId == userId
                || p.Household.Members.Any(t => t.Id == userId));

            */

            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult UpdateBalance(int id)
        {
            var userId = User.Identity.GetUserId();

            var bankAccount = Context
                .BankAccounts
                .FirstOrDefault(p => p.Id == id);

            if (bankAccount == null)
            {
                return NotFound();
            }

            if (bankAccount.Household.OwnerId != userId)
            {
                ModelState.AddModelError("", "You're not the owner of this household");
                return BadRequest(ModelState);
            }

            //var balance = 0m;

            //foreach(var transaction in bankAccount.Transactions)
            //{
            //    if (!transaction.IsVoided)
            //    {
            //        balance += transaction.Amount;
            //    }
            //}

            //bankAccount.Balance = balance;

            bankAccount.Balance = Context
                .Transactions
                .Where(p => !p.IsVoided && p.BankAccountId == id)
                .Sum(p => (decimal?)p.Amount) ?? 0;
            
            Context.SaveChanges();

            var result = Mapper.Map<BankAccountViewModel>(bankAccount);

            return Ok(result);
        }
    }
}
