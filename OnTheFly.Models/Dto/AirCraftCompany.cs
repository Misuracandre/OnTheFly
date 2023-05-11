using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OnTheFly.Models.Dto
{
    public class AirCraftCompany
    {
        [StringLength(18)]
        [JsonProperty("cnpj")]
        public string Cnpj { get; set; }
        [StringLength(30)]
        [JsonProperty("name")]
        public string? Name { get; set; }
        [StringLength(30)]
        [JsonProperty("nameOpt")]
        public string NameOpt { get; set; }
        [JsonProperty("dtOpen")]
        public string DtOpen { get; set; }
        [JsonProperty("status")]
        public bool? Status { get; set; }
        [JsonProperty("address")]
        public AddressMapDataDTO Address { get; set; }

        public AirCraftCompany() { }
    }
}
