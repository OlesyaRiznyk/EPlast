﻿using AutoMapper;
using EPlast.BLL.DTO.City;
using EPlast.BLL.Interfaces.AzureStorage;
using EPlast.BLL.Interfaces.City;
using EPlast.DataAccess.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataAccessCity = EPlast.DataAccess.Entities;

namespace EPlast.BLL.Services
{
    public class CityService : ICityService
    {
        private readonly IRepositoryWrapper _repoWrapper;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly ICityBlobStorageRepository _cityBlobStorage;
        
        public CityService(IRepositoryWrapper repoWrapper,
            IMapper mapper,
            IWebHostEnvironment env,
            ICityBlobStorageRepository cityBlobStorage)
        {
            _repoWrapper = repoWrapper;
            _mapper = mapper;
            _env = env;
            _cityBlobStorage = cityBlobStorage;
        }

        public async Task<IEnumerable<DataAccessCity.City>> GetAllAsync()
        {
            var cities = await _repoWrapper.City.GetAllAsync(
                    include: source => source
                       .Include(c => c.CityAdministration)
                           .ThenInclude(t => t.AdminType)
                       .Include(k => k.CityAdministration)
                           .ThenInclude(a => a.User)
                       .Include(m => m.CityMembers)
                           .ThenInclude(u => u.User)
                       .Include(l => l.CityDocuments)
                           .ThenInclude(d => d.CityDocumentType)
                       .Include(r => r.Region));

            return cities;
        }

        public async Task<IEnumerable<CityDTO>> GetAllDTOAsync()
        {
            return _mapper.Map<IEnumerable<DataAccessCity.City>, IEnumerable<CityDTO>>(await GetAllAsync());
        }

        public async Task<IEnumerable<CityDTO>> GetCitiesByRegionAsync(int regionId)
        {
            var cities = await _repoWrapper.City.GetAllAsync(c => c.RegionId == regionId);

            return _mapper.Map<IEnumerable<DataAccessCity.City>, IEnumerable<CityDTO>>(cities);
        }

        public async Task<CityDTO> GetByIdAsync(int cityId)
        {
            var city = await _repoWrapper.City.GetFirstOrDefaultAsync(
                    predicate: c => c.ID == cityId,
                    include: source => source
                       .Include(c => c.CityAdministration)
                           .ThenInclude(t => t.AdminType)
                       .Include(k => k.CityAdministration)
                           .ThenInclude(a => a.User)
                       .Include(m => m.CityMembers)
                           .ThenInclude(u => u.User)
                       .Include(l => l.CityDocuments)
                           .ThenInclude(d => d.CityDocumentType)
                       .Include(r => r.Region));

            return _mapper.Map<DataAccessCity.City, CityDTO>(city);
        }

        public async Task<CityProfileDTO> GetCityProfileAsync(int cityId)
        {
            var city = await GetByIdAsync(cityId);
            if (city == null)
            {
                return null;
            }

            var cityHead = city.CityAdministration?
                .FirstOrDefault(a => a.AdminType.AdminTypeName == "Голова Станиці"
                    && (DateTime.Now < a.EndDate || a.EndDate == null));
            var cityAdmins = city.CityAdministration
                .Where(a => a.AdminType.AdminTypeName != "Голова Станиці"
                    && (DateTime.Now < a.EndDate || a.EndDate == null))
                .Take(6)
                .ToList();
            var members = city.CityMembers
                .Where(m => m.IsApproved)
                .Take(6)
                .ToList();
            var followers = city.CityMembers
                .Where(m => !m.IsApproved)
                .Take(6)
                .ToList();
            var cityDoc = city.CityDocuments.Take(4).ToList();

            var cityProfileDto = new CityProfileDTO
            {
                City = city,
                Head = cityHead,
                Members = members,
                Followers = followers,
                Admins = cityAdmins,
                Documents = cityDoc,     
            };
            cityProfileDto.City.CanEdit = true;
            cityProfileDto.City.CanJoin = true;
            cityProfileDto.City.CanApprove = true;
            cityProfileDto.City.CanSeeReports = true;
            cityProfileDto.City.CanAddReports = true;

            return cityProfileDto;
        }

