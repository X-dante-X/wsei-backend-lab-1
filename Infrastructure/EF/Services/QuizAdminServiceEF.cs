using ApplicationCore.Interfaces.AdminService;
using ApplicationCore.Interfaces.Criteria;
using ApplicationCore.Models.QuizAggregate;
using AutoMapper;
using Infrastructure.EF.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EF.Services;
public class QuizAdminServiceEF : IQuizAdminService
{
    private readonly QuizDbContext _context;
    private readonly IMapper _mapper;

    public QuizAdminServiceEF(QuizDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public Quiz AddQuiz(Quiz quiz)
    {
        if (quiz == null)
            throw new ArgumentNullException(nameof(quiz));

        var quizEntity = _mapper.Map<QuizEntity>(quiz);
        _context.Quizzes.Add(quizEntity);
        _context.SaveChanges();
        return _mapper.Map<Quiz>(quizEntity);
    }

    public QuizItem AddQuizItemToQuiz(int quizId, QuizItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        var quizEntity = _context.Quizzes.Include(q => q.Items).FirstOrDefault(q => q.Id == quizId);
        if (quizEntity == null)
            throw new InvalidOperationException($"Quiz with ID {quizId} not found.");

        var itemEntity = _mapper.Map<QuizItemEntity>(item);
        quizEntity.Items.Add(itemEntity);
        _context.SaveChanges();
        return _mapper.Map<QuizItem>(itemEntity);
    }

    public IQueryable<QuizItem> FindAllQuizItems()
    {
        return _context.QuizItems.Select(q => _mapper.Map<QuizItem>(q)).AsQueryable();
    }

    public IQueryable<Quiz> FindAllQuizzes()
    {
        var quizEntities = _context.Quizzes.ToList();
        var quizzes = _mapper.Map<List<Quiz>>(quizEntities);
        return quizzes.AsQueryable();
    }

    public IEnumerable<Quiz> FindBySpecification(ISpecification<Quiz> specification)
    {
        ArgumentNullException.ThrowIfNull(specification);

        IQueryable<Quiz> query = _context.Quizzes.Select(q => _mapper.Map<Quiz>(q));

        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        query = specification.Includes.Aggregate(query,
                                (current, include) => current.Include(include));

        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        return query.AsEnumerable();
    }

    public void UpdateQuiz(Quiz quiz)
    {
        ArgumentNullException.ThrowIfNull(quiz);

        var existingQuiz = _context.Quizzes.FirstOrDefault(q => q.Id == quiz.Id);
        if (existingQuiz == null)
            throw new InvalidOperationException($"Quiz with ID {quiz.Id} not found.");

        _mapper.Map(quiz, existingQuiz);
        _context.SaveChanges();
    }
}
