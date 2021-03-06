﻿using ResumeBuilder.Models;
using ResumeBuilder.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ResumeBuilder.Controllers
{
    public class ProfileController : Controller
    {
        private ResumeBuilderDBContext db;
        public ProfileController()
        {
            db = new ResumeBuilderDBContext();
        }
        // GET: Profile
        public ActionResult Index()
        {
            return View();
        }



        [NonAction]
        private PublicProfileViewModel GetUserDetails()
        {
            var uiModel = new PublicProfileViewModel();
            int id;
            var result = Int32.TryParse(Session["UserID"] as String, out id);
            if (result)
            {
                try
                {
                    // user details
                    var userData = db.UserDetails.FirstOrDefault(a => a.UserID == id);

                    // User Name
                    uiModel.Name = userData.Name;

                    // User Gender
                    uiModel.Gender = userData.Gender;

                    // User Gender
                    uiModel.DOB = (userData.DateOfBirth.ToString().Split(' '))[0];

                    // User Phone
                    uiModel.PhoneNumber = userData.Phone;

                    // User E-mail
                    uiModel.Email = userData.Login.Username;

                    // User Summary
                    uiModel.Summary = userData.Summary;

                    // Education Details
                    //uiModel.EducationList = (from user in userData.EducationalDetails
                    //                          select new EducationUIModel
                    //                          {
                    //                              CourseName = (userData.EducationalDetails.FirstOrDefault
                    //                              db.Courses.FirstOrDefault(x => x.CourseId == user.CourseId).CourseName == "10"
                    //                                           || db.Courses.FirstOrDefault(x => x.CourseId == user.CourseId).CourseName == "12") ?
                    //                                           db.Courses.FirstOrDefault(x => x.CourseId == user.CourseId).CourseName + " TH" :
                    //                                           db.Courses.FirstOrDefault(x => x.CourseId == user.CourseId).CourseName,
                    //                              CGPAOrPercentage = user.CGPAOrPercentage,
                    //                              Board = user.Board,
                    //                              Stream = (user.Stream == null) ? "N/A" : user.Stream,
                    //                              TotalPercentorCGPAValue = user.TotalPercentorCGPAValue,
                    //                              PassingYear = user.PassingYear
                    //                          }).OrderByDescending(x => x.PassingYear).ToList();

                    // Skills
                    uiModel.SkillList = userData.Skills.Select(a => a.SkillName).ToList();

                    // Project Details
                    uiModel.ProjectList = (from user in userData.Projects.Where(x => x.UserID == id)
                                            select new ProjectUIModel
                                            {
                                                Title = user.ProjectTitle,
                                                Description = user.Description,
                                                Duration = user.DurationInMonth
                                            }).ToList();

                    // Work Ex.
                    uiModel.WorkExList = (from user in db.WorkExperiences.Where(x => x.UserID == id)
                                           select new WorkExUIModel
                                           {
                                               OrganizationName = user.OrganizationName,
                                               StartMonth = (user.StartMonth <= 9) ? "0" + user.StartMonth : user.StartMonth.ToString(),
                                               StartYear = user.StartYear,
                                               EndMonth = (user.EndMonth <= 9) ? "0" + user.EndMonth : user.EndMonth.ToString(),
                                               EndYear = user.EndYear,
                                               Role = user.Designation,
                                               CurrentlyWorking = user.IsCurrentlyWorking
                                           }).OrderByDescending(x => x.StartYear).ToList();

                    // Languages 
                    uiModel.Languages = userData.Languages.Select(a => a.LanguageName).ToList();

                }
                catch (Exception)
                {
                    uiModel.ErrorMsg = "Unexpected error occured, try again...";
                }
            }
            return uiModel;
        }

        // GET: Resume/Preview
        public ActionResult Preview()
        {
            if (Session["UserID"] != null)
            {
                var uiModel = GetUserDetails();
                return PartialView(uiModel);
            }
            return RedirectToAction("Login", "Account");
        }

        public ActionResult data()
        {
            var x = db.UserDetails.FirstOrDefault(a => a.UserID == 1).Name;
            return Json(x,JsonRequestBehavior.AllowGet);
        }
    }
}