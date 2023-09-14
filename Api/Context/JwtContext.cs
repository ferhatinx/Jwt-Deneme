using Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Api.Context;

public class JwtContext : IdentityDbContext
{
	public JwtContext(DbContextOptions<JwtContext> options) : base(options)
	{

	}
	public DbSet<Employee>? Employees { get; set; }
}
