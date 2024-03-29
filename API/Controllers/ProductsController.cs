﻿using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Extensions;
using API.RequestHelpers;
using System.Text.Json;

namespace API.Controllers
{ 
    public class ProductsController:BaseApiController
    {
        private StoreContext _context;

        public ProductsController(StoreContext storeContext)
        {
            _context = storeContext;
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<Product>>> GetProducts([FromQuery]ProductParams productParams)
        {
            //var products = await _context.Products.ToListAsync();
            var query = _context.Products
                .Sort(productParams.OrderBy)
                .Search(productParams.SearchTerm)
                .Filter(productParams.Brands,productParams.Types)
                .AsQueryable();

            var products = await PagedList<Product>.ToPagedList(query, 
                productParams.PageNumber,productParams.PageSize);

            Response.AddPaginationHeader(products.Metadata);

            return products;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product =  await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            return product;
        }

        [HttpGet("filters")]
        public async Task<IActionResult> GetFilters()
        {
            var brands = await _context.Products.Select(p => p.Brand).Distinct().ToListAsync();
            var types = await _context.Products.Select(p => p.Type).Distinct().ToListAsync();

            return Ok(new { brands, types });
        }
    }
}
