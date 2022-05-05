﻿using System.Collections.Generic;
using System;

//! The EFModels namespace
/*! 
 * This namespace falls under the AbetAPI namespace, and is for EFModels.
 * The EFModels are generally called from the Controllers namespace, to 
 * provide the controllers functionality, ultimately giving endpoints/functionality
 * for the UI elements
 */
namespace AbetApi.Models
{
    //! The StudentOutcomesCompleted Class
    /*! 
     * This class gets called by the StudentOutcomesCompletedController class
     * and provides functions to get and return data
     */
    public class StudentOutcomesCompleted
    {
        //! The ConvertToModelStudentOutcomesCompleted function
        /*! 
         * This function converts data structures and data into a form usable my the StudentOutcomesCompleted in Model
         * \param term The Term (Fall/Spring) for the given semester
         * \param year The year for the given semester
         * \param department Major department, such as CSCE or MEEN
         * \param courseNumber Course identifier, such as 3600 for Systems Programming
         * \param StudentOutcomesCompleted
         */
        public static List<Dictionary<string, string>> ConvertToModelStudentOutcomesCompleted(string term, int year, string department, string courseNumber, List<AbetApi.EFModels.StudentOutcomesCompleted> studentOutcomesCompletedList)
        {
            List<Dictionary<string, string>> tempList = new List<Dictionary<string, string>>();

            //Pulls all the relevant course outcomes in to a list
            List<AbetApi.EFModels.CourseOutcome> courseOutcomeList = AbetApi.EFModels.CourseOutcome.GetCourseOutcomes(term, year, department, courseNumber).Result;

            List<AbetApi.EFModels.Major> majorList = AbetApi.EFModels.Major.GetMajors(term, year).Result;

            //Scan through the list of course outcomes for the provided course. For each outcome, create a dictionary entry for it.
            foreach(EFModels.CourseOutcome courseOutcome in courseOutcomeList)
            {
                Dictionary<string, string> tempDictionary = new Dictionary<string, string>();
                tempDictionary.Add("outcomeName", courseOutcome.Name);
                tempDictionary.Add("outcomeDescription", courseOutcome.Description);

                //Adds blank forms to the object
                if(majorList != null)
                {
                    foreach(var major in majorList)
                    {
                        tempDictionary.Add(major.Name, "0");
                    }
                }

                tempList.Add(tempDictionary);
            }

            //For each studentOutcomesCompleted object, find the associated course outcome and add its entry to the appropriate dictionary
            foreach(var studentOutcomesCompleted in studentOutcomesCompletedList)
            {
                foreach(var dictionary in tempList)
                {
                    if(dictionary["outcomeName"] == studentOutcomesCompleted.CourseOutcomeName)
                    {
                        //dictionary.Add(studentOutcomesCompleted.MajorName, studentOutcomesCompleted.StudentsCompleted.ToString());
                        dictionary[studentOutcomesCompleted.MajorName] = studentOutcomesCompleted.StudentsCompleted.ToString();
                    }
                }
            }

            return tempList;
        }

        //! The ConvertEFToModelStudentOutcomesCompleted function
        /*! 
         * This function converts data structures and data into a form usable my the StudentOutcomesCompleted in EFModel
         * \param term The Term (Fall/Spring) for the given semester
         * \param year The year for the given semester
         * \param department Major department, such as CSCE or MEEN
         * \param courseNumber Course identifier, such as 3600 for Systems Programming
         * \param sectionNumber Course section, such as 001 or 002
         * \param studentOutcomesCompletedDictionary
         */
        public static List<AbetApi.EFModels.StudentOutcomesCompleted> ConvertToEFModelStudentOutcomesCompleted(string term, int year, string department, string courseNumber, string sectionNumber, List<Dictionary<string, string>> studentOutcomesCompletedDictionaries)
        {
            //Get a list of existing majors
            //Scan through each key of the dictionary. If the name matches with an existing major, convert it to a StudentOutcomesCompleted object

            List<AbetApi.EFModels.StudentOutcomesCompleted> studentOutcomesCompletedList = new List<AbetApi.EFModels.StudentOutcomesCompleted>();

            var majorList = AbetApi.EFModels.Major.GetMajors(term, year).Result;

            //For each major, find the corresponding major entry in this provided studentOutcomesCompleted object
            foreach(var major in majorList)
            {
                foreach (Dictionary<string, string> dictionary in studentOutcomesCompletedDictionaries)
                {
                    //For each separate item in the dictionary, validate it is a major name
                    foreach (KeyValuePair<string, string> item in dictionary)
                    {
                        //If it is a major name, turn that major in to a stand alone student outcomes completed object
                        if (item.Key == major.Name)
                        {
                            //Convert the string integer back to an int
                            //This will fail if the value of the students completed string does not convert back in to an integer. This may need more error handling.
                            int tempInt = 0;
                            if (Int32.TryParse(item.Value, out tempInt))
                            {

                                //create a student outcome object
                                studentOutcomesCompletedList.Add(new AbetApi.EFModels.StudentOutcomesCompleted(term, year, department, courseNumber, sectionNumber, dictionary["outcomeName"], item.Key, tempInt));
                            }
                            else
                            {
                                throw new Exception("Data format was incorrect");
                            }
                        }
                    }
                }
            }
            return studentOutcomesCompletedList;
        }
    }
}
