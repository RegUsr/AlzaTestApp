// ------------------------------------------------------------------------------------------------
// <copyright file="Department.cs" company="Peter Tomciak">
//   Copyright (c) 2021 by Peter Tomciak
// </copyright>
// <summary>
//   Defines the Department type.
// </summary>
// ------------------------------------------------------------------------------------------------
namespace AlzaTestApp.Models
{
    public class Department
    {
        #region Properties
        public string Name { get; set; }
        public string SeoName { get; set; }
        public string Url { get; set; }
        public Meta Meta { get; set; }

        #endregion
    }
}
