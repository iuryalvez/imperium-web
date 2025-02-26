namespace backend.Dtos.Usuario
{
    public class EditarUsuarioDto
    {
        public int Id { get; set; }
        public int UpcId { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Nome { get; set; }
        public string? Perfil { get; set; }
        public bool Habilitado { get; set; }    
    }
}