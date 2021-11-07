// ------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Peter Tomciak">
//   Copyright (c) 2021 by Peter Tomciak
// </copyright>
// <summary>
//   Defines the Program type.
//
//      https:// webapi.alza.cz/api/career/v2/positions/softwarovy-tester?country=cz 
//      https:// webapi.alza.cz/api/career/v2/positions/skladnik-alza?country=cz
//      https:// www.alza.cz/kariera/pozice/softwarovy-tester
//
// </summary>
// ------------------------------------------------------------------------------------------------
namespace AlzaTestApp
{
    using Helpers;
    using WebApiTests;
    using System;

    class Program
    {
        #region Methods

        /// <summary>
        /// Vstupní bod do aplikace, zde se spouští testy přímo z prostředí Visual Studia<br/>
        /// pomocí F5 / Ctrl+F5. Výstupy směřují do konzole + logovacího souboru.<br/>
        /// NLog.config obsahuje definici cesty pro logFile.txt.
        /// </summary>
        static void Main(string[] args)
        {
            var resultMsg = JobWebApiTest.MainPrgRunner()
                ? "PASS" : "FAILED";

            Log.InfoPlain();
            Log.InfoPlain($"TestAPP final result: {resultMsg}.");

            // Konec
            Console.ReadKey();

            #region Others

            // ------------------------------------------------------------------------------------
            // Implementations:
            // ------------------------------------------------------------------------------------
            // ---
            // DONE: #0 Check > job.SeoName                    --> "seoName": "softwarovy-tester"
            // DONE: #0 Check > job.Name                       --> "name": "Softwarový Tester"
            // DONE: #0 Check > job.HrefLangs[].LanguageCode   --> "languageCode": "cs-CZ"
            // DONE: #0 Check > job.HrefLangs[].Url            --> "url": "..."
            // ---
            // DONE: #1 Download > job.PositionItems.Meta.Href --> "items"."content"."Type"
            // ---
            // DONE: #2 Check > job.Workplace                  --> "workplace": "ČR"
            // ---
            // DONE: #2 Check > job.PlaceOfEmployment.Name     --> "name": "Hall office park"
            // DONE: #2 Check > job.PlaceOfEmployment.State    --> "state": "Česká republika"
            // DONE: #2 Check > job.PlaceOfEmployment.City     --> "city": "Praha"
            // ---
            // DONE: #2 Check > job.Department.SeoName         --> "seoName": "it"
            // DONE: #2 Check > job.Department.Name            --> "name": "IT"
            // DONE: #2 Check > job.Department.Url             --> "url": "/kariera/oddeleni/it"
            // DONE: #2 Check > job.Department.Meta.Href       --> "url": "https://..."
            // ---
            // DONE: #3 Check -> job.ForStudents               --> "forStudents": true/false
            // ---
            // DONE: #4 Download > job.ExecutiveUser.Meta.Href --> "name"."image"."description"
            // DONE: #4 Download > job.GestorUser.Meta.Href    --> "name"."image"."description"
            // ---
            // ------------------------------------------------------------------------------------

            #endregion
        }

        #endregion
    }
}
