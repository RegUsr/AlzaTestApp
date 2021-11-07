// ------------------------------------------------------------------------------------------------
// <copyright file="TestResult.cs" company="Peter Tomciak">
//   Copyright (c) 2021 by Peter Tomciak
// </copyright>
// <summary>
//   Defines the TestResult type.
// </summary>
// ------------------------------------------------------------------------------------------------
namespace AlzaTestApp.Models
{
    public class Result
    {
        #region Fields

        private int _testsPass = 0;

        private int _testsFail = 0;

        #endregion


        #region Properties

        public string PassedCnt => _testsPass.ToString().PadLeft(2, '0');
        public string FailedCnt => _testsFail.ToString().PadLeft(2, '0');
        public bool FinalStatus => _testsFail == 0;

        #endregion


        #region Methods


        public void IncreasePassed()
        {
            _testsPass++;
        }

        public void IncreaseFailed()
        {
            _testsFail++;
        }

        #endregion

    }
}

