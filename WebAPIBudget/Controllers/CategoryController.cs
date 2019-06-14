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
    public class CategoryController : ApiController
    {
        private ApplicationDbContext Context;

        public CategoryController()
        {
            Context = new ApplicationDbContext();
        }

        [HttpPost]
        public IHttpActionResult Create(CreateCategoryBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Identity.GetUserId();
            var household = Context
                .Households
                .FirstOrDefault(p => p.Id == model.HouseholdId);

            if (household == null || household.OwnerId != userId)
            {
                return NotFound();
            }
            
            var category = Mapper.Map<Category>(model);
            category.DateCreated = DateTime.Now;

            Context.Categories.Add(category);
            Context.SaveChanges();

            var result = Mapper.Map<CategoryViewModel>(category);

            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult Edit(int id, EditCategoryBindingModel model)
        {
            var userId = User.Identity.GetUserId();

            var category = Context
                .Categories
                .FirstOrDefault(p => p.Id == id &&
                p.Household.OwnerId == userId);

            if (category == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Mapper.Map(model, category);
            category.DateUpdated = DateTime.Now;
            Context.SaveChanges();

            var result = Mapper.Map<CategoryViewModel>(category);

            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult Delete(int id)
        {
            var userId = User.Identity.GetUserId();

            var category = Context
                .Categories
                .FirstOrDefault(p => p.Id == id &&
                p.Household.OwnerId == userId);

            if (category == null)
            {
                return NotFound();
            }

            Context.Categories.Remove(category);
            Context.SaveChanges();
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult HouseHoldCategories(int id)
        {
            var userId = User.Identity.GetUserId();
            
            var h = Context.Households.FirstOrDefault(p => p.Id == id);
            if(h == null)
            {
              return  NotFound();
            }
            var result = h.Categories
                .Select(p => new CategoryViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    CreatedBy = p.Household.Owner.UserName,
                    IsOwner = p.Household.OwnerId == userId,
                    Description = p.Description,
                    HouseholdId = id,
                })
                .ToList();

            return Ok(result);
        }

        public IHttpActionResult GetCategory(int? id)
        {
            var userId = User.Identity.GetUserId();

            var category = Context
                            .Categories
                            .FirstOrDefault(p => p.Id == id);

            if (category.Household.OwnerId == userId /*|| category.Household.HouseMembers.Any(x => x.Id == userId*/)
            {
                var model = new CategoryViewModel
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

            var categories = Context
                .Categories
                .Where(p => p.HouseholdId == id &&
                (p.Household.OwnerId == userId
                || p.Household.Members.Any(t => t.Id == userId)))
                .ToList();
            //.Where(p =>
            // {
            //     //bool isHouse = p.HouseholdId == id;
            //     //bool isOwner = p.Household.OwnerId == userId;
            //     //bool isMember = p.Household.Members.Any(t => t.Id == userId);
            //     //return (isHouse && (isOwner || isMember));
            // })
            if (categories == null)
            {
                return NotFound();
            }
            //Mapper.Map<List<CategoryViewModel>>(categories);
            var result = categories.Select(p => new CategoryViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                DateCreated = p.DateCreated,
                DateUpdated = p.DateUpdated,
                CreatedBy = p.Household.CreatedBy,
                IsOwner = p.Household.OwnerId == userId
            });


            return Ok(result);
        }
    }
}
