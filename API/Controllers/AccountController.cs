using API.Data;
using API.DTOs;
using API.Entities;
using API.Entities.OrderAggregate;
using API.Extensions;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers;

public class AccountController: BaseApiController
{
    private readonly UserManager<User> _userManager;
    private readonly TokenService _tokenService;
    private readonly StoreContext _context;

    public AccountController(UserManager<User> userManager, TokenService tokenService,
		StoreContext storeContext)
	{
		_userManager = userManager;
		_tokenService = tokenService;
		_context = storeContext;
	}

	[HttpPost("login")]
	public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
	{
		var user = await _userManager.FindByNameAsync(loginDto.Username);

		if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
			return Unauthorized();

		var userBasket = await RetrieveBasket(loginDto.Username);
		var anonymousBasket = await RetrieveBasket(Request.Cookies["buyerId"]);

		//Transfer anonymous basket to user if anonymous basket is not null
		if(anonymousBasket != null)
		{
			if (userBasket != null) _context.Baskets.Remove(userBasket);
			anonymousBasket.BuyerId = user.UserName;
			//Since anonymous basket transferred to user, we can delete the anonymous buyer Id cookie
			Response.Cookies.Delete("buyerId");
			await _context.SaveChangesAsync();
		}

		return new UserDto
		{
			Email = user.Email,
			Token = await _tokenService.GenerateToken(user),
		    Basket = anonymousBasket != null ? anonymousBasket.MapBasketToDto() : userBasket?.MapBasketToDto()
		};
	}

	[HttpPost("register")]
	public async Task<ActionResult> Register(RegisterDto registerDto)
	{
		var user = new User { UserName = registerDto.Username, Email = registerDto.Email };
		var result = await _userManager.CreateAsync(user, registerDto.Password);

		if(!result.Succeeded)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(error.Code, error.Description);
			}

			return ValidationProblem();
		}

		await _userManager.AddToRoleAsync(user, "Member");

		return StatusCode(201);
	}

	[Authorize]
	[HttpGet("currentUser")]
	public async Task<ActionResult<UserDto>> GetCurrentUser()
	{
		var user = await _userManager.FindByNameAsync(User.Identity.Name);

        var userBasket = await RetrieveBasket(User.Identity.Name);

        return new UserDto
		{
			Email = user.Email,
			Token = await _tokenService.GenerateToken(user),
			Basket = userBasket?.MapBasketToDto()
		};
	}

	[Authorize]
	[HttpGet("savedAddress")]
	public async Task<ActionResult<UserAddress>> GetSavedAddress()
	{
		return await _userManager.Users
			.Where(x => x.UserName == User.Identity.Name)
			.Select(user => user.Address)
			.FirstOrDefaultAsync();
	}


    private async Task<Basket> RetrieveBasket(string buyerId)
    {
        if (string.IsNullOrEmpty(buyerId))
        {
            Response.Cookies.Delete("buyerId");
			return null;
		}

        return await _context.Baskets
                .Include(i => i.Items)
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(x => x.BuyerId == buyerId);
    }

}