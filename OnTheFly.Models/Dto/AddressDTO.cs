using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OnTheFly.Models.Dto
{
    public class AddressDTO
    {
        [JsonIgnore]
        public string Id { get; set; }
        [JsonProperty("cep")]
        public string ZipCode { get; set; }

        [JsonProperty("logradouro")]
        public string? Street { get; set; }
        public int Number { get; set; }

        [JsonProperty("complemento")]
        public string? Complement { get; set; }

        [JsonProperty("localidade")]
        public string City { get; set; }

        [JsonProperty("uf")]
        public string State { get; set; }

        public AddressDTO() { }

        public AddressDTO(Address address)
        {
            this.Id = address.Id;
            this.ZipCode = address.ZipCode;
            this.Street = address.Street;
            this.Number = address.Number;
            this.Complement = address.Complement;
            this.City = address.City;
            this.State = address.State;
        }
    }
}
