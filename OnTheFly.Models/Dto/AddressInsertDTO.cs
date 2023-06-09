﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OnTheFly.Models.Dto
{
    public class AddressInsert
    {
        public string ZipCode { get; set; }
        public int Number { get; set; }
        public string? Complement { get; set; }

        public AddressInsert() { }

        public AddressInsert(Address address) 
        {
            this.ZipCode = address.ZipCode;
            this.Number = address.Number;
            this.Complement = address.Complement;
        }
    }


}
