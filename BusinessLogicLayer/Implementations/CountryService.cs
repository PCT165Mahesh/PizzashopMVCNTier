using DataLogicLayer.Interfaces;
using DataLogicLayer.Models;

namespace BusinessLogicLayer.Implementations;

public class CountryService : ICountryService
{
    private readonly ICountryDetailRepository _countryDetailRepository;


    public CountryService(ICountryDetailRepository countryDetailRepository)
    {
        _countryDetailRepository = countryDetailRepository;

    }
    public List<City> GetCities(long id)
    {
        return _countryDetailRepository.GetCity(id);
    }

    public List<Country> GetCountries()
    {
        return _countryDetailRepository.GetCountry();
    }

    public List<State> GetStates(long countryid)
    {
        return _countryDetailRepository.GetState(countryid);
    }

}
