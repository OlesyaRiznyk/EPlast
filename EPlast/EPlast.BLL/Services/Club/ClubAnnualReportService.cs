﻿using AutoMapper;
using EPlast.BLL.DTO.Club;
using EPlast.BLL.Interfaces.Club;
using EPlast.DataAccess.Entities;
using EPlast.DataAccess.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EPlast.BLL.Services.Club
{
    public class ClubAnnualReportService: IClubAnnualReportService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly UserManager<User> _userManager;
        private readonly IClubAccessService _clubAccessService;
        private readonly IMapper _mapper;

        public ClubAnnualReportService(IRepositoryWrapper repositoryWrapper,
                                    UserManager<User> userManager, IClubAccessService clubAccessService, IMapper mapper)
        {
            _repositoryWrapper = repositoryWrapper;
            _userManager = userManager;
            _clubAccessService = clubAccessService;
            _mapper = mapper;
        }

        ///<inheritdoc/>

        public async Task<ClubAnnualReportDTO> GetByIdAsync(ClaimsPrincipal claimsPrincipal, int id)
        {
            var clubAnnualReport = await _repositoryWrapper.ClubAnnualReports.GetFirstOrDefaultAsync(
                    predicate: a => a.ID == id,
                    include: source => source
                        .Include(a => a.Club));
            return await _clubAccessService.HasAccessAsync(claimsPrincipal, clubAnnualReport.Club.ID) ? _mapper.Map<ClubAnnualReport, ClubAnnualReportDTO>(clubAnnualReport)
                : throw new UnauthorizedAccessException();
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<ClubAnnualReportDTO>> GetAllAsync(ClaimsPrincipal claimsPrincipal)
        {
            var annualReports = await _repositoryWrapper.ClubAnnualReports.GetAllAsync(
                    include: source => source
                        .Include(ar => ar.Club));
            var citiesDTO = await _clubAccessService.GetClubsAsync(claimsPrincipal);
            var filteredAnnualReports = annualReports.Where(ar => citiesDTO.Any(c => c.ID == ar.Club.ID));
            return _mapper.Map<IEnumerable<ClubAnnualReport>, IEnumerable<ClubAnnualReportDTO>>(annualReports);
        }

        public async Task CreateAsync(ClaimsPrincipal claimsPrincipal, ClubAnnualReportDTO clubAnnualReportDTO)
        {
            var club = await _repositoryWrapper.Club.GetFirstOrDefaultAsync(
                predicate: a => a.ID == clubAnnualReportDTO.Club.ID);
            if (await CheckCreated(club.ID))
            {
                throw new InvalidOperationException();
            }
            if (!await _clubAccessService.HasAccessAsync(claimsPrincipal, club.ID))
            {
                throw new UnauthorizedAccessException();
            }
            var clubAnnualReport = _mapper.Map<ClubAnnualReportDTO, ClubAnnualReport>(clubAnnualReportDTO);
            clubAnnualReport.Date = DateTime.Now;
            await _repositoryWrapper.ClubAnnualReports.CreateAsync(clubAnnualReport);
            await _repositoryWrapper.SaveAsync();

        }

        private async Task<bool> CheckCreated(int Id)
        {
            return await _repositoryWrapper.ClubAnnualReports.GetFirstOrDefaultAsync(
                predicate: a => a.Club.ID == Id && a.Date.Year == DateTime.Now.Year) != null;
        }
    }
}
