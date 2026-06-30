using AssetManagementAPI.DTOs.Asset;
using AssetManagementAPI.DTOs.Employee;
using AssetManagementAPI.DTOs.ServiceRequest;
using AssetManagementAPI.Models;
using AutoMapper;

namespace AssetManagementAPI.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateAssetDTO, Asset>();

            CreateMap<UpdateAssetDTO, Asset>();

            CreateMap<Asset, AssetResponseDTO>();

            CreateMap<Employee, EmployeeResponseDTO>();

            CreateMap<ServiceRequest, ServiceRequestResponseDTO>();
        }
    }
}