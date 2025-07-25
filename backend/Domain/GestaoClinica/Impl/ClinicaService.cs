using backend.Infra.Data;
using backend.Domain.GestaoClinica.Entities;
using backend.Domain.GestaoClinica.Interface;
using Microsoft.EntityFrameworkCore;
using backend.Domain.AtendimentoAnimal.Entities;
using backend.Domain.GestaoClinica.DTOs;
using backend.Domain.GestaoDeAcesso.Interfaces;

public class ClinicaService : IClinicaService
{
    private readonly AppDbContext _context;
    private readonly IClinicaRepository _clinicaRepository;

    public ClinicaService(AppDbContext context,
                          IClinicaRepository clinicaRepository)
    {
        _context = context;
        _clinicaRepository = clinicaRepository;
    }

    public async Task<IEnumerable<GetClinicaDto>> GetAllAsync() => await _clinicaRepository.GetAllAsync();

    public async Task<ServiceResult> UpdateClinicaAsync(EditarClinicaAdminDto dto)
    {
        var clinica = await _clinicaRepository.GetByIdAsync(dto.Id);
        if (clinica == null)
            return new ServiceResult { Succeeded = false, Message = "Clínica não encontrada." };

        // Validação de negócio (ex: CNPJ único)
        if (!string.IsNullOrWhiteSpace(dto.Cnpj) && await _clinicaRepository.CnpjExistsForAnotherClinicAsync(dto.Cnpj, dto.Id))
        {
            return new ServiceResult { Succeeded = false, Message = "Este CNPJ já está em uso por outra clínica." };
        }

        if (!string.IsNullOrWhiteSpace(dto.NomeClinica)) clinica.NomeClinica = dto.NomeClinica;

        // Para campos que podem ser string vazia intencionalmente, verificamos apenas se não é nulo.
        if (dto.NomeFantasia != null) clinica.NomeFantasia = dto.NomeFantasia;
        if (dto.Cnpj != null) clinica.Cnpj = dto.Cnpj;
        if (dto.Endereco != null) clinica.Endereco = dto.Endereco;
        if (dto.EndEstado != null) clinica.EndEstado = dto.EndEstado;
        if (dto.EndCidade != null) clinica.EndCidade = dto.EndCidade;
        if (dto.Cep != null) clinica.Cep = dto.Cep;
        if (dto.Telefone1 != null) clinica.Telefone1 = dto.Telefone1;
        if (dto.Whatsapp != null) clinica.Whatsapp = dto.Whatsapp;
        if (dto.Email != null) clinica.Email = dto.Email;
        if (dto.Instagram != null) clinica.Instagram = dto.Instagram;

        // Para tipos de valor "nullable" (int?, bool?), usamos .HasValue
        if (dto.EndNumero.HasValue) clinica.EndNumero = dto.EndNumero.Value;
        clinica.IsAtiva = dto.IsAtiva;

        await _clinicaRepository.SaveChangesAsync();
        return new ServiceResult { Succeeded = true, Message = "Clínica atualizada com sucesso." };
    }

    public async Task<Clinica?> ObterClinicaAsync(int clinicaId)
    {
        return await _context.Clinicas.FindAsync(clinicaId);
    }

    public async Task<bool> TutorCpfExisteNaClinicaAsync(string cpf, int clinicaId)
    {
        return await _context.TutoresClinicas
            .Include(tc => tc.Tutor)
            .AnyAsync(tc => tc.ClinicaId == clinicaId && tc.Tutor!.Cpf == cpf);
    }
    public async Task<bool> UsuarioCpfExisteNaClinicaAsync(string cpf, int clinicaId)
    {
        return await _context.UsuariosPerfisClinicas
            .Include(upc => upc.Usuario)
            .AnyAsync(upc => upc.ClinicaId == clinicaId && upc.Usuario!.Cpf == cpf);
    }

    public async Task<List<TutorClinica>> ObterVinculosTutoresDaClinicaAsync(int clinicaId)
    {
        return await _context.TutoresClinicas
            .Include(tc => tc.Tutor)
            .Where(tc => tc.ClinicaId == clinicaId)
            .ToListAsync();
    }

    public async Task<List<AnimalClinica>> ObterVinculosAnimaisDaClinicaAsync(int clinicaId)
    {
        return await _context.AnimaisClinicas
            .Include(ac => ac.Animal)
            .Where(ac => ac.ClinicaId == clinicaId)
            .ToListAsync();
    }

    public async Task<List<AnimalClinica>> ObterVinculosAnimaisDaClinicaComTutoresAsync(int clinicaId)
    {
        return await _context.AnimaisClinicas
            .AsNoTracking() // Use AsNoTracking para consultas de leitura, melhora o desempenho
            .Include(ac => ac.Animal!)
                .ThenInclude(a => a.TutoresAnimais!) // Inclui a coleção de vínculos TutorAnimal
                    .ThenInclude(ta => ta.Tutor)   // Para cada TutorAnimal, inclui o Tutor
            .Where(ac => ac.ClinicaId == clinicaId && ac.Animal != null) // Garante que o animal existe
            .ToListAsync();
    }
}