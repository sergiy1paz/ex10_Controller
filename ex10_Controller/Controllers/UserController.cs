using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ex10_Controller.Database;
using ex10_Controller.Models;
using Microsoft.EntityFrameworkCore;

namespace ex10_Controller.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationContext _db;

        public UserController(ApplicationContext db)
        {
            _db = db;
        }

        //return collection (get)
        /*
         * 
         * Route for query:
         *      api/users/all
         *  
         * **/
        [HttpGet("all")]
        public async Task<IActionResult> GetUsers()
        {
            return new ObjectResult(await _db.Users.ToListAsync());
        }



        //return user by id (get)
        /*
         * Route for query:
         *      api/users/get?id={necessary_id} 
         * 
         * **/
        [HttpGet("get")]
        public async Task<IActionResult> GetUserById(int? id) // [FromQuery]
        {
            if (id is not null)
            {
                var user = await _db.Users.FirstOrDefaultAsync(user => user.Id == id.Value);
                if (user is not null)
                {
                    return Ok(user);
                }
                return BadRequest("User with same id does not exist!");
            } else
            {
                return BadRequest("Error request!");
            } 
        }


        // add(post)
        /*
         * Route for query:
         *      api/users/add
         *      
         * Body:
         *      {
         *          "name" : "some_name",
         *          "position" : "some_position"
         *      }
         * 
         * **/
        [HttpPost("add")]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            await _db.Users.AddAsync(user);
            try
            {
                await _db.SaveChangesAsync();
            } catch (Exception)
            {
                return BadRequest("Error request!");
            }
            return Ok("User was added!");
        }


        // update(put)
        /*
         * Routes for query:
         *      api/users/{some_id}?name={name_to_change}
         *      api/users/{some_id}?position={position_to_change}
         *      api/users/{some_id}?name={name_to_change}&position={position_to_change}
         * 
         * **/
        [HttpPut("update/{necessaryId:int}")] // [FromRoute]
        public async Task<IActionResult> UpdateUser(int necessaryId, string name, string position)
        {
            var user = await _db.Users.FirstOrDefaultAsync(user => user.Id == necessaryId);

            if (user is not null)
            {
                if (name is not null)
                {
                    user.Name = name;
                }

                if (position is not null)
                {
                    user.Position = position;
                }

                await _db.SaveChangesAsync();
                return Ok("User was updated!");
            }

            return BadRequest("Such user was not found!");
        }

        // delete
        /*
         * Route for query:
         *      api/users/delete
         *      
         * Headers:
         *      idToDelete : {some_id}
         * 
         * **/
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser([FromHeader] int idToDelete)
        {
            var user = await _db.Users.FirstOrDefaultAsync(user => user.Id == idToDelete);

            if (user is not null)
            {
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
                return Ok("User was deleted!");
            }

            return BadRequest("User with same id does not exist!");
        }
    }
}
