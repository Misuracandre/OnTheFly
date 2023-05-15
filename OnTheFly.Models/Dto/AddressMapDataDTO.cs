using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OnTheFly.Models.Dto
{
    public class AddressMapDataDTO
    {
        [JsonProperty("zipCode")]
        public string ZipCode { get; set; }

        [JsonProperty("street")]
        public string? Street { get; set; }

        [JsonProperty("number")]
        public int Number { get; set; }

        [JsonProperty("complement")]
        public string? Complement { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
    }
}
