﻿using EPlast.DataAccess.Entities;
using System.Collections.Generic;

namespace EPlast.BussinessLayer.DTO
{
    public class CityDTO
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string CityURL { get; set; }
        public string Description { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string OfficeNumber { get; set; }
        public string PostIndex { get; set; }
        public Region Region { get; set; }
        public string Logo { get; set; }
        public IEnumerable<CityDocuments> CityDocuments { get; set; }
        public IEnumerable<CityMembers> CityMembers { get; set; }
        public IEnumerable<UnconfirmedCityMember> UnconfirmedCityMember { get; set; }
        public IEnumerable<CityAdministration> CityAdministration { get; set; }
        public IEnumerable<AnnualReport> AnnualReports { get; set; }
        public IEnumerable<CityLegalStatus> CityLegalStatuses { get; set; }
    }
}