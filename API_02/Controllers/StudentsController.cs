using API_02.Models;
using API_02.Models.API;
using API_02.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace API_02.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        #region Dependency
        private readonly ApplicationDbContext _context;
        private readonly ApiResponce _apiResponce;

        #endregion

        #region Constructor
        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
            _apiResponce = new ApiResponce();
        }
        #endregion

        #region GetAll
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DtoStudent))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DtoStudent))]
        public async Task<ApiResponce> GetAllStudent()
        {
            try
            {
                IQueryable<DtoStudent> AllStudent = _context.Students.Select(s => new DtoStudent
                {
                    Id = s.Id,
                    Name = s.Name,
                    Division = s.Division,
                    Standard = s.Standard
                });

                _apiResponce.Result = AllStudent;
                _apiResponce.IsSuccess = true;
                _apiResponce.HttpStatus = HttpStatusCode.OK;

                return _apiResponce;
            }
            catch (Exception exception)
            {
                _apiResponce.Result = exception;
                _apiResponce.ErrorMessage = exception.Message;
                _apiResponce.HttpStatus = HttpStatusCode.BadRequest;
                _apiResponce.IsSuccess = false;

                return _apiResponce;
            }
        }
        #endregion

        #region GetById 
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DtoStudent))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DtoStudent))]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable, Type = typeof(DtoStudent))]
        public async Task<ApiResponce> GetById(int id)
        {
            try
            {
                if (id == null || id == 0)
                {
                    _apiResponce.HttpStatus = HttpStatusCode.BadRequest;
                    return _apiResponce;
                };

                var IdFromDb = await _context.Students.FindAsync(id);

                if (IdFromDb == null)
                {
                    _apiResponce.HttpStatus = HttpStatusCode.NotFound;
                    return _apiResponce;
                }
                        
                var GetStudent = new DtoStudent
                {
                    Id = IdFromDb.Id,
                    Name = IdFromDb.Name,
                    Division = IdFromDb.Division,
                    Standard = IdFromDb.Standard
                };

                _apiResponce.Result = GetStudent;
                _apiResponce.HttpStatus = HttpStatusCode.OK;
                _apiResponce.IsSuccess = true;

                return _apiResponce;
                
            }
            catch (Exception exception)
            {
                _apiResponce.Result = exception.HResult;
              _apiResponce.ErrorMessage = exception.Message;
                _apiResponce.IsSuccess = false;
                _apiResponce.HttpStatus = HttpStatusCode.NotAcceptable;

                return _apiResponce;
            }
        }
        #endregion

        #region POST
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(DtoStudent))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DtoStudent))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(DtoStudent))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DtoStudent))]

        public async Task<ApiResponce> Create(DtoAddStudent addStudent)
        {
            try
            {
                Student NewStudent = new Student
                {
                    Name = addStudent.Name,
                    Division = addStudent.Division,
                    Standard = addStudent.Standard,
                    IsActive = true
                };

                await _context.Students.AddAsync(NewStudent);
                await _context.SaveChangesAsync();

                DtoStudent dtoStudent = new DtoStudent
                {
                    Id = NewStudent.Id,
                    Name = NewStudent.Name,
                    Division = NewStudent.Division,
                    Standard = NewStudent.Standard,
                };

                _apiResponce.Result = dtoStudent;
                _apiResponce.HttpStatus = HttpStatusCode.Created;
                return _apiResponce;
            }
            catch (Exception excption)
            {
                _apiResponce.ErrorMessage += excption.Message;
                _apiResponce.Result = excption;
                _apiResponce.HttpStatus = HttpStatusCode.NotFound;
                _apiResponce.IsSuccess = false;
                return _apiResponce;
            } 
        }
        #endregion

        #region PUT
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed, Type = typeof(DtoStudent))]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(DtoStudent))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(DtoStudent))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DtoStudent))]

        public async Task<ApiResponce> Update(int id, DtoStudent dtoStudent)
        {
            if (id == null || id == 0)
            {
                _apiResponce.HttpStatus = HttpStatusCode.NotFound; 
                return _apiResponce;
            }

            try
            {
                var IdFromDb = await _context.Students.SingleOrDefaultAsync(e => e.Id == id);
                if(IdFromDb == null)
                {
                    _apiResponce.HttpStatus= HttpStatusCode.NotFound;
                    return _apiResponce;
                }

                IdFromDb.Name = dtoStudent.Name;
                IdFromDb.Division = dtoStudent.Division;
                IdFromDb.Standard = dtoStudent.Standard;
                

                _context.Students.Update(IdFromDb);
                await _context.SaveChangesAsync();

                DtoStudent Updated = new DtoStudent
                {
                    Id = IdFromDb.Id,
                    Name = IdFromDb.Name,
                    Division = IdFromDb.Division,
                    Standard = IdFromDb.Standard
                };

                _apiResponce.Result = Updated;
                _apiResponce.HttpStatus = HttpStatusCode.NoContent;
                _apiResponce.IsSuccess = true;
                return _apiResponce;
            }
            catch (Exception exception)
            {
                _apiResponce.Result = exception;
                _apiResponce.ErrorMessage = exception.Message;
                _apiResponce.IsSuccess = false;
                _apiResponce.HttpStatus = HttpStatusCode.MethodNotAllowed;
                  
                return _apiResponce;
            }

        }
        #endregion

        #region Patch
        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(DtoStudent))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(DtoStudent))]
        [ProducesResponseType(StatusCodes.Status409Conflict,Type = typeof(DtoStudent))]
        [ProducesResponseType(StatusCodes.Status200OK,Type = typeof(DtoStudent))]
        public async Task<ApiResponce> PartialUpdate(int id, JsonPatchDocument<DtoStudent> jsonPatch)
        {
            if (id == null || id == 0)
            {
                _apiResponce.HttpStatus = HttpStatusCode.NotFound;
                return _apiResponce;
            }

                try
                {
                Student? IdFromDb = await _context.Students.AsNoTracking<Student>().SingleOrDefaultAsync(e => e.Id == id);
                if (IdFromDb == null)
                {
                    _apiResponce.HttpStatus = HttpStatusCode.BadRequest;
                    return _apiResponce;
                }

                    DtoStudent student = new()
                {
                    Id = IdFromDb.Id,
                    Name = IdFromDb.Name,
                    Division = IdFromDb.Division,
                    Standard = IdFromDb.Standard,
                };

                jsonPatch.ApplyTo(student);

                Student UpdateStudent = new()
                {
                  Id = IdFromDb.Id,
                  Name = student.Name,
                  Division = student.Division,
                  Standard = student.Standard,
                };

                _context.Students.Update(UpdateStudent);
                _context.SaveChanges();

                DtoStudent PartiallyUpdated = new()
                {
                    Id = student.Id,
                    Name = student.Name,
                    Division = student.Division,
                    Standard = student.Standard
                };

                _apiResponce.Result = PartiallyUpdated;
                _apiResponce.HttpStatus = HttpStatusCode.NoContent;
                return _apiResponce;

            }
            catch (Exception exception)
            {
                _apiResponce.Result = exception;
                _apiResponce.ErrorMessage = exception.Message;
                _apiResponce.IsSuccess = false;
                _apiResponce.HttpStatus = HttpStatusCode.Conflict; 
                return _apiResponce;
            }
        }
        #endregion

        #region Remove
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(DtoStudent))]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(DtoStudent))]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(DtoStudent))]
        public async Task<ApiResponce> Remove(int id)
        {
            if (id == 0 || id == null)
            {
                _apiResponce.HttpStatus = HttpStatusCode.NotFound;
                return _apiResponce;
            };
            try
            {
                var IdFromdb = await _context.Students.FindAsync(id);
                _context.Remove(IdFromdb);
                await _context.SaveChangesAsync();

                _apiResponce.Result = "Deleted";
                _apiResponce.HttpStatus = HttpStatusCode.NoContent;
                return _apiResponce;
            }
            catch (Exception exception)
            {
                _apiResponce.Result = exception;
                _apiResponce.ErrorMessage = exception.Message;
               _apiResponce.IsSuccess=false;
                _apiResponce.HttpStatus = HttpStatusCode.NotFound;

                return _apiResponce;
            }
        }
        #endregion

    }
}
