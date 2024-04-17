using AutoMapper;
using Key_Management_System.DTOs.KeyDtos;
using Key_Management_System.Models;

namespace Key_Management_System.Configuration
{
    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration() 
        {
            //add key
            CreateMap<AddKeyDto, Key>();

            //get keys(s)
            CreateMap<Key, GetKeyDto>();


            /*
             Note that, with automapper it works like 
                      source    destination (while adding an object with automapper)
            CreateMap<AddKeyDto, Key>();

            while in getting from db then mapping 
                    source  destination
            CreateMap<Key, GetKeyDto>();


            when mapping the object
                      destination  source
            IMapper.Map<GetKeyDto>(Key)
             */
        }
    }
}
