using Newtonsoft.Json;
using OnTheFly.Models;

namespace Utility
{
    public class Util
    {
        public string JustDigits(string text)
        {
            if (text == "" || text == null) return null;
            string removeChars = " .?&^$#@/!()+-,:;<>’\'-_*abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string result = text;

            foreach (char c in removeChars)
            {
                result = result.Replace(c.ToString(), string.Empty);
            }

            return result;
        }

        public bool VerifyCpf (string cpf) 
        { 
            if(cpf.Length != 11)
                return false;

            int soma = 0;
            int digit = 0;
            for (int i = 0; i < 9; i++)
            {
                digit = int.Parse(cpf[i].ToString());
                soma += digit * (i + 1);
            }
            int first = soma % 11;
            
            first = first == 10 ? 0 : first;

            if(first != int.Parse(cpf[9].ToString()))
                return false;

            soma = 0;
            digit = 0;
            for (int i = 0; i < 10; i++)
            {
                digit = int.Parse(cpf[i].ToString());
                soma += digit * i;
            }
            int second = soma % 11;
            second = second == 10 ? 0 : second;
            if (second != int.Parse(cpf[10].ToString()))
                return false;

            return true; 
        }

        public bool VerifyCnpj(string cnpj)
        {
            if (string.IsNullOrEmpty(cnpj) || cnpj.Length != 14)
            {
                return false;
            }
            int[] multiplier1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplier2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            if (cnpj.Length != 14)
            {
                return false;
            }
            string tempCnpj = cnpj.Substring(0, 12);
            int sum = 0;
            for (int i = 0; i < 12; i++)
            {
                sum += int.Parse(tempCnpj[i].ToString()) * multiplier1[i];
            }
            int remainder = (sum % 11);
            if (remainder < 2)
            {
                remainder = 0;
            }
            else
            {
                remainder = 11 - remainder;
            }
            string digit = remainder.ToString();
            tempCnpj = tempCnpj + digit;
            sum = 0;
            for (int i = 0; i < 13; i++)
            {
                sum += int.Parse(tempCnpj[i].ToString()) * multiplier2[i];
            }
            remainder = (sum % 11);
            if (remainder < 2)
            {
                remainder = 0;
            }
            else
            {
                remainder = 11 - remainder;
            }
            digit = digit + remainder.ToString();
            return cnpj.EndsWith(digit);
        }

        public async Task<Address> GetAddress(string cep)
        {
            HttpClient address = new HttpClient();
            try
            {
                HttpResponseMessage response = await address.GetAsync("https://viacep.com.br/ws/" + cep + "/json/");
                response.EnsureSuccessStatusCode();
                string ender = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Address>(ender);
            }
            catch (HttpRequestException)
            {
                return null;
            }

            
        }

        public bool LegalAge (DateTime dtBirth)
        {
            DateTime today = DateTime.Now;

            int qtdYears = today.Year - dtBirth.Year;
            int qtdMonths = today.Month - dtBirth.Month;
            int qtdDays = today.Day - dtBirth.Day;

            if (qtdYears > 18)
                return true;
            if (qtdMonths > 0)
                return true;
            if (qtdDays > 0)
                return true;
            return false;
        }
    }
}