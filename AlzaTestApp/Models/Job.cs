// ------------------------------------------------------------------------------------------------
// <copyright file="Job.cs" company="Peter Tomciak">
//   Copyright (c) 2021 by Peter Tomciak
// </copyright>
// <summary>
//   Defines the Job type.
// </summary>
// ------------------------------------------------------------------------------------------------
namespace AlzaTestApp.Models
{
    using System.Collections.Generic;

    public class Job
    {
        #region Properties

        // ShortName pracovní pozice
        public string SeoName { get; set; }

        // Název pracovní pozice
        public string Name { get; set; }

        // Lang/Url dané pracovní pozice
        public List<HrefLangs> HrefLangs { get; set; }

        // Vyplněný popis pozice
        public PositionItems PositionItems { get; set; }

        // Kde bude pracovat #1
        public string Workplace { get; set; }

        // Kde bude pracovat #2
        public PlaceOfEmployment PlaceOfEmployment { get; set; }

        // Kde bude pracovat #3
        public Department Department { get; set; }

        // Kdo bude na pohovoru #1
        public GestorUser GestorUser { get; set; }

        // Kdo bude na pohovoru #2
        public ExecutiveUser ExecutiveUser { get; set; }

        // Jedná se o job pro studenty?
        public bool? ForStudents { get; set; }

        #endregion
    }
}
