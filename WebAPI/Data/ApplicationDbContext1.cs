using Microsoft.EntityFrameworkCore;
using System;
using WebAPI.Data.Entities;

namespace WebAPI.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions options) : base(options)
		{
		}

		public DbSet<OceanNetworkEntity> OceanNetworks { get; set; }
	}
}
