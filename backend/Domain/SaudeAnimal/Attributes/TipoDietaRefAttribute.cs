using System;
using backend.Domain.SaudeAnimal.Entities;

namespace backend.Domain.SaudeAnimal.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TipoDietaRefAttribute : Attribute
    {
        public TipoDieta[] TiposDieta { get; }

        public TipoDietaRefAttribute(params TipoDieta[] tiposDieta)
        {
            TiposDieta = tiposDieta;
        }
    }
}
