using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ai_addresses_data.Entity;
using AutoMapper;

namespace ai_addresses_services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Gender, Gender>();
            CreateMap<Address, Address>();
            CreateMap<AddressType, AddressType>();
            CreateMap<Country, Country>();
            CreateMap<State, State>();
            CreateMap<Person, Person>();
        }
    }
}
