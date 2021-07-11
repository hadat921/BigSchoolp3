using BigSchool.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BigSchool.Controllers
{
    public class AttendancesController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Attend (Course attendanceDto)
        {
            var userID = User.Identity.GetUserId();
            BigSchoolContext1 context = new BigSchoolContext1();
            if (context.Attendance.Any(p => p.Attendee == userID && p.CourseId== attendanceDto.Id))
            {
                return BadRequest("The attendance alreadly exists!");
            }
            var attendance = new Attendance()
            {
                CourseId = attendanceDto.Id,
                Attendee = User.Identity.GetUserId()
            };
            context.Attendance.Add(attendance);
            context.SaveChanges();
            return Ok();

            }

        }
    }

