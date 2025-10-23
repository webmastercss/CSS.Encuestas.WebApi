using CSS.Encuestas.Domain.Entities;

namespace CSS.Encuestas.Application.Interfaces;
public interface IEncuestaRepository
{
    Task<int> AddAsync(EncuestaPreanalitica entity);

}
