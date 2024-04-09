using ApplicationCore.Models;
using ApplicationCore.Models.QuizAggregate;
using AutoMapper;
using Infrastructure.EF.Entities;

namespace Infrastructure.EF.Mappers;

public class AutoMapper : Profile
{

    public AutoMapper()
    {
        CreateMap<QuizItemEntity, QuizItem>();
        CreateMap<QuizEntity, Quiz>();
        CreateMap<QuizItemUserAnswerEntity, QuizItemUserAnswer>();
    }
}

