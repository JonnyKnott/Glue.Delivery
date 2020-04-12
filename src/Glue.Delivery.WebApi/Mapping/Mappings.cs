using System.Collections;
using System.Collections.Generic;
using AutoMapper;
using ServiceModels = Glue.Delivery.Models.ServiceModels;
using ApiModels = Glue.Delivery.Models.ApiModels;

namespace Glue.Delivery.WebApi.Mapping
{
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<ApiModels.Delivery.DeliveryRecord, ServiceModels.Delivery.DeliveryRecord>().ReverseMap();
            CreateMap<ApiModels.Delivery.Recipient, ServiceModels.Delivery.Recipient>().ReverseMap();
            CreateMap<ApiModels.Delivery.AccessWindow, ServiceModels.Delivery.AccessWindow>().ReverseMap();
            CreateMap<ApiModels.Delivery.Order, ServiceModels.Delivery.Order>().ReverseMap();
            // CreateMap<List<ApiModels.Delivery.DeliveryRecord>, List<ServiceModels.Delivery.DeliveryRecord>>().ReverseMap();
        }
    }
}