        public async Task<CityProfileDTO> GetCityMembersAsync(int cityId)
        {
            var city = await GetByIdAsync(cityId);
            if (city == null)
            {
                return null;
            }

            var cityHead = city.CityAdministration?
                .FirstOrDefault(a => a.AdminType.AdminTypeName == "Голова Станиці"
                    && (DateTime.Now < a.EndDate || a.EndDate == null));
            var members = city.CityMembers
                .Where(m => m.IsApproved)
                .ToList();

            var cityProfileDto = new CityProfileDTO
            {
                City = city,
                Members = members,
                Head = cityHead
            };
            cityProfileDto.City.CanApprove = true;

            return cityProfileDto;
        }

        public async Task<CityProfileDTO> GetCityFollowersAsync(int cityId)
        {
            var city = await GetByIdAsync(cityId);
            if (city == null)
            {
                return null;
            }

            var cityHead = city.CityAdministration?
                .FirstOrDefault(a => a.AdminType.AdminTypeName == "Голова Станиці"
                    && (DateTime.Now < a.EndDate || a.EndDate == null));
            var followers = city.CityMembers
                .Where(m => !m.IsApproved)
                .ToList();

            var cityProfileDto = new CityProfileDTO
            {
                City = city,
                Followers = followers,
                Head = cityHead
            };
            cityProfileDto.City.CanApprove = true;

            return cityProfileDto;
        }

        public async Task<CityProfileDTO> GetCityAdminsAsync(int cityId)
        {
            var city = await GetByIdAsync(cityId);
            if (city == null)
            {
                return null;
            }

            var cityHead = city.CityAdministration?
                .FirstOrDefault(a => a.AdminType.AdminTypeName == "Голова Станиці"
                    && (DateTime.Now < a.EndDate || a.EndDate == null));
            var cityAdmins = city.CityAdministration
                .Where(a => a.AdminType.AdminTypeName != "Голова Станиці"
                    && (DateTime.Now < a.EndDate || a.EndDate == null))
                .ToList();

            var cityProfileDto = new CityProfileDTO
            {
                City = city,
                Admins = cityAdmins,
                Head = cityHead
            };
            cityProfileDto.City.CanApprove = true;

            return cityProfileDto;
        }

        public async Task<CityProfileDTO> GetCityDocumentsAsync(int cityId)
        {
            var city = await GetByIdAsync(cityId);
            if (city == null)
            {
                return null;
            }

            var cityHead = city.CityAdministration?
                .FirstOrDefault(a => a.AdminType.AdminTypeName == "Голова Станиці"
                    && (a.EndDate < DateTime.Now || a.EndDate == null));
            var cityDoc = city.CityDocuments.ToList();

            var cityProfileDto = new CityProfileDTO
            {
                City = city,
                Documents = cityDoc,
                Head = cityHead
            };
            cityProfileDto.City.CanAddReports = true;

            return cityProfileDto;
        }

        public async Task<string> GetLogoBase64(string logoName)
        {
            var logoBase64 = await _cityBlobStorage.GetBlobBase64Async(logoName);

            return logoBase64;
        }

        public async Task<CityProfileDTO> EditAsync(int cityId)
        {
            var city = await GetByIdAsync(cityId);
            if (city == null)
            {
                return null;
            }

            var cityAdmins = city.CityAdministration
                .ToList();
            var members = city.CityMembers
                .Where(p => cityAdmins.All(a => a.UserId != p.UserId))
                .Where(m => m.IsApproved)
                .ToList();
            var followers = city.CityMembers
                .Where(m => !m.IsApproved)
                .ToList();

            var cityProfileDto = new CityProfileDTO
            {
                City = city,
                Admins = cityAdmins,
                Members = members,
                Followers = followers
            };

            return cityProfileDto;
        }

        public async Task EditAsync(CityProfileDTO model, IFormFile file)
        {
            await UploadPhotoAsync(model.City, file);
            var city = await CreateCityAndRegionAsync(model);

            _repoWrapper.City.Attach(city);
            _repoWrapper.City.Update(city);
            await _repoWrapper.SaveAsync();
        }

