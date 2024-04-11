using ApplicationCore.Models;
using ApplicationCore.Models.QuizAggregate;
using AutoMapper;
using Infrastructure.EF.Entities;

namespace Infrastructure.EF.Mappers;

public class AutoMappers : Profile
{
    public AutoMappers()
    {
        CreateMap<Quiz, QuizEntity>()
             .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        CreateMap<QuizEntity, Quiz>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        CreateMap<QuizItem, QuizItemEntity>()
            .ForMember(dest => dest.IncorrectAnswers, opt => opt.MapFrom(src => src.IncorrectAnswers.Select(answer => new QuizItemAnswerEntity { Answer = answer })));

        CreateMap<QuizItemEntity, QuizItem>()
            .ForMember(dest => dest.IncorrectAnswers, opt => opt.MapFrom(src => src.IncorrectAnswers.Select(answer => answer.Answer)));

        CreateMap<QuizItemAnswerEntity, string>()
            .ConvertUsing(src => src.Answer);

        CreateMap<string, QuizItemAnswerEntity>()
            .ForMember(dest => dest.QuizItems, opt => opt.Ignore());
    }
}

