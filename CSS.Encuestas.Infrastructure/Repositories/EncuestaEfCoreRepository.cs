using CSS.Encuestas.Application.Interfaces.Repositories;
using CSS.Encuestas.Domain.Entities;
using CSS.Encuestas.Infrastructure.Persistence;

namespace CSS.Encuestas.Infrastructure.Repositories;
public class EncuestaEfCoreRepository(AppDbContext db) : IEncuestaRepository
{

    public async Task<int> AddAsync(EncuestaPreanalitica entity)
    {
        db.Encuestas.Add(entity);
        await db.SaveChangesAsync();

        return entity.Id;


    }
}
