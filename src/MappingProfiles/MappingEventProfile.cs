using AutoMapper;
using MMAEvents.ApiClients.Models;

namespace MMAEvents.TelegramBot
{
    public class MappingEventProfile : Profile
    {
        public MappingEventProfile()
        {
            CreateMap<EventData, EventDTO>()
                .ForMember(
                    dto => dto.Date,
                    options => options.MapFrom(src => src.Date.ToDateTimeOffset()))
                .ForMember(
                    dto => dto.FightCard,
                    options => options.MapFrom(src => src.FightCards)
                    );

            CreateMap<FightCardData, FightCardDTO>();

            CreateMap<FightRecordData, FightDTO>();
        }
    }
}