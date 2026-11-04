using backend.Domain.AtendimentoAnimal.DTOs.Consulta;
using backend.Domain.Utils.Paginacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Domain.AtendimentoAnimal.Entities;

namespace backend.Domain.AtendimentoAnimal.Interfaces
{
    public interface IConsultaRepository
    {
        Task<Consulta?> GetByIdAsync(int consultaId);
        Task<Consulta?> GetByIdWithDetailsAsync(int consultaId);
        Task<PaginaResultado<Consulta>> ListarDaClinicaAsync(ConsultaFiltro filtro, int clinicaId, int? usuarioId);
        Task<Consulta?> GetByIdAndClinicIdAsync(int consultaId, int clinicaId);
        Task AddAsync(Consulta consulta);
        void Delete(Consulta consulta);
        Task<int> SaveChangesAsync();
    }
}