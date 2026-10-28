using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using backend.Domain.AtendimentoAnimal.Enums;
using backend.Domain.GestaoClinica.Entities;

namespace backend.Domain.AtendimentoAnimal.Entities
{
    public class Raca
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        public Porte? Porte { get; set; }

        [Required]
        public EspecieAnimal Especie { get; set; }

        public double? PesoMinimoReferenciaFemea { get; set; }
        public double? PesoMaximoReferenciaFemea { get; set; }
        public double? PesoMinimoReferenciaMacho { get; set; }
        public double? PesoMaximoReferenciaMacho { get; set; }
        [Required]
        [StringLength(100)]
        public string? Referencia { get; set; } = string.Empty;

        [JsonIgnore] // Avoids circular references in API responses
        public ICollection<Animal>? Animais { get; set; }
        public int? ClinicaId { get; set; }
        [ForeignKey("ClinicaId")]
        public Clinica? Clinica { get; set; }

        public CategoriaMetabolica CategoriaMetabolica { get; set; } = CategoriaMetabolica.Padrao;

        public DateTime DataCadastro { get; set; }
        public bool IsAtivo { get; set; } = true;
        public DateTime? DataDesativacao { get; set; }

        [JsonIgnore]
        public ICollection<RacaOcultaClinica>? OcultaEmClinicas { get; set; } // clínicas que ocultaram esta raça do sistema

        [NotMapped]
        public double? PesoMedioMacho => (PesoMinimoReferenciaMacho.HasValue && PesoMaximoReferenciaMacho.HasValue)
            ? (PesoMinimoReferenciaMacho.Value + PesoMaximoReferenciaMacho.Value) / 2f
            : (PesoMinimoReferenciaMacho ?? PesoMaximoReferenciaMacho);

        [NotMapped]
        public double? PesoMedioFemea => (PesoMinimoReferenciaFemea.HasValue && PesoMaximoReferenciaFemea.HasValue)
            ? (PesoMinimoReferenciaFemea.Value + PesoMaximoReferenciaFemea.Value) / 2f
            : (PesoMinimoReferenciaFemea ?? PesoMaximoReferenciaFemea);
    }
}

