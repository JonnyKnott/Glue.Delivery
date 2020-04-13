using System.Collections;
using System.Collections.Generic;
using AutoMapper;
using Glue.Delivery.Models.ServiceModels.Delivery;
using ServiceModels = Glue.Delivery.Models.ServiceModels;
using ApiModels = Glue.Delivery.Models.ApiModels;

namespace Glue.Delivery.WebApi.Mapping
{
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<ApiModels.Delivery.DeliveryRequest, ServiceModels.Delivery.DeliveryRecord>()
                .ForMember(
                    member => member.AccessWindowEndTime, 
                    x => x.MapFrom(
                        req => req.AccessWindow.EndTime)
                )
                .ForMember(
                    member => member.AccessWindowStartTime, 
                    x => x.MapFrom(
                        req => req.AccessWindow.StartTime)
                );
            CreateMap<ServiceModels.Delivery.DeliveryRecord, ApiModels.Delivery.DeliveryResponse>().ForMember(
                    member => member.AccessWindow, 
                    x => x.MapFrom(
                        req => new AccessWindow{ EndTime = req.AccessWindowEndTime, StartTime = req.AccessWindowStartTime})
                );
            CreateMap<ApiModels.Delivery.Recipient, Recipient>().ReverseMap();
            CreateMap<ApiModels.Delivery.AccessWindow, AccessWindow>().ReverseMap();
            CreateMap<ApiModels.Delivery.Order, Order>().ReverseMap();
        }
    }
}