        public async Task EditAsync(CityDTO model)
        {
            await UploadPhotoAsync(model);
            var city = await CreateCityAsync(model);

            _repoWrapper.City.Attach(city);
            _repoWrapper.City.Update(city);
            await _repoWrapper.SaveAsync();
        }

        public async Task<int> CreateAsync(CityProfileDTO model, IFormFile file)
        {
            await UploadPhotoAsync(model.City, file);
            var city = await CreateCityAndRegionAsync(model);

            _repoWrapper.City.Attach(city);
            await _repoWrapper.City.CreateAsync(city);
            await _repoWrapper.SaveAsync();

            return city.ID;
        }

        public async Task<int> CreateAsync(CityDTO model)
        {
            await UploadPhotoAsync(model);
            var city = await CreateCityAsync(model);

            _repoWrapper.City.Attach(city);
            await _repoWrapper.City.CreateAsync(city);
            await _repoWrapper.SaveAsync();

            return city.ID;
        }

        private async Task<DataAccessCity.City> CreateCityAndRegionAsync(CityProfileDTO model)
        {
            var cityDto = model.City;

            var city = _mapper.Map<CityDTO, DataAccessCity.City>(cityDto);
            var region = await _repoWrapper.Region.GetFirstOrDefaultAsync(r => r.RegionName == city.Region.RegionName);

            if (region == null)
            {
                region = new DataAccessCity.Region
                {
                    RegionName = city.Region.RegionName
                };

                await _repoWrapper.Region.CreateAsync(region);
                await _repoWrapper.SaveAsync();
            }

            city.RegionId = region.ID;
            city.Region = region;

            return city;
        }

        private async Task<DataAccessCity.City> CreateCityAsync(CityDTO model)
        {
            var city = _mapper.Map<CityDTO, DataAccessCity.City>(model);
            var region = await _repoWrapper.Region.GetFirstOrDefaultAsync(r => r.RegionName == city.Region.RegionName);
            
            city.RegionId = region.ID;
            city.Region = region;

            return city;
        }

        private async Task UploadPhotoAsync(CityDTO city, IFormFile file)
        {
            var cityId = city.ID;
            var oldImageName = (await _repoWrapper.City.GetFirstOrDefaultAsync(
                predicate: i => i.ID == cityId))
                ?.Logo;
            
            if (file != null && file.Length > 0)
            {
                using (var img = Image.FromStream(file.OpenReadStream()))
                {
                    var uploads = Path.Combine(_env.WebRootPath, "images\\Cities");
                    if (!string.IsNullOrEmpty(oldImageName))
                    {
                        var oldPath = Path.Combine(uploads, oldImageName);
                        if (File.Exists(oldPath))
                        {
                            File.Delete(oldPath);
                        }
                    }

                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploads, fileName);
                    img.Save(filePath);
                    city.Logo = fileName;
                }
            }
            else
            {
                city.Logo = oldImageName ?? null;
            }
        }

        private async Task UploadPhotoAsync(CityDTO city)
        {
            var oldImageName = (await _repoWrapper.City.GetFirstOrDefaultAsync(i => i.ID == city.ID))?.Logo;
            var logoBase64 = city.Logo;

            if (!string.IsNullOrWhiteSpace(logoBase64) && logoBase64.Length > 0)
            {
                var logoBase64Parts = logoBase64.Split(',');
                var extension = logoBase64Parts[0].Split(new[] { '/', ';' }, 3)[1];
                extension = extension != ""
                    ? (extension[0] == '.' ? "" : "." + extension)
                    : extension;
                
                var fileName = Guid.NewGuid() + extension;

                await _cityBlobStorage.UploadBlobForBase64Async(logoBase64Parts[1], fileName);
                city.Logo = fileName;

                if (!string.IsNullOrEmpty(oldImageName))
                {
                    await _cityBlobStorage.DeleteBlobAsync(oldImageName);
                }
            }
            else
            {
                city.Logo = oldImageName ?? null;
            }
        }
    }
}