// Listas de raças de cães e gatos
// const dogBreeds = [
//   "Sem Raça Definida - SRD",
//   "Affenpinscher",
//   "Afghan Hound",
//   "Airedale Terrier",
//   "Akita",
//   "Alaskan Malamute",
//   "American Bulldog",
//   "American Pit Bull Terrier",
//   "American Staffordshire Terrier",
//   "Anatolian Shepherd Dog",
//   "Australian Cattle Dog",
//   "Australian Shepherd",
//   "Australian Terrier",
//   "Basenji",
//   "Basset Hound",
//   "Beagle",
//   "Bearded Collie",
//   "Bedlington Terrier",
//   "Belgian Malinois",
//   "Belgian Sheepdog",
//   "Belgian Tervuren",
//   "Bernese Mountain Dog",
//   "Bichon Frise",
//   "Black and Tan Coonhound",
//   "Bloodhound",
//   "Border Collie",
//   "Border Terrier",
//   "Borzoi",
//   "Boston Terrier",
//   "Bouvier des Flandres",
//   "Boxer",
//   "Boykin Spaniel",
//   "Briard",
//   "Brittany",
//   "Brussels Griffon",
//   "Bull Terrier",
//   "Bulldog",
//   "Bullmastiff",
//   "Cairn Terrier",
//   "Canaan Dog",
//   "Cane Corso",
//   "Cardigan Welsh Corgi",
//   "Cavalier King Charles Spaniel",
//   "Chesapeake Bay Retriever",
//   "Chihuahua",
//   "Chinese Crested",
//   "Chinese Shar-Pei",
//   "Chow Chow",
//   "Clumber Spaniel",
//   "Cocker Spaniel",
//   "Collie",
//   "Coonhound",
//   "Coton de Tulear",
//   "Curly-Coated Retriever",
//   "Dachshund",
//   "Dalmatian",
//   "Dandie Dinmont Terrier",
//   "Doberman Pinscher",
//   "Dogue de Bordeaux",
//   "English Bulldog",
//   "English Cocker Spaniel",
//   "English Foxhound",
//   "English Setter",
//   "English Springer Spaniel",
//   "English Toy Spaniel",
//   "Eskimo Dog",
//   "Finnish Spitz",
//   "Flat-Coated Retriever",
//   "French Bulldog",
//   "German Pinscher",
//   "German Shepherd Dog",
//   "German Shorthaired Pointer",
//   "German Wirehaired Pointer",
//   "Giant Schnauzer",
//   "Glen of Imaal Terrier",
//   "Golden Retriever",
//   "Gordon Setter",
//   "Great Dane",
//   "Great Pyrenees",
//   "Greater Swiss Mountain Dog",
//   "Greyhound",
//   "Harrier",
//   "Havanese",
//   "Ibizan Hound",
//   "Icelandic Sheepdog",
//   "Irish Setter",
//   "Irish Terrier",
//   "Irish Water Spaniel",
//   "Irish Wolfhound",
//   "Italian Greyhound",
//   "Jack Russell Terrier",
//   "Japanese Chin",
//   "Japanese Spitz",
//   "Keeshond",
//   "Kerry Blue Terrier",
//   "Komondor",
//   "Kuvasz",
//   "Labrador Retriever",
//   "Lakeland Terrier",
//   "Lhasa Apso",
//   "Lowchen",
//   "Maltese",
//   "Manchester Terrier",
//   "Mastiff",
//   "Miniature Bull Terrier",
//   "Miniature Pinscher",
//   "Miniature Schnauzer",
//   "Neapolitan Mastiff",
//   "Newfoundland",
//   "Norfolk Terrier",
//   "Norwegian Elkhound",
//   "Norwich Terrier",
//   "Nova Scotia Duck Tolling Retriever",
//   "Old English Sheepdog",
//   "Otterhound",
//   "Papillon",
//   "Pekingese",
//   "Pembroke Welsh Corgi",
//   "Petit Basset Griffon Vendeen",
//   "Pharaoh Hound",
//   "Plott",
//   "Pointer",
//   "Polish Lowland Sheepdog",
//   "Pomeranian",
//   "Poodle",
//   "Portuguese Water Dog",
//   "Pug",
//   "Puli",
//   "Pumi",
//   "Rat Terrier",
//   "Redbone Coonhound",
//   "Rhodesian Ridgeback",
//   "Rottweiler",
//   "Saint Bernard",
//   "Saluki",
//   "Samoyed",
//   "Schipperke",
//   "Scottish Deerhound",
//   "Scottish Terrier",
//   "Sealyham Terrier",
//   "Shetland Sheepdog",
//   "Shiba Inu",
//   "Shih Tzu",
//   "Siberian Husky",
//   "Silky Terrier",
//   "Skye Terrier",
//   "Sloughi",
//   "Smooth Fox Terrier",
//   "Soft Coated Wheaten Terrier",
//   "Spanish Water Dog",
//   "Spinone Italiano",
//   "Staffordshire Bull Terrier",
//   "Standard Schnauzer",
//   "Sussex Spaniel",
//   "Swedish Vallhund",
//   "Tibetan Mastiff",
//   "Tibetan Spaniel",
//   "Tibetan Terrier",
//   "Toy Fox Terrier",
//   "Treeing Walker Coonhound",
//   "Vizsla",
//   "Weimaraner",
//   "Welsh Springer Spaniel",
//   "Welsh Terrier",
//   "West Highland White Terrier",
//   "Whippet",
//   "Wire Fox Terrier",
//   "Wirehaired Pointing Griffon",
//   "Yorkshire Terrier",
// ];

// const catBreeds = [
//   "Sem Raça Definida - SRD",
//   "Abyssinian",
//   "American Bobtail",
//   "American Curl",
//   "American Shorthair",
//   "American Wirehair",
//   "Balinese",
//   "Bengal",
//   "Birman",
//   "Bombay",
//   "British Shorthair",
//   "Burmese",
//   "Burmilla",
//   "Chartreux",
//   "Cornish Rex",
//   "Devon Rex",
//   "Egyptian Mau",
//   "European Burmese",
//   "Exotic",
//   "Havana Brown",
//   "Himalayan",
//   "Japanese Bobtail",
//   "Khao Manee",
//   "Korat",
//   "LaPerm",
//   "Lykoi",
//   "Maine Coon",
//   "Manx",
//   "Norwegian Forest Cat",
//   "Ocicat",
//   "Oriental",
//   "Persian",
//   "Ragamuffin",
//   "Ragdoll",
//   "Russian Blue",
//   "Savannah",
//   "Scottish Fold",
//   "Selkirk Rex",
//   "Siamese",
//   "Siberian",
//   "Singapura",
//   "Snowshoe",
//   "Somali",
//   "Sphynx",
//   "Tonkinese",
//   "Toyger",
//   "Turkish Angora",
//   "Turkish Van",
// ];