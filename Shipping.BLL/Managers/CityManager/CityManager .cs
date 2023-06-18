﻿using Shipping.BLL.Dtos.CityDtos;
using Shipping.BLL.Dtos;
using Shipping.DAL.Data.Models;
using Shipping.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Shipping.BLL.Managers
{
    public class CityManager:ICityManager
    {

        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<Governorate> _governorateRepository;

        public CityManager(IRepository<City> cityRepository, IRepository<Governorate> governorateRepository)
        {
            _cityRepository = cityRepository;
            _governorateRepository=governorateRepository;
        }
        public async Task<IEnumerable<ShowCityDto>> GetAllCityWithDeletedAsync()
        {
            var cities = _cityRepository.GetAllAsync().Result.Include(c=>c.Governorate);
         

            return cities.Select(c => new ShowCityDto
            {
                id=c.Id,
                Name = c.Name,
                Price = c.Price,
                Pickup = c.Pickup,
                IsDeleted = c.isDeleted,
                GovernorateId = c.GovernorateId,
                GovernorateName =c.Governorate.Name
            });
        }
        public async Task<int> CreateCityAsync(AddCityDto cityDto)
        {
            var governorate = await _governorateRepository.GetByCriteriaAsync(g => g.Id == cityDto.GovernorateId && g.IsDeleted == false);


            if (governorate == null)
            {
                return -1;
            }
            var city = new City
            {
                Name = cityDto.Name,
                Price = cityDto.Price,
                Pickup = cityDto.Pickup,
                GovernorateId = cityDto.GovernorateId
            };

            return await _cityRepository.AddAsync(city);
        }

        public async Task<int> UpdateCityAsync(UpdateCityDto cityDto)
        {
            var city = await _cityRepository.GetByIdAsync(cityDto.Id);
            if (city == null)
            {
                return 0;
            }

           
            var governorate =  await _governorateRepository.GetByCriteriaAsync(g => g.Id == cityDto.GovernorateId && g.IsDeleted == false);
            if (governorate == null)
            {
                return -1;
            }
           
            city.Name = cityDto.Name;
            city.Price = cityDto.Price;
            city.Pickup = cityDto.Pickup;
            city.GovernorateId = cityDto.GovernorateId;
            return await _cityRepository.UpdateAsync(city);
        }

        public async Task<int> DeleteCityAsync(int id)
        {
            var city = await _cityRepository.GetByIdAsync(id);
            if (city == null)
            {
                return 0;
            }

            city.isDeleted = true;

            return await _cityRepository.UpdateAsync(city);
        }


    }
}



