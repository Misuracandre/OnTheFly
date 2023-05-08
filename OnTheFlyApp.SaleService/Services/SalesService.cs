using OnTheFly.Models.Dto;
using OnTheFly.Models;
using Utility;
using MongoDB.Driver;
using OnTheFlyApp.SaleService.config;
using Microsoft.AspNetCore.Identity;

namespace OnTheFlyApp.SaleService.Services
{
    public class SalesService
    {
        private readonly IMongoCollection<Sale> _sale;
        private readonly IMongoCollection<Sale> _saleDeactivated;
        private readonly Util _util;

        public SalesService(ISaleServiceSettings settings, Util util)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _sale = database.GetCollection<Sale>(settings.SaleCollectionName);
            _saleDeactivated = database.GetCollection<Sale>(settings.SaleDeactivedCollectionName);
            _util = util;
        }

        public List<Sale> GetAll()
        {
            return _sale.Find<Sale>(s => true).ToList();
        }

        public Sale GetByIdentifier(string rab, DateTime schedule, string cpf)
        {
            Sale s = _sale.Find(s => s.Flight.Departure == schedule && s.Flight.Plane.Rab == rab &&
                                    s.Passengers[0].Cpf == cpf).FirstOrDefault();
            if (s == null) return null;
            return s;
        }

        public SaleDTO Create(SaleDTO sale)
        {
            //numero de vendas nao exceda a capacidade do aviao
            //garantir que o mesmo cpf nao apareca na lista de passageiros
            //verificar se o passageiro nao esta restrito, cancelar venda de verdadeiro
            //primeiro passageiro deve ser maior de 18 anos, se menor de 18, cancela a venda
            Sale s = new Sale(sale);
            _sale.InsertOne(s);
            return sale;
        }
    }
}
