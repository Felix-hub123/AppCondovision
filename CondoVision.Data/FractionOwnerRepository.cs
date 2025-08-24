using CondoVision.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CondoVision.Data
{
    public class FractionOwnerRepository :GenericRepository<FractionOwner>, IFractionOwnerRepository
    {
        public FractionOwnerRepository(DataContext context) : base(context)
        {
        }
        public async Task<IEnumerable<FractionOwner>> GetAllActiveAsync()
        {
            return await _context.FractionOwners
                .Where(f => !f.WasDeleted)
                .Include(f => f.Unit)
                .ThenInclude(u => u!.Condominium)
                .Include(f => f.User)
                .ToListAsync();
        }

        public async Task CreateWithRelationsAsync(FractionOwner entity)
        {
            if (entity.UnitId > 0)
            {
                entity.Unit = await _context.Units.FindAsync(entity.UnitId);
                if (entity.Unit == null)
                    throw new ArgumentException("Unidade não encontrada.");
            }
            if (!string.IsNullOrEmpty(entity.UserId))
            {
                entity.User = await _context.Users.FindAsync(entity.UserId);
                if (entity.User == null)
                    throw new ArgumentException("Usuário não encontrado.");
            }
            await _context.FractionOwners.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateWithRelationsAsync(FractionOwner entity)
        {
            if (entity.UnitId > 0)
            {
                entity.Unit = await _context.Units.FindAsync(entity.UnitId);
                if (entity.Unit == null)
                    throw new ArgumentException("Unidade não encontrada.");
            }
            if (!string.IsNullOrEmpty(entity.UserId))
            {
                entity.User = await _context.Users.FindAsync(entity.UserId);
                if (entity.User == null)
                    throw new ArgumentException("Usuário não encontrado.");
            }
            _context.FractionOwners.Update(entity);
            await _context.SaveChangesAsync();
        }
    }



}

