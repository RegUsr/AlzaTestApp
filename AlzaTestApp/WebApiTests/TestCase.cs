// ------------------------------------------------------------------------------------------------
// <copyright file="TestCase.cs" company="Peter Tomciak">
//   Copyright (c) 2021 by Peter Tomciak
// </copyright>
// <summary>
//   Defines the TestCase type.
// </summary>
// ------------------------------------------------------------------------------------------------
namespace AlzaTestApp.WebApiTests
{
    using Helpers;
    using Models;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;

    public class TestCase
    {
        #region Fields

        private readonly Result _result;

        #endregion

        #region Properties
        public string CallerMethodName{ get; private set; }

        public int CaseId { get; init; } = 99;

        public string Msg { get; set; }

        public Status Status { get; set; } = Status.InProgress;

        #endregion

        #region Constructor

        public TestCase(ref Result rs, string message, [CallerMemberName] string callerMethodName = "")
        {
            CallerMethodName = callerMethodName;
            Msg = message;
            
            _result = rs;

            var match = Regex.Match(callerMethodName, @"^\w(\d+)_");

            if (match.Success)
            {
                // Zjištění pořadového čísla testovacího případu z názvu metody.
                CaseId = int.Parse(match.Groups[1].Value);
            }
        }

        #endregion

        #region Methods
        public void LogFailResult()
        {
            _result.IncreaseFailed();
            Status = Status.Fail;

            Log.InfoStatus(Status.ToString(), $@"{Msg}");
        }

        public void LogFailResult(string errorMsg)
        {
            _result.IncreaseFailed();
            Status = Status.Fail;

            Log.InfoStatus(Status.ToString(), $@"{Msg}");
            Log.Error($@"{errorMsg}");
        }


        public void LogPassResult()
        {
            _result.IncreasePassed();
            Status = Status.Pass;
            
            Log.InfoStatus(Status.ToString(), $@"{Msg}");
        }


        public void LogPlainMessage(string msg)
        {
            Log.InfoPlain($@"{msg}");
        }

        #endregion
    }


    #region Enums

    public enum Status
    {
        InProgress,
        Pass,
        Fail
    }

    #endregion
}
