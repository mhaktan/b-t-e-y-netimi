using AutoMapper;
using ButceYonetimi.Entities;
using ButceYonetimi.ExchangeRates.Dto;

namespace ButceYonetimi.ExchangeRates
{
    public class ExchangeRateMapProfile : Profile
    {
        public ExchangeRateMapProfile()
        {
            CreateMap<ExchangeRate, ExchangeRateDto>();
            CreateMap<CreateExchangeRateDto, ExchangeRate>();
            CreateMap<ExchangeRateDto, ExchangeRate>();
        }
    }
}
