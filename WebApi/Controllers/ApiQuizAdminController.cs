using ApplicationCore.Interfaces.AdminService;
using ApplicationCore.Interfaces.UserService;
using ApplicationCore.Models.QuizAggregate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;
using Microsoft.AspNetCore.JsonPatch.Operations;
using static System.Runtime.InteropServices.JavaScript.JSType;
using AutoMapper;
using System.ComponentModel.DataAnnotations;
using WebApi.Validators;
using FluentValidation;

namespace WebApi.Controllers;

[ApiController]
[Route("/api/v1/admin/quizzes")]
public class ApiQuizAdminController : ControllerBase
{
    private readonly IQuizAdminService _adminService;
    private readonly IMapper _mapper;

    public ApiQuizAdminController(IQuizAdminService adminService, IMapper mapper)
    {
        _adminService = adminService;
        _mapper = mapper;
    }

    [HttpPost]
    public IActionResult AddQuiz(LinkGenerator link, NewQuizDto dto)
    {
        var quiz = _adminService.AddQuiz(_mapper.Map<Quiz>(dto));
        return Created(
            link.GetPathByAction(HttpContext, nameof(GetById), null, new { quiId = quiz.Id }),
            quiz
        );
    }

    [HttpGet]
    [Route("{id}")]
    public ActionResult<Quiz> GetById(int id)
    {
        var quiz = _adminService.FindAllQuizzes().FirstOrDefault(q=>q.Id == id);
        if (quiz == null) 
        {
            return NotFound();
        }
        return quiz;
    }

    [HttpPatch]
    [Route("{quizId}")]
    [Consumes("application/json-patch+json")]
    public ActionResult<Quiz> AddItemToQuiz(int quizId, JsonPatchDocument<Quiz> document) 
    {
        var quiz = _adminService.FindAllQuizzes().FirstOrDefault(q => q.Id == quizId);
        var disabled = document.Operations.FirstOrDefault(p => p.OperationType == OperationType.Replace && p.path == "id");

        if (disabled != null)
        {
            return BadRequest(new { error = "Cant replace Id!" });
        }
        if (document == null)
        {
            return BadRequest();
        }
        if (quiz == null) 
        {
            return NotFound(new {error = $"quiz with id = {quizId} not found!"});
        }
        int previousCount = quiz.Items.Count;
        document.ApplyTo(quiz, ModelState);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (previousCount < quiz.Items.Count)
        {
            QuizItem item = quiz.Items[^1];
            quiz.Items.RemoveAt(quiz.Items.Count - 1);
            _adminService.AddQuizItemToQuiz(quizId, item);
        }
        return Ok(_adminService.FindAllQuizzes().FirstOrDefault(q => q.Id == quizId));
    }


}
