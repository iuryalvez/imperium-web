using System.ComponentModel.DataAnnotations;

public class SetActivationPayload
{
    [Required]
    public bool Activate { get; set; }
}