// ------------------------------------------------------------------------------------------------
// <copyright file="WebApiTests.cs" company="Peter Tomciak">
//   Copyright (c) 2021 by Peter Tomciak
// </copyright>
// <summary>
//   END-POINT to test:
//      
//      https:// webapi.alza.cz/api/career/v2/positions/softwarovy-tester?country=cz 
//
// </summary>
// ------------------------------------------------------------------------------------------------
namespace AlzaTestApp.WebApiTests
{
    using Helpers;
    using Models;
    using RestSharp;
    using RestSharp.Serializers.NewtonsoftJson;
    using System;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Text.RegularExpressions;

    public static class JobWebApiTest
    {
        #region Constants

        /////////////////////////////-----------------------------------------------//////////
        //                                                                                  //
        //  Nastavení vstupních parametrů pro samotnou aplikaci                             //
        //                                                                                  //
        ////////----------------------------------------------------//////////////////////////
        /// 

        // Definice konstanty Uniform Resource Locator.
        private const string ApiBaseUrl = "https://webapi.alza.cz";

        // Definice konstanty Uniform Resource Indentifier.
        private const string ApiUri = "/api/career/v2/positions/";

        // ID pracovní pozice ve formátu SEO. Její načtená data jsou předmětem testu.
        private const string ApiParJobName = "softwarovy-tester";

        // ID krajiny/státu, kde je pracovní pozice nabízena.
        private const string ApiParJobCountry = "country=cz";

        // ------------------------------/////////////////////////////////////////////////////

        #endregion

        #region Fields

        private static string _apiBaseUrl;
        private static string _apiParJobName;
        private static string _apiParJobCountry;
        private static string _apiUri;
        private static string _apiUriForExecUsr;
        private static string _apiUriForGestUsr;
        private static string _apiUriForJobDesc;
        private static string _apiUriWithParams;

        private static RestClient _client;
        private static RestRequest _request;
        private static IRestResponse<Job> _respJob;
        private static IRestResponse<JobDescription> _respJobDescr;
        private static IRestResponse<User> _respJobExecUser;
        private static IRestResponse<User> _respJobGestUser;
        private static HttpStatusCode _statusCode;

        private static Job _job;
        private static JobDescription _jobDescr;
        private static User _jobExecUser;
        private static User _jobGestUser;

        private static Result _rs = new Result();

        #endregion

        #region Methods

        public static bool MainPrgRunner()
        {
            // --------------------------------------------------------------------------------
            // Provedení operací typu connect, request, response na definovaný end-point,
            // v rámci těchto operací dojde k provedení tzv. infrastrukturních testů,
            // někdy označovaných taky tzv. smoke testy.
            //
            // Jakýkoliv FAIL status znamená STOP v provádění dalších operací,
            // protože není z jakéhokoliv důvodu funkční/přístupný tech. stack
            // a bez něho skutečně nelze dále pokračovat v exekuci testů.
            // --------------------------------------------------------------------------------
            //

            Log.InitLogService();
            Log.TraceMethodStart();

            Log.Trace($"Main DLL location: {Assembly.GetExecutingAssembly().Location}");
            Log.Trace($"Main DLL version: {Assembly.GetExecutingAssembly().GetName().Version}");

            try
            {
                #region Test Cases

                // Infrastructure TestCase #01:
                // Formální a věcná kontrola obsahu ApiBaseUrl.
                //
                T01_Check_ApiBaseUrl(ApiBaseUrl);

                // Infrastructure TestCase #02:
                // Formální a věcná kontrola obsahu ApiUri.
                //
                T02_Check_ApiUri(ApiUri);

                // Infrastructure TestCase #03:
                // Formální a věcná kontrola obsahu ApiParJobName.
                //
                T03_Check_ApiParJobName(ApiParJobName);

                // Infrastructure TestCase #04:
                // Formální a věcná kontrola obsahu ApiParJobCountry.
                //
                T04_Check_ApiParJobCountry(ApiParJobCountry);

                // Infrastructure TestCase #05:
                // Kontrola formální správnosti vyskládaného ApiUri.
                //
                T05_Check_ApiUriWithPars();

                // Infrastructure TestCase #06:
                // Formální kontrola instance RestClient.
                //
                T06_CreateCheck_Client();

                // Infrastructure TestCase #07:
                // Formální a věcná kontrola vytvořené instance RestRequest.
                //
                T07_CreateCheck_Request();

                // Infrastructure TestCase #08:
                // Věcná kontrola správnosti obsahu RestRequest headeru.
                //
                T08_ModifyCheck_Request();

                // Infrastructure TestCase #09:
                // Odeslání requestu, příjem a kontrola odpovědi z cílového end-pointu služby.
                //
                T09_SendReceiveCheck_ReqResp();

                // Infrastructure TestCase #10:
                // Formální kontrola přijatých a rozparsovaných Job dat.
                //
                T10_Check_ResponseData();

                // ----------------------------------------------------------------------

                // TestCase #11:
                // Věcná kontrola response dat: Job.SeoName.
                //
                T11_Check_JobSeoName();

                // TestCase #12:
                // Věcná kontrola response dat: Job.Name.
                //
                T12_Check_JobName();

                // TestCase #13:
                // Formální a věcná kontrola response dat: Job.Name.
                //
                T13_Check_JobLangCode();

                // TestCase #14:
                // Formální a věcná kontrola response dat: Job.Url.
                //
                T14_Check_JobUrl();

                // TestCase #15:
                // Formální a věcná kontrola položky pro popis pozice: Job.PositionItems.
                //
                T15_Check_JobDescrUrl();

                // ----------------------------------------------------------------------

                // Infrastructure TestCase #16:
                // Formální a věcná kontrola vytvořené instance RestRequest
                // pro zjištění obsahu job popisu.
                //
                T16_CreateCheck_Request();

                // Infrastructure TestCase #17:
                // Věcná kontrola přidaných doplňkových parametrů RestRequest
                // pro zjištění obsahu job popisu.
                T17_ModifyCheck_Request();

                // Infrastructure TestCase #18:
                // Odeslání requestu, příjem a kontrola odpovědi ze sub end-pointu
                // webové služby, který vrací popis job pozice.
                T18_SendReceiveCheck_ReqResp();

                // Infrastructure TestCase #19:
                // Formální kontrola přijatých a rozparsovaných dat pro popis pozice.
                T19_Check_ResponseData();

                // ----------------------------------------------------------------------

                // TestCase #20:
                // Formální a věcná kontrola obsahu položky Job.Description.
                // (Vyplněný popis pozice).
                //
                T20_Check_JobDescription();

                // TestCase #21:
                // Věcná kontrola obsahu položky přijatých dat: Job.WorkPlace.
                // (Kde se bude pracovat).
                //
                T21_Check_JobWorkPlace();

                // TestCase #22:
                // Věcná kontrola položky: Job.PlaceOfEmployment.
                // (Kde se bude pracovat).
                //
                T22_Check_JobPlaceEmploy();

                // TestCase #23:
                // Věcná kontrola položky: Job.Department.
                // (Kde se bude pracovat).
                //
                T23_Check_JobDepartment();

                // TestCase #24:
                // Formální kontrola položky: Job.ForStudents.
                // (Info, zda-li je to práce pro studenty).
                //
                T24_Check_JobForStudents();

                // TestCase #25:
                // Formální a věcná kontrola položky: Job.ExecutiveUser.
                // (Kdo bude na pohovoru za vedení).
                T25_Check_JobExecutiveUrl();

                // ----------------------------------------------------------------------

                // Infrastructure TestCase #26:
                // Formální a věcná kontrola vytvořené instance RestRequest pro
                // zjištění osoby Job.ExecutiveUser přítomné u pohovoru.
                //
                T26_CreateCheck_Request();

                // Infrastructure TestCase #27:
                // Věcná kontrola přidaných doplňkových parametrů RestRequest
                // pro Job.ExecutiveUser.
                T27_ModifyCheck_Request();

                // Infrastructure TestCase #28:
                // Odeslání requestu, příjem a kontrola odpovědi ze sub end-pointu
                // webové služby, který data o Job.ExecutiveUser.
                T28_SendReceiveCheck_ReqResp();

                // Infrastructure TestCase #29:
                // Formální kontrola přijatých a rozparsovaných dat
                // pro Job.ExecutiveUser.
                T29_Check_ResponseData();

                // ----------------------------------------------------------------------

                // TestCase #30:
                // Věcná kontrola obsahu hodnot položky Job.ExecutiveUser.Name.
                // (Kdo bude na pohovoru za vedení).
                //
                T30_Check_JobExecName();

                // TestCase #31:
                // Věcná kontrola obsahu hodnot položky Job.ExecutiveUser.Description.
                // (Kdo bude na pohovoru za vedení).
                //
                T31_Check_JobExecDesc();

                // TestCase #32:
                // Věcná kontrola obsahu hodnot položky Job.ExecutiveUser.Image.
                // (Kdo bude na pohovoru za vedení).
                //
                T32_Check_JobExecImg();

                // TestCase #33:
                // Formální a věcná kontrola položky: Job.GestorUser.
                // (Kdo bude na pohovoru za HR).
                //
                T33_Check_JobGestorUrl();

                // ----------------------------------------------------------------------

                // Infrastructure TestCase #34:
                // Formální a věcná kontrola vytvořené instance RestRequest pro
                // zjištění osoby Job.GestorUser přítomné u pohovoru.
                //
                T34_CreateCheck_Request();

                // Infrastructure TestCase #35:
                // Věcná kontrola přidaných doplňkových parametrů RestRequest
                // pro Job.GestorUser.
                T35_ModifyCheck_Request();

                // Infrastructure TestCase #36:
                // Odeslání requestu, příjem a kontrola odpovědi ze sub end-pointu
                // webové služby, který data o Job.GestorUser.
                T36_SendReceiveCheck_ReqResp();

                // Infrastructure TestCase #37:
                // Formální kontrola přijatých a rozparsovaných dat
                // pro Job.GestorUser.
                T37_Check_ResponseData();

                // ----------------------------------------------------------------------

                // TestCase #38:
                // Věcná kontrola obsahu hodnot položky Job.GestorUser.Name.
                // (Kdo bude na pohovoru za HR).
                //
                T38_Check_JobGestName();

                // TestCase #39:
                // Věcná kontrola obsahu hodnot položky Job.GestorUser.Description.
                // (Kdo bude na pohovoru za HR).
                //
                T39_Check_JobGestDesc();

                // TestCase #40:
                // Věcná kontrola obsahu hodnot položky Job.GestorUser.Image.
                // (Kdo bude na pohovoru za HR).
                //
                T40_Check_JobGestImg();

                #endregion
            }
            catch (Exception e)
            {
                // # Selhal infrastructure test, exekuce dalších testů se nebude již provádět.
                //

                Log.TraceMethodEnd();

                Log.InfoPlain();
                Log.InfoPlain($"WARNING: Exekuce testů je ukončena z důvodu chyby: {e.Message}.");
                return false;
            }

            Log.TraceMethodEnd();

            // # Neselhal žádný z infrastrukturních testů.
            // Zobrazí se pass/fail statistika + hodnoty pro každou testovanou položku.
            //

            Log.InfoPlain();
            Log.InfoPlain($"{string.Concat(Enumerable.Repeat("-", 96))}");
            Log.InfoPlain($">> REPORT DAT:{string.Concat(Enumerable.Repeat(" ", 35))}Test cases | >> PASS({_rs.PassedCnt}) / FAIL({_rs.FailedCnt}) <<");
            Log.InfoPlain($"{string.Concat(Enumerable.Repeat("-", 96))}");
            Log.InfoPlain();
            Log.InfoPlain($"Název pozice: {_job.Name}, SEO název pozice: {_job.SeoName}");
            Log.InfoPlain($"Job URL: {_job.HrefLangs.First().Url}");
            Log.InfoPlain($"Je to práce pro studenty: {(_job.ForStudents != null && (bool) _job.ForStudents ? "ANO" : "NE")}");
            Log.InfoPlain();
            var content = Regex.Match(_jobDescr.Items.Where(i => i.Type == 1).ToList()
                    .First().Content, @"\<span[^>]+\>(.+)\<\/span").Groups[1].Value
                .Replace("&nbsp;", " ");
            Log.InfoPlain($"Popis pozice: {content.Substring(0, 73)}...");
            Log.InfoPlain($"Kde se bude pracovat: {_job.Department.Name}, {_job.PlaceOfEmployment.Name}, {_job.PlaceOfEmployment.City}, {_job.PlaceOfEmployment.State}, { _job.Workplace}");
            Log.InfoPlain();
            Log.InfoPlain($"Na pohovoru za exekutivu bude: {_jobExecUser.Name}");
            Log.InfoPlain($"Kdo je to: {_jobExecUser.Description.Substring(0, 68)}...");
            Log.InfoPlain($"Fotografie: {_jobExecUser.Image.Substring(0, 59)}...");
            Log.InfoPlain();
            Log.InfoPlain($"Na pohovoru za HR bude: {_jobGestUser.Name}");
            Log.InfoPlain($"Kdo je to: {_jobGestUser.Description.Substring(0, 68)}...");
            Log.InfoPlain($"Fotografie: {_jobGestUser.Image.Substring(0, 59)}...");
            Log.InfoPlain();
            Log.InfoPlain($"{string.Concat(Enumerable.Repeat("-", 96))}");

            return _rs.FinalStatus;
        }

        // ----------------------------------------------------------------------
        //  Infrastrukturní testy prostředí, ověřují tech. stack, zda-li je OK.
        //  Jakýkoliv vygenerovaný FALSE znamená zde STOP pro další exekuci
        //  testů, protože pokud něco nefunguje, stejně žádné data z webové
        //  služby nedorazí a není v podstatě co testovat.
        // ----------------------------------------------------------------------

        /// <summary>
        /// Formální a věcná kontrola obsahu vstupního parametru ApiBaseUrl.<br/>
        /// Na vstupu je v rámci testApp očekáváná hodnota<br/>
        /// ve formátu "webapi.alza.cz".<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        /// <param name="apiBaseUrl">Testovaná hodnota ApiBaseUrl.
        /// </param>
        private static void T01_Check_ApiBaseUrl(string apiBaseUrl)
        {
            var testCase = new TestCase(ref _rs, "Formální a věcná kontrola obsahu ApiBaseUrl");
            apiBaseUrl = apiBaseUrl.ToLower();

            // Definice obsahu pro věcnou kontrolu.
            string[] mustBeValues = { "https", "webapi", "alza", "cz"};

            // Definice obsahu pro formální kontrolu.
            var pattern = @"^(\w+):\/\/(\w+)\.(\w+)\.(\w{2,})$";

            try
            {
                // Formální kontrola položky.
                if (!Regex.IsMatch(apiBaseUrl, pattern))
                {
                    throw new ArgumentException(
                        "ApiBaseUrl nesplňuje formát 'xxx.xxx.xx'");
                }
                
                string[] testContentArr = Regex.Split(apiBaseUrl, pattern);

                // Věcná kontrola položky.
                // Obsah uložen pod indexy [1, 2, 3, 4]. Split vrátí 6.
                if (testContentArr.Length != 6
                    || !testContentArr.Where(
                        val => val != string.Empty).ToArray().SequenceEqual(mustBeValues))
                {
                    throw new ArgumentException(
                        "ApiBaseUrl nemá očekávaný obsah");
                }
            }
            catch (Exception e)
            {
                testCase.LogFailResult(e.Message);
                throw new Exception(e.Message);
            }

            testCase.LogPassResult();

            _apiBaseUrl = apiBaseUrl;
        }

        /// <summary>
        /// Formální a věcná kontrola obsahu vstupního parametru ApiUri.<br/>
        /// Na vstupu je v rámci testApp očekáváná hodnota<br/>
        /// ve formátu "/api/career/v2/positions/".<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        /// <param name="apiUri">Testovaná hodnota ApiUri.
        /// </param>

        private static void T02_Check_ApiUri(string apiUri)
        {
            var testCase = new TestCase(ref _rs, "Formální a věcná kontrola obsahu ApiUri");
            apiUri = apiUri.ToLower();

            // Definice obsahu pro věcnou kontrolu.
            string[] mustBeValues = { "api", "career", "v2", "positions" };

            // Definice obsahu pro formální kontrolu.
            var pattern = @"^\/(\w+)\/(\w+)\/(\w{2})\/(\w+)\/$";

            try
            {
                // Formální kontrola položky.
                if (!Regex.IsMatch(apiUri, pattern))
                {
                    throw new ArgumentException(
                        "ApiUri nesplňuje formát '/xxx/xxx/xx/xxx/'");
                }

                string[] testContentArr = Regex.Split(apiUri, pattern);

                // Věcná kontrola položky.
                // Obsah uložen pod indexy [1, 2, 3, 4]. Split vrátí 6.
                if (testContentArr.Length != 6
                    || !testContentArr.Where(
                        val => val != string.Empty).ToArray().SequenceEqual(mustBeValues))
                {
                    throw new ArgumentException(
                        "ApiUri nemá očekávaný obsah");
                }
            }
            catch (Exception e)
            {
                testCase.LogFailResult(e.Message);
                throw new Exception(e.Message);
            }

            testCase.LogPassResult();

            _apiUri = apiUri;
        }

        /// <summary>
        /// Formální & věcná kontrola obsahu vstupního parametru ApiParJobName.<br/>
        /// Na vstupu je očekáváný název pracovní pozice ve formátu<br/>
        /// "softwarovy-tester".<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        /// <param name="apiParJobName">Testovaná hodnota JobName.
        /// </param>
        private static void T03_Check_ApiParJobName(string apiParJobName)
        {
            var testCase = new TestCase(ref _rs, "Formální a věcná kontrola obsahu ApiParJobName");
            apiParJobName = apiParJobName.ToLower();

            // Definice obsahu pro věcnou kontrolu.
            string[] mustBeValue = { "softwarovy-tester" };

            // Definice obsahu pro formální kontrolu.
            var pattern = @"^(\w+(?:-\w+)*)$";

            try
            {
                // Formální kontrola položky.
                if (!Regex.IsMatch(apiParJobName, pattern))
                {
                    throw new ArgumentException(
                        "ApiParJobName nesplňuje formát zápisu 'xxx-xxx'");
                }

                string[] testContentArr = Regex.Split(apiParJobName, pattern);

                // Věcná kontrola položky.
                // Obsah uložen pod indexem [1]. Split vrátí 3.
                if (testContentArr.Length != 3
                    || !testContentArr.Where(
                        val => val != string.Empty).ToArray().SequenceEqual(mustBeValue))
                {
                    throw new ArgumentException(
                        "ApiParJobName nemá očekávaný obsah");
                }
            }
            catch (Exception e)
            {
                testCase.LogFailResult(e.Message);
                throw new Exception(e.Message);
            }

            testCase.LogPassResult();

            _apiParJobName = apiParJobName;
        }

        /// <summary>
        /// Formální & věcná kontrola obsahu vstupního parametru ApiParJobCountry.<br/>
        /// Na vstupu je očekáván target country pro danou prac. prozici<br/>
        /// ve formátu "country=cz".<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        /// <param name="apiParJobCountry">Testovaná hodnota JobCountry.
        /// </param>
        private static void T04_Check_ApiParJobCountry(string apiParJobCountry)
        {
            var testCase = new TestCase(ref _rs, "Formální a věcná kontrola obsahu ApiParJobCountry");
            apiParJobCountry = apiParJobCountry.ToLower();

            // Definice obsahu pro věcnou kontrolu.
            string[] mustBeValues = { "country", "cz" };

            // Definice obsahu pro formální kontrolu.
            var pattern = @"^([a-z]+)\=([a-z]{2,})$";

            try
            {
                // Formální kontrola položky.
                if (!Regex.IsMatch(apiParJobCountry, pattern))
                {
                    throw new ArgumentException(
                        "ApiParJobCountry nesplňuje formát zápisu 'xxxxxxx=xx'");
                }

                string[] testContentArr = Regex.Split(apiParJobCountry, pattern);

                // Věcná kontrola položky.
                // Obsah uložen pod indexy [1, 2]. Split vrátí 4.
                if (testContentArr.Length != 4
                    || !testContentArr.Where(
                        val => val != string.Empty).ToArray().SequenceEqual(mustBeValues))
                {
                    throw new ArgumentException(
                        "ApiParJobCountry nemá očekávaný obsah");
                }
            }
            catch (Exception e)
            {
                testCase.LogFailResult(e.Message);
                throw new Exception(e.Message);
            }

            testCase.LogPassResult();

            _apiParJobCountry = apiParJobCountry;
        }

        /// <summary>
        /// Kontrola formální správnosti obsahu vstupního ApiUri včetně jeho parametrů.<br/>
        /// Na vstupu je vyskládaná kompletní hodnota ApiUri ve formátu<br/>
        /// "https://webapi.alza.cz/api/career/v2/positions/softwarovy-tester?country=cz".<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static void T05_Check_ApiUriWithPars()
        {
            var testCase = new TestCase(ref _rs, "Formální kontrola správnosti cílové end-point adresy");

            // Definice obsahu pro formální kontrolu.
            var apiUriWithPars = 
                $"{_apiBaseUrl}{_apiUri}{_apiParJobName}?{_apiParJobCountry}".ToLower();

            var pattern =
                @"^https\:\/\/\w+\.\w+\.[a-z]{2,}\/\w+\/\w+\/\w\d{1,}\/\w+\/[^\?]+\?\w+\=\w{2,}$";

            try
            {
                // Formální kontrola zápisu adresy webové služby včetně parametrů.
                // Věcná kontrola byla již provedena v předchozích testCases.
                //

                if (!Regex.IsMatch(apiUriWithPars, pattern))
                {
                    throw new ArgumentException(
                        "ApiUriWithPars nemá požadovaný formát adresy end-pointu");
                }
            }
            catch (Exception e)
            {
                testCase.LogFailResult(e.Message);
                throw new Exception(e.Message);
            }

            testCase.LogPassResult();

            _apiUriWithParams = apiUriWithPars;
        }

        /// <summary>
        /// Formální kontrola vytvoření instance Rest klienta včetně kontroly<br/>
        /// jeho BaseUrl hodnoty.<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static void T06_CreateCheck_Client()
        {
            var testCase = new TestCase(ref _rs, "Formální a věcná kontrola instance Rest klienta");
            _client = new RestClient(_apiBaseUrl);
            _client.UseNewtonsoftJson();

            try
            {
                // Formální kontrola položky.
                if (_client is null)
                {
                    throw new Exception(
                        "Nepovedlo se vytvořit novou instanci Rest klienta");
                }

                // Formální kontrola položky.
                if (_client.BaseUrl is null)
                {
                    throw new Exception(
                        "Instance Rest klienta je bez mandatory parametrů");
                }
            }
            catch (Exception e)
            {
                testCase.LogFailResult(e.Message);
                throw new Exception(e.Message);
            }

            testCase.LogPassResult();
        }

        /// <summary>
        /// Formální a věcná kontrola vytvoření instance RestRequest včetně kontroly<br/>
        /// některých její hodnot.<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static void T07_CreateCheck_Request()
        {
            var testCase = new TestCase(ref _rs, "Formální a věcná kontrola instance RestRequest");
            _request = new RestRequest(_apiUriWithParams, Method.GET);

            // Definice obsahu pro věcnou kontrolu RestRequest.Parameters.
            string[] mustBeValues = _apiParJobCountry.Split('=');

            try
            {
                // Kontrola na formální existenci Request instance.
                if (_request is null)
                {
                    throw new Exception(
                        "Nepovedlo se vytvořit novou instanci RestRequest");
                }
                
                // Věcná kontrola hodnoty parametru Method.
                if (_request.Method != Method.GET)
                {
                    throw new Exception(
                        "Hodnota RestRequest.GET nemá očekávanou hodnotu");
                }

                // Věcná kontrola hodnot RestRequest.Resource parametru.
                if (!_request.Resource.Contains(_apiUri)
                    || !_request.Resource.Contains(_apiParJobName))
                {
                    throw new Exception(
                        "Hodnota RestRequest.Resource nemá definovaný obsah");
                }

                // Věcná kontrola položky.
                if (mustBeValues.Length != 2
                    || !_request.Parameters.Any()
                    || _request.Parameters[0].Name != null
                        && !_request.Parameters[0].Name.Equals(mustBeValues[0])
                    || _request.Parameters[0].Value != null
                        && !_request.Parameters[0].Value.Equals(mustBeValues[1]))
                {
                    throw new Exception(
                        "Hodnota RestRequest.Parameters nemá očekávaný obsah");
                }
            }
            catch (Exception e)
            {
                testCase.LogFailResult(e.Message);
                throw new Exception(e.Message);
            }

            testCase.LogPassResult();
        }

        /// <summary>
        /// Věcná kontrola přidaných doplňkových parametrů pro RestRequest.<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static void T08_ModifyCheck_Request()
        {
            var testCase = new TestCase(ref _rs, "Věcná kontrola doplňkových parametrů pro RestRequest");
            _request.AddHeader("Accept", "application/json");
            _request.RequestFormat = DataFormat.Json;

            // Definice obsahu pro věcnou kontrolu RestRequest.Parameters.
            string[] mustBeValues = { "Accept", "application/json" };

            try
            {
                // Věcná kontrola položky.
                if (_request.RequestFormat != DataFormat.Json)
                {
                    throw new Exception(
                        "Hodnota RestRequest.RequestFormat nemá očekávaný obsah");
                }

                // Věcná kontrola položky.
                if (mustBeValues.Length != 2
                    || _request.Parameters.Count < 2
                    || _request.Parameters[1].Name is null
                    || !_request.Parameters[1].Name.Equals(mustBeValues[0])
                    || _request.Parameters[1].Value is null
                    || !_request.Parameters[1].Value.Equals(mustBeValues[1]))
                {
                    throw new Exception(
                        "Hodnota RestRequest.Parameters nemá očekávaný obsah");
                }
            }
            catch (Exception e)
            {
                testCase.LogFailResult(e.Message);
                throw new Exception(e.Message);
            }

            testCase.LogPassResult();
        }

        /// <summary>
        /// Odeslání requestu, příjem a kontrola odpovědi z cílového end-pointu webové služby.<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static void T09_SendReceiveCheck_ReqResp()
        {
            var testCase = new TestCase(ref _rs, "Send/Receive/Kontrola dat na/z target end-pointu");
            _respJob = _client.Execute<Job>(_request);

            try
            {
                // Formální kontrola položky.
                if (_respJob is null)
                {
                    throw new Exception(
                        "Response je ve stavu NULL, tedy bez vrácených hodnot");
                }

                // Zpracování typu vrácené odpovědi z Rest.WebService.
                _statusCode = _respJob.StatusCode;

                // Věcná kontrola položky.
                if (_statusCode != HttpStatusCode.OK)
                {
                    throw new Exception(
                        "Http stavový kód odpovědí nemá očekávaný obsah");
                }

                // Věcná kontrola položky.
                if (_respJob.Content.Length <= 0)
                {
                    throw new Exception(
                        "Položka Content.Length má neočekávanou hodnotu");
                }
            }
            catch (Exception e)
            {
                testCase.LogFailResult(e.Message);
                throw new Exception(e.Message);
            }

            testCase.LogPassResult();
        }

        /// <summary>
        /// Formální kontrola přijatých a rozparsovaných Job dat.<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static void T10_Check_ResponseData()
        {
            var testCase = new TestCase(ref _rs, "Formální kontrola rozparsovaných Job dat");
            _job = _respJob.Data;               // Data memberField is deserialized.

            try
            {
                // Formální kontrola obsahu přijaté odpovědi.
                if (_job is null)
                {
                    throw new Exception(
                        "Položka Job.Content.Data je ve stavu NULL");
                }

                // Formální kontrola typu přijaté odpovědi.
                if (_job.GetType() != typeof(Job))
                {
                    throw new Exception(
                        "Položka Job.Content.Data má nesprávný typ dat");
                }

                // Formální kontrola na výskyt rozparsovaných mandatory položek.
                if (string.IsNullOrEmpty(_job.Workplace)
                    || string.IsNullOrEmpty(_job.SeoName)
                    || string.IsNullOrEmpty(_job.Name)
                    || _job.PlaceOfEmployment is null
                    || _job.PositionItems is null
                    || _job.ExecutiveUser is null
                    || _job.Department is null
                    || _job.GestorUser is null
                    || _job.HrefLangs is null)
                {
                    throw new Exception(
                        "Job.Content.Data nemá platnou obsahovou strukturu");
                }
            }
            catch (Exception e)
            {
                testCase.LogFailResult(e.Message);
                throw new Exception(e.Message);
            }

            testCase.LogPassResult();
        }

        // ----------------------------------------------------------------------
        //  TestCases pro data, které vrátila již samotná webová služba.
        // ----------------------------------------------------------------------

        /// <summary>
        /// Věcná kontrola obsahu položky přijatých dat: Job.SeoName.<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static bool T11_Check_JobSeoName()
        {
            var testCase = new TestCase(ref _rs, "Věcná kontrola položky Job.SeoName");

            // Definice obsahu pro věcnou kontrolu.
            string mustBeValue = "softwarovy-tester";

            try
            {
                // Formální kontrola na NOT NULL or empty value
                // proběhla v testCase T10_Check_ResponseData().

                // Věcná kontrola položky.
                if (!_job.SeoName.Equals(mustBeValue))
                {
                    throw new Exception(
                        "Položka Job.SeoName nemá očekávanou hodnotu");
                }
            }
            catch
            {
                // U chybového statusu se již error text neuvádí,
                // pro zalogování postačuje plně zápis FAIL.

                testCase.LogFailResult();
                return false;
            }

            testCase.LogPassResult();
            return true;
        }

        /// <summary>
        /// Věcná kontrola položky přijatých dat: Job.Name.<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static bool T12_Check_JobName()
        {
            var testCase = new TestCase(ref _rs, "Věcná kontrola položky Job.Name");

            // Definice obsahu pro věcnou kontrolu.
            string mustBeValue = "Softwarový Tester";

            try
            {
                // Formální kontrola na NOT NULL or empty value
                // proběhla v testCase T10_Check_ResponseData().

                // Věcná kontrola položky.
                if (!_job.Name.Equals(mustBeValue))
                {
                    throw new Exception(
                        "Položka Job.Name nemá očekávanou hodnotu");
                }
            }
            catch
            {
                // U chybového statusu se již error text neuvádí,
                // pro zalogování postačuje plně zápis FAIL.

                testCase.LogFailResult();
                return false;
            }

            testCase.LogPassResult();
            return true;
        }

        /// <summary>
        /// Formální a věcná kontrola položky přijatých dat: Job.LanguageCode.<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static bool T13_Check_JobLangCode()
        {
            var testCase = new TestCase(ref _rs, "Formální a věcná kontrola položky Job.LanguageCode");

            // Definice obsahu pro věcnou kontrolu.
            string mustBeValue = "cs-CZ";

            try
            {
                // Formální kontrola položky.
                if (_job.HrefLangs is null
                    || !_job.HrefLangs.Any())
                {
                    throw new Exception(
                        "Json uzel Job.HrefLangs is undefined");
                }

                // Věcná kontrola položky.
                if (!_job.HrefLangs.First().LanguageCode.Equals(mustBeValue))
                {
                    throw new Exception(
                        "Položka Job.LanguageCode má neočekávanou hodnotu");
                }
            }
            catch
            {
                // U chybového statusu se již error text neuvádí,
                // pro zalogování postačuje plně zápis FAIL.

                testCase.LogFailResult();
                return false;
            }

            testCase.LogPassResult();
            return true;
        }

        /// <summary>
        /// Formální a věcná kontrola položky přijatých dat: Job.Url.<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static bool T14_Check_JobUrl()
        {
            var testCase = new TestCase(ref _rs, "Formální a věcná kontrola položky Job.Url");

            // Definice obsahu pro věcnou kontrolu.
            string mustBeValue = @"https://www.alza.cz/kariera/pozice/softwarovy-tester";

            try
            {
                // Formální kontrola položky.
                if (_job.HrefLangs is null || !_job.HrefLangs.Any())
                {
                    throw new Exception(
                        "JSON uzel položky Job.HrefLangs není definován");
                }

                // Věcná kontrola položky.
                if (!_job.HrefLangs.First().Url.Equals(mustBeValue))
                {
                    throw new Exception(
                        "Položka Job.Url má neočekávanou hodnotu");
                }
            }
            catch
            {
                // U chybového statusu se již error text neuvádí,
                // pro zalogování postačuje plně zápis FAIL.

                testCase.LogFailResult();
                return false;
            }

            testCase.LogPassResult();
            return true;
        }

        /// <summary>
        /// Formální a věcná kontrola položky pro popis pozice: Job.PositionItems.<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static bool T15_Check_JobDescrUrl()
        {
            var testCase = new TestCase(ref _rs, "Formální a věcná kontrola adresy pro Job.Description");

            // Definice obsahu pro věcnou kontrolu.
            string mustBeValue = 
                @"https://webapi.alza.cz/api/career/v1/positions/softwarovy-tester/items";

            try
            {
                // Formální kontrola položky.
                if (_job.PositionItems is null
                    || _job.PositionItems.Meta is null
                    || _job.PositionItems.Meta.Href is null)
                {
                    throw new Exception(
                        "JSON uzel položky Job.PositionItems není definován");
                }

                // Věcná kontrola hodnoty položky.
                // Testování konkrétního obsahu zde může být na zvážení,
                // podle toho, zda-li je hodnota vyprojektovaná jako
                // fixní, či nikoliv, tedy povolen variabilní obsah.
                //

                if (!_job.PositionItems.Meta.Href.Equals(mustBeValue))
                {
                    throw new Exception(
                        "Položka Job.PositionItems.Meta.Href nemá očekávanou hodnotu");
                }

                // Adresa end-pointu pro zjištění popisu prac. pozice.
                _apiUriForJobDesc = _job.PositionItems.Meta.Href;
            }
            catch
            {
                // U chybového statusu se již error text neuvádí,
                // pro zalogování postačuje plně zápis FAIL.

                testCase.LogFailResult();
                return false;
            }

            testCase.LogPassResult();
            return true;
        }

        // ----------------------------------------------------------------------
        //  Infrastrukturní testy prostředí
        // ----------------------------------------------------------------------

        /// <summary>
        /// Formální a věcná kontrola vytvoření instance RestRequest pro zjištění popisu<br/>
        /// job pozice včetně kontroly jeho některých hodnot.<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static void T16_CreateCheck_Request()
        {
            var testCase = new TestCase(ref _rs, "Formální a věcná kontrola instance RestRequest");
            _request = new RestRequest(_apiUriForJobDesc, Method.GET);

            try
            {
                // Kontrola na formální existenci Request instance.
                if (_request is null)
                {
                    throw new Exception(
                        "Nepovedlo se vytvořit novou instanci RestRequest");
                }

                // Věcná kontrola hodnoty parametru Method.
                if (_request.Method != Method.GET)
                {
                    throw new Exception(
                        "Hodnota RestRequest.GET nemá očekávanou hodnotu");
                }

                // Věcná kontrola hodnot RestRequest.Resource parametru.
                if (!_request.Resource.Contains(_apiUriForJobDesc))
                {
                    throw new Exception(
                        "Hodnota RestRequest.Resource nemá definovaný obsah");
                }
            }
            catch (Exception e)
            {
                testCase.LogFailResult(e.Message);
                throw new Exception(e.Message);
            }

            testCase.LogPassResult();
        }

        /// <summary>
        /// Věcná kontrola přidaných doplňkových parametrů pro RestRequest.<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static void T17_ModifyCheck_Request()
        {
            var testCase = new TestCase(ref _rs, "Věcná kontrola doplňkových parametrů pro RestRequest");
            _request.AddHeader("Accept", "application/json");
            _request.RequestFormat = DataFormat.Json;

            // Definice obsahu pro věcnou kontrolu RestRequest.Parameters.
            string[] mustBeValues = { "Accept", "application/json" };

            try
            {
                // Věcná kontrola položky.
                if (_request.RequestFormat != DataFormat.Json)
                {
                    throw new Exception(
                        "Hodnota RestRequest.RequestFormat nemá očekávaný obsah");
                }

                // Věcná kontrola položky.
                if (mustBeValues.Length != 2
                    || _request.Parameters.Count < 1
                    || _request.Parameters[0].Name is null
                    || !_request.Parameters[0].Name.Equals(mustBeValues[0])
                    || _request.Parameters[0].Value is null
                    || !_request.Parameters[0].Value.Equals(mustBeValues[1]))
                {
                    // TODO: Dořešit Exceptions a E.Messages, které vzniknou
                    // v rámci vyhodnocování podmínky tak, aby se nelogovala
                    // konkrétní generická chyba, ale níže uvedený obsah.

                    throw new Exception(
                        "Hodnota RestRequest.Parameters nemá očekávaný obsah");
                }
            }
            catch (Exception e)
            {
                testCase.LogFailResult(e.Message);
                throw new Exception(e.Message);
            }

            testCase.LogPassResult();
        }



        /// <summary>
        /// Odeslání requestu, příjem a kontrola odpovědi ze sub end-pointu webové služby,<br/>
        /// defined by Job.PositionItems.Meta.Href, která vrací popis job pozice.<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static void T18_SendReceiveCheck_ReqResp()
        {
            var testCase = new TestCase(ref _rs, "Send/Receive/Kontrola dat na/z target end-pointu");
            _respJobDescr = _client.Execute<JobDescription>(_request);

            try
            {
                // Formální kontrola položky.
                if (_respJobDescr is null)
                {
                    throw new Exception(
                        "Response je ve stavu NULL, tedy bez vrácených hodnot");
                }

                // Zpracování typu vrácené odpovědi z Rest.WebService.
                _statusCode = _respJobDescr.StatusCode;

                // Věcná kontrola položky.
                if (_statusCode != HttpStatusCode.OK)
                {
                    throw new Exception(
                        "Http stavový kód odpovědí nemá očekávaný obsah");
                }

                // Věcná kontrola položky.
                if (_respJobDescr.Content.Length <= 0)
                {
                    throw new Exception(
                        "Položka Content.Length má neočekávanou hodnotu");
                }
            }
            catch (Exception e)
            {
                testCase.LogFailResult(e.Message);
                throw new Exception(e.Message);
            }

            testCase.LogPassResult();
        }

        /// <summary>
        /// Formální kontrola přijatých a rozparsovaných dat pro popis pozice.<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static void T19_Check_ResponseData()
        {
            var testCase = new TestCase(ref _rs, "Formální kontrola rozparsovaných JobDescription dat");
            _jobDescr = _respJobDescr.Data;                // Data memberField is deserialized.

            try
            {
                // Formální kontrola obsahu přijaté odpovědi.
                if (_jobDescr is null)
                {
                    throw new Exception(
                        "Položka Job.Descr.Content.Data je ve stavu NULL");
                }

                // Formální kontrola typu přijaté odpovědi.
                if (_jobDescr.GetType() != typeof(JobDescription))
                {
                    throw new Exception(
                        "Položka Job.Descr.Content.Data má nesprávný typ dat");
                }

                // Formální kontrola na výskyt rozparsovaných mandatory položek.
                if (_jobDescr.Items is null)
                {
                    throw new Exception(
                        "Job.Descr.Content.Data nemá platnou obsahovou strukturu");
                }
            }
            catch (Exception e)
            {
                testCase.LogFailResult(e.Message);
                throw new Exception(e.Message);
            }

            testCase.LogPassResult();
        }

        // ----------------------------------------------------------------------
        //  TestCases pro data, které vrátila již samotná webová služba.
        // ----------------------------------------------------------------------

        /// <summary>
        /// Formální a věcná kontrola obsahu položky Job.Description.<br/>
        /// (Vyplněný popis pozice).<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static bool T20_Check_JobDescription()
        {
            var testCase = new TestCase(ref _rs, "Formální a věcná kontrola položky Job.Description");

            try
            {
                // Definice obsahu pro formální a věcnou kontrolu.
                // Popis pozice se nachází v prvku, u kterého platí: Type = 1.

                var jobDescrItems = _jobDescr.Items.Where(i => i.Type == 1)
                    .ToList();

                // Formální kontrola položky.
                if (!jobDescrItems.Any())
                {
                    throw new Exception(
                        "Položka Job.Description neexistuje");
                }

                // Věcná kontrola položky.
                if (string.IsNullOrEmpty(jobDescrItems.First().Content))
                {
                    // Otázkou je, co je považováno za vyplněný obsah.
                    // Předpokládám, že se jedná o cokoliv, validace
                    // není blíže specifikována, resp. požadována.
                    //

                    throw new Exception(
                        "Položka Job.Description nemá žádný obsah");
                }
            }
            catch
            {
                // U chybového statusu se již error text neuvádí,
                // pro zalogování postačuje plně zápis FAIL.

                testCase.LogFailResult();
                return false;
            }

            testCase.LogPassResult();
            return true;
        }

        /// <summary>
        /// Věcná kontrola obsahu položky přijatých dat: Job.WorkPlace.<br/>
        /// (Kde se bude pracovat).<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static bool T21_Check_JobWorkPlace()
        {
            var testCase = new TestCase(ref _rs, "Věcná kontrola položky Job.WorkPlace");

            try
            {
                // Formální kontrola na NOT NULL or empty value
                // proběhla v testCase T10_Check_ResponseData().

                // Věcná kontrola hodnoty položky.
                if (_job.Workplace.Length < 2)
                {
                    // Otázkou je, co je považováno za vyplněný obsah.
                    // Předpokládám, že se jedná o cokoliv typu "ČR",
                    // bližší specifikace není uvedena.
                    //

                    throw new Exception(
                        "Položka Job.WorkPlace nemá očekávaný obsah");
                }
            }
            catch
            {
                // U chybového statusu se již error text neuvádí,
                // pro zalogování postačuje plně zápis FAIL.

                testCase.LogFailResult();
                return false;
            }

            testCase.LogPassResult();
            return true;
        }

        /// <summary>
        /// Věcná kontrola položky přijatých dat: Job.PlaceOfEmployment.<br/>
        /// (Kde se bude pracovat).<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static bool T22_Check_JobPlaceEmploy()
        {
            var testCase = new TestCase(ref _rs, "Věcná kontrola položky Job.PlaceOfEmloyment");

            try
            {
                // Formální kontrola na NOT NULL
                // proběhla v testCase T10_Check_ResponseData().

                // Věcná kontrola položky.
                if (string.IsNullOrEmpty(_job.PlaceOfEmployment.Name)
                    || string.IsNullOrEmpty(_job.PlaceOfEmployment.State)
                    || string.IsNullOrEmpty(_job.PlaceOfEmployment.City))
                {
                    // Otázkou je, co je považováno za vyplněný obsah.
                    // Předpokládám, že se jedná o cokoliv nenulového,
                    // blížší specifikace není definována.
                    //

                    throw new Exception(
                        "Uzel položky Job.PlaceOfEmloyment není vyplněn");
                }
            }
            catch
            {
                // U chybového statusu se již error text neuvádí,
                // pro zalogování postačuje plně zápis FAIL.

                testCase.LogFailResult();
                return false;
            }

            testCase.LogPassResult();
            return true;
        }

        /// <summary>
        /// Věcná kontrola položky přijatých dat: Job.Department.<br/>
        /// (Kde se bude pracovat).<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static bool T23_Check_JobDepartment()
        {
            var testCase = new TestCase(ref _rs, "Věcná kontrola položky Job.Department");

            try
            {
                // Formální kontrola na NOT NULL
                // proběhla v testCase T10_Check_ResponseData().

                // Věcná kontrola položky.
                if (string.IsNullOrEmpty(_job.Department.SeoName)
                    || string.IsNullOrEmpty(_job.Department.Name)
                    || string.IsNullOrEmpty(_job.Department.Url)
                    || string.IsNullOrEmpty(_job.Department.Meta.Href))
                {
                    // Otázkou je, co je považováno za vyplněný obsah.
                    // Předpokládám, že se jedná o cokoliv nenulového,
                    // blížší specifikace není definována.
                    //

                    throw new Exception(
                        "Uzel položky Job.Department není vyplněn");
                }

                // Věcná kontrola obsahu konkrétního uzlu.
                if (!_job.Department.Meta.Href.StartsWith("https://"))
                {
                    throw new Exception(
                        "Uzel Job.Department.Meta.Href nemá očekávaný obsah");
                }
            }
            catch
            {
                // U chybového statusu se již error text neuvádí,
                // pro zalogování postačuje plně zápis FAIL.

                testCase.LogFailResult();
                return false;
            }

            testCase.LogPassResult();
            return true;
        }

        /// <summary>
        /// Formální kontrola položky přijatých dat: Job.ForStudents.<br/>
        /// (Info, zda-li je to práce pro studenty).<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static bool T24_Check_JobForStudents()
        {
            var testCase = new TestCase(ref _rs, "Formální kontrola položky Job.ForStudents");

            try
            {
                // Formální kontrola položky.
                if (_job.ForStudents is null)
                {
                    throw new Exception(
                        "Položka Job.ForStudents není vyplněna");
                }
            }
            catch
            {
                // U chybového statusu se již error text neuvádí,
                // pro zalogování postačuje plně zápis FAIL.

                testCase.LogFailResult();
                return false;
            }

            testCase.LogPassResult();
            return true;
        }

        /// <summary>
        /// Formální a věcná kontrola položky přijatých dat: Job.ExecutiveUser.<br/>
        /// (Kdo bude na pohovoru za vedení).<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static bool T25_Check_JobExecutiveUrl()
        {
            var testCase = new TestCase(ref _rs, "Formální a věcná kontrola adresy Job.Executive");

            // Definice obsahu pro věcnou kontrolu.
            string mustBeValue =
                @"https://webapi.alza.cz/api/career/v1/employees/";

            try
            {
                // Formální kontrola položky.
                if (_job.ExecutiveUser is null
                    || _job.ExecutiveUser.Meta is null
                    || _job.ExecutiveUser.Meta.Href is null)
                {
                    throw new Exception(
                        "JSON uzly položky Job.ExecutiveUser jsou ve stavu NULL");
                }

                // Věcná kontrola položky.
                if (!_job.ExecutiveUser.Meta.Href.StartsWith(mustBeValue))
                {
                    throw new Exception(
                        "Položka Job.ExecutiveUser.Meta.Href nemá očekávanou hodnotu");
                }

                // Adresa end-pointu pro zjištění osoby přítomné u pohovoru.
                _apiUriForExecUsr = _job.ExecutiveUser.Meta.Href;
            }
            catch
            {
                // U chybového statusu se již error text neuvádí,
                // pro zalogování postačuje plně zápis FAIL.

                testCase.LogFailResult();
                return false;
            }

            testCase.LogPassResult();
            return true;
        }

        // ----------------------------------------------------------------------
        //  Infrastrukturní testy prostředí
        // ----------------------------------------------------------------------

        /// <summary>
        /// Formální a věcná kontrola vytvoření instance RestRequest pro zjištění osoby
        /// Job.ExecutiveUser přítomné u pohovoru, včetně kontroly některých<br/>
        /// její hodnot.<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static void T26_CreateCheck_Request()
        {
            var testCase = new TestCase(ref _rs, "Formální a věcná kontrola instance RestRequest");
            _request = new RestRequest(_apiUriForExecUsr, Method.GET);

            try
            {
                // Kontrola na formální existenci Request instance.
                if (_request is null)
                {
                    throw new Exception(
                        "Nepovedlo se vytvořit novou instanci RestRequest");
                }

                // Věcná kontrola hodnoty parametru Method.
                if (_request.Method != Method.GET)
                {
                    throw new Exception(
                        "Hodnota RestRequest.GET nemá očekávanou hodnotu");
                }

                // Věcná kontrola hodnot RestRequest.Resource parametru.
                if (!_request.Resource.Contains(_apiUriForExecUsr))
                {
                    throw new Exception(
                        "Hodnota RestRequest.Resource nemá definovaný obsah");
                }
            }
            catch (Exception e)
            {
                testCase.LogFailResult(e.Message);
                throw new Exception(e.Message);
            }

            testCase.LogPassResult();
        }

        /// <summary>
        /// Věcná kontrola přidaných doplňkových parametrů RestRequest<br/>
        /// pro Job.ExecutiveUser.
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static void T27_ModifyCheck_Request()
        {
            var testCase = new TestCase(ref _rs, "Věcná kontrola doplňkových parametrů pro RestRequest");
            _request.AddHeader("Accept", "application/json");
            _request.RequestFormat = DataFormat.Json;

            // Definice obsahu pro věcnou kontrolu RestRequest.Parameters.
            string[] mustBeValues = { "Accept", "application/json" };

            try
            {
                // Věcná kontrola položky.
                if (_request.RequestFormat != DataFormat.Json)
                {
                    throw new Exception(
                        "Hodnota RestRequest.RequestFormat nemá očekávaný obsah");
                }

                // Věcná kontrola položky.
                if (mustBeValues.Length != 2
                    || _request.Parameters.Count < 1
                    || _request.Parameters[0].Name is null
                    || !_request.Parameters[0].Name.Equals(mustBeValues[0])
                    || _request.Parameters[0].Value is null
                    || !_request.Parameters[0].Value.Equals(mustBeValues[1]))
                {
                    // TODO: Dořešit Exceptions a E.Messages, které vzniknou
                    // v rámci vyhodnocování podmínky tak, aby se nelogovala
                    // konkrétní generická chyba, ale níže uvedený obsah.

                    throw new Exception(
                        "Hodnota RestRequest.Parameters nemá očekávaný obsah");
                }
            }
            catch (Exception e)
            {
                testCase.LogFailResult(e.Message);
                throw new Exception(e.Message);
            }

            testCase.LogPassResult();
        }



        /// <summary>
        /// Odeslání requestu, příjem a kontrola odpovědi ze sub end-pointu webové služby,<br/>
        /// defined by Job.ExecutiveUser.Meta.Href, pro Job.ExecutiveUser.<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static void T28_SendReceiveCheck_ReqResp()
        {
            var testCase = new TestCase(ref _rs, "Send/Receive/Kontrola dat na/z target end-pointu");
            _respJobExecUser = _client.Execute<User>(_request);

            try
            {
                // Formální kontrola položky.
                if (_respJobExecUser is null)
                {
                    throw new Exception(
                        "Response je ve stavu NULL, tedy bez vrácených hodnot");
                }

                // Zpracování typu vrácené odpovědi z Rest.WebService.
                _statusCode = _respJobExecUser.StatusCode;

                // Věcná kontrola položky.
                if (_statusCode != HttpStatusCode.OK)
                {
                    throw new Exception(
                        "Http stavový kód odpovědí nemá očekávaný obsah");
                }

                // Věcná kontrola položky.
                if (_respJobExecUser.Content.Length <= 0)
                {
                    throw new Exception(
                        "Položka Content.Length má neočekávanou hodnotu");
                }
            }
            catch (Exception e)
            {
                testCase.LogFailResult(e.Message);
                throw new Exception(e.Message);
            }

            testCase.LogPassResult();
        }

        /// <summary>
        /// Formální kontrola přijatých a rozparsovaných dat pro Job.ExecutiveUser.<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static void T29_Check_ResponseData()
        {
            var testCase = new TestCase(ref _rs, "Formální kontrola rozparsovaných Job.ExecUser dat");
            _jobExecUser = _respJobExecUser.Data;        // Data memberField is deserialized.

            try
            {
                // Formální kontrola obsahu přijaté odpovědi.
                if (_jobExecUser is null)
                {
                    throw new Exception(
                        "Položka Job.ExecUser.Content.Data je ve stavu NULL");
                }

                // Formální kontrola typu přijaté odpovědi.
                if (_jobExecUser.GetType() != typeof(User))
                {
                    throw new Exception(
                        "Položka Job.ExecUser.Content.Data má nesprávný typ dat");
                }

                // Formální kontrola na výskyt rozparsovaných mandatory položek.
                if (string.IsNullOrEmpty(_jobExecUser.Name)
                    || string.IsNullOrEmpty(_jobExecUser.Description)
                    || string.IsNullOrEmpty(_jobExecUser.Image))
                {
                    throw new Exception(
                        "Job.ExecUser.Content.Data nemá platnou obsahovou strukturu");
                }
            }
            catch (Exception e)
            {
                testCase.LogFailResult(e.Message);
                throw new Exception(e.Message);
            }

            testCase.LogPassResult();
        }

        // ----------------------------------------------------------------------
        //  TestCases pro data, které vrátila již samotná webová služba.
        // ----------------------------------------------------------------------

        /// <summary>
        /// Věcná kontrola obsahu hodnot položky Job.ExecutiveUser.Name<br/>
        /// (Kdo bude na pohovoru za vedení).<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static bool T30_Check_JobExecName()
        {
            var testCase = new TestCase(ref _rs, "Věcná kontrola položky Job.ExecUser.Name");

            try
            {
                // Formální kontrola na NOT NULL or empty value
                // proběhla v testCase T29_Check_ResponseData().

                // Věcná kontrola položky.
                if (string.IsNullOrEmpty(_jobExecUser.Name)
                )
                {
                    // Otázkou je, co je považováno za vyplněný obsah.
                    // Předpokládám, že se jedná o cokoliv, validace
                    // není blíže specifikována, resp. požadována.
                    //

                    throw new Exception(
                        "Položka Job.ExecUser.Name není vyplněna");
                }
            }
            catch
            {
                // U chybového statusu se již error text neuvádí,
                // pro zalogování postačuje plně zápis FAIL.

                testCase.LogFailResult();
                return false;
            }

            testCase.LogPassResult();
            return true;
        }

        /// <summary>
        /// Věcná kontrola obsahu hodnot položky Job.ExecutiveUser.Description<br/>
        /// (Kdo bude na pohovoru za vedení).<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static bool T31_Check_JobExecDesc()
        {
            var testCase = new TestCase(ref _rs, "Věcná kontrola položky Job.ExecUser.Desc");

            try
            {
                // Formální kontrola na NOT NULL or empty value
                // proběhla v testCase T29_Check_ResponseData().

                // Věcná kontrola položky.
                if (string.IsNullOrEmpty(_jobExecUser.Description)
                )
                {
                    // Otázkou je, co je považováno za vyplněný obsah.
                    // Předpokládám, že se jedná o cokoliv, validace
                    // není blíže specifikována, resp. požadována.
                    //

                    throw new Exception(
                        "Položka Job.ExecUser.Desc není vyplněna");
                }
            }
            catch
            {
                // U chybového statusu se již error text neuvádí,
                // pro zalogování postačuje plně zápis FAIL.

                testCase.LogFailResult();
                return false;
            }

            testCase.LogPassResult();
            return true;
        }

        /// <summary>
        /// Věcná kontrola obsahu hodnot položky Job.ExecutiveUser.Image<br/>
        /// (Kdo bude na pohovoru za vedení).<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static bool T32_Check_JobExecImg()
        {
            var testCase = new TestCase(ref _rs, "Věcná kontrola položky Job.ExecUser.Image");

            try
            {
                // Formální kontrola na NOT NULL or empty value
                // proběhla v testCase T29_Check_ResponseData().

                // Věcná kontrola položky.
                if (string.IsNullOrEmpty(_jobExecUser.Image)
                )
                {
                    // Otázkou je, co je považováno za vyplněný obsah.
                    // Předpokládám, že se jedná o cokoliv, validace
                    // není blíže specifikována, resp. požadována.
                    //

                    throw new Exception(
                        "Položka Job.ExecUser.Image není vyplněna");
                }
            }
            catch
            {
                // U chybového statusu se již error text neuvádí,
                // pro zalogování postačuje plně zápis FAIL.

                testCase.LogFailResult();
                return false;
            }

            testCase.LogPassResult();
            return true;
        }

        /// <summary>
        /// Formální a věcná kontrola položky přijatých dat: Job.GestorUser.<br/>
        /// (Kdo bude na pohovoru za HR).<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static bool T33_Check_JobGestorUrl()
        {
            var testCase = new TestCase(ref _rs, "Formální a věcná kontrola adresy Job.Gestor");

            // Definice obsahu pro věcnou kontrolu.
            string mustBeValue =
                @"https://webapi.alza.cz/api/career/v1/employees/";

            try
            {
                // Formální kontrola položky.
                if (_job.GestorUser is null
                    || _job.GestorUser.Meta is null
                    || _job.GestorUser.Meta.Href is null)
                {
                    throw new Exception(
                        "JSON uzly položky Job.GestorUser jsou ve stavu NULL");
                }

                // Věcná kontrola položky.
                if (!_job.GestorUser.Meta.Href.StartsWith(mustBeValue))
                {
                    throw new Exception(
                        "Položka Job.GestorUser.Meta.Href nemá očekávanou hodnotu");
                }

                // Adresa end-pointu pro zjištění osoby přítomné u pohovoru.
                _apiUriForGestUsr = _job.GestorUser.Meta.Href;
            }
            catch
            {
                // U chybového statusu se již error text neuvádí,
                // pro zalogování postačuje plně zápis FAIL.

                testCase.LogFailResult();
                return false;
            }

            testCase.LogPassResult();
            return true;
        }

        // ----------------------------------------------------------------------
        //  Infrastrukturní testy prostředí
        // ----------------------------------------------------------------------

        /// <summary>
        /// Formální a věcná kontrola vytvoření instance RestRequest pro zjištění osoby
        /// Job.GestorUser přítomné u pohovoru, včetně kontroly některých<br/>
        /// její hodnot.<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static void T34_CreateCheck_Request()
        {
            var testCase = new TestCase(ref _rs, "Formální a věcná kontrola instance RestRequest");
            _request = new RestRequest(_apiUriForGestUsr, Method.GET);

            try
            {
                // Kontrola na formální existenci Request instance.
                if (_request is null)
                {
                    throw new Exception(
                        "Nepovedlo se vytvořit novou instanci RestRequest");
                }

                // Věcná kontrola hodnoty parametru Method.
                if (_request.Method != Method.GET)
                {
                    throw new Exception(
                        "Hodnota RestRequest.GET nemá očekávanou hodnotu");
                }

                // Věcná kontrola hodnot RestRequest.Resource parametru.
                if (!_request.Resource.Contains(_apiUriForGestUsr))
                {
                    throw new Exception(
                        "Hodnota RestRequest.Resource nemá definovaný obsah");
                }
            }
            catch (Exception e)
            {
                testCase.LogFailResult(e.Message);
                throw new Exception(e.Message);
            }

            testCase.LogPassResult();
        }

        /// <summary>
        /// Věcná kontrola přidaných doplňkových parametrů RestRequest<br/>
        /// pro Job.GestorUser.
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static void T35_ModifyCheck_Request()
        {
            var testCase = new TestCase(ref _rs, "Věcná kontrola doplňkových parametrů pro RestRequest");
            _request.AddHeader("Accept", "application/json");
            _request.RequestFormat = DataFormat.Json;

            // Definice obsahu pro věcnou kontrolu RestRequest.Parameters.
            string[] mustBeValues = { "Accept", "application/json" };

            try
            {
                // Věcná kontrola položky.
                if (_request.RequestFormat != DataFormat.Json)
                {
                    throw new Exception(
                        "Hodnota RestRequest.RequestFormat nemá očekávaný obsah");
                }

                // Věcná kontrola položky.
                if (mustBeValues.Length != 2
                    || _request.Parameters.Count < 1
                    || _request.Parameters[0].Name is null
                    || !_request.Parameters[0].Name.Equals(mustBeValues[0])
                    || _request.Parameters[0].Value is null
                    || !_request.Parameters[0].Value.Equals(mustBeValues[1]))
                {
                    // TODO: Dořešit Exceptions a E.Messages, které vzniknou
                    // v rámci vyhodnocování podmínky tak, aby se nelogovala
                    // konkrétní generická chyba, ale níže uvedený obsah.

                    throw new Exception(
                        "Hodnota RestRequest.Parameters nemá očekávaný obsah");
                }
            }
            catch (Exception e)
            {
                testCase.LogFailResult(e.Message);
                throw new Exception(e.Message);
            }

            testCase.LogPassResult();
        }



        /// <summary>
        /// Odeslání requestu, příjem a kontrola odpovědi ze sub end-pointu webové služby,<br/>
        /// defined by Job.GestorUser.Meta.Href, pro Job.GestorUser.<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static void T36_SendReceiveCheck_ReqResp()
        {
            var testCase = new TestCase(ref _rs, "Send/Receive/Kontrola dat na/z target end-pointu");
            _respJobGestUser = _client.Execute<User>(_request);

            try
            {
                // Formální kontrola položky.
                if (_respJobGestUser is null)
                {
                    throw new Exception(
                        "Response je ve stavu NULL, tedy bez vrácených hodnot");
                }

                // Zpracování typu vrácené odpovědi z Rest.WebService.
                _statusCode = _respJobGestUser.StatusCode;

                // Věcná kontrola položky.
                if (_statusCode != HttpStatusCode.OK)
                {
                    throw new Exception(
                        "Http stavový kód odpovědí nemá očekávaný obsah");
                }

                // Věcná kontrola položky.
                if (_respJobGestUser.Content.Length <= 0)
                {
                    throw new Exception(
                        "Položka Content.Length má neočekávanou hodnotu");
                }
            }
            catch (Exception e)
            {
                testCase.LogFailResult(e.Message);
                throw new Exception(e.Message);
            }

            testCase.LogPassResult();
        }

        /// <summary>
        /// Formální kontrola přijatých a rozparsovaných dat pro Job.GestorUser.<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static void T37_Check_ResponseData()
        {
            var testCase = new TestCase(ref _rs, "Formální kontrola rozparsovaných Job.GestUser dat");
            _jobGestUser = _respJobGestUser.Data;        // Data memberField is deserialized.

            try
            {
                // Formální kontrola obsahu přijaté odpovědi.
                if (_jobGestUser is null)
                {
                    throw new Exception(
                        "Položka Job.GestUser.Content.Data je ve stavu NULL");
                }

                // Formální kontrola typu přijaté odpovědi.
                if (_jobGestUser.GetType() != typeof(User))
                {
                    throw new Exception(
                        "Položka Job.GestUser.Content.Data má nesprávný typ dat");
                }

                // Formální kontrola na výskyt rozparsovaných mandatory položek.
                if (string.IsNullOrEmpty(_jobGestUser.Name)
                    || string.IsNullOrEmpty(_jobGestUser.Description)
                    || string.IsNullOrEmpty(_jobGestUser.Image))
                {
                    throw new Exception(
                        "Job.ExecUser.Content.Data nemá platnou obsahovou strukturu");
                }
            }
            catch (Exception e)
            {
                testCase.LogFailResult(e.Message);
                throw new Exception(e.Message);
            }

            testCase.LogPassResult();
        }

        // ----------------------------------------------------------------------
        //  TestCases pro data, které vrátila již samotná webová služba.
        // ----------------------------------------------------------------------

        /// <summary>
        /// Věcná kontrola obsahu hodnot položky Job.GestorUser.Name<br/>
        /// (Kdo bude na pohovoru za HR).<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static bool T38_Check_JobGestName()
        {
            var testCase = new TestCase(ref _rs, "Věcná kontrola položky Job.GestUser.Name");

            try
            {
                // Formální kontrola na NOT NULL or empty value
                // proběhla v testCase T37_Check_ResponseData().

                // Věcná kontrola položky.
                if (string.IsNullOrEmpty(_jobGestUser.Name)
                )
                {
                    // Otázkou je, co je považováno za vyplněný obsah.
                    // Předpokládám, že se jedná o cokoliv, validace
                    // není blíže specifikována, resp. požadována.
                    //

                    throw new Exception(
                        "Položka Job.GestUser.Name není vyplněna");
                }
            }
            catch
            {
                // U chybového statusu se již error text neuvádí,
                // pro zalogování postačuje plně zápis FAIL.

                testCase.LogFailResult();
                return false;
            }

            testCase.LogPassResult();
            return true;
        }

        /// <summary>
        /// Věcná kontrola obsahu hodnot položky Job.GestorUser.Description<br/>
        /// (Kdo bude na pohovoru za HR).<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static bool T39_Check_JobGestDesc()
        {
            var testCase = new TestCase(ref _rs, "Věcná kontrola položky Job.GestUser.Desc");

            try
            {
                // Formální kontrola na NOT NULL or empty value
                // proběhla v testCase T29_Check_ResponseData().

                // Věcná kontrola položky.
                if (string.IsNullOrEmpty(_jobGestUser.Description)
                )
                {
                    // Otázkou je, co je považováno za vyplněný obsah.
                    // Předpokládám, že se jedná o cokoliv, validace
                    // není blíže specifikována, resp. požadována.
                    //

                    throw new Exception(
                        "Položka Job.GestUser.Desc není vyplněna");
                }
            }
            catch
            {
                // U chybového statusu se již error text neuvádí,
                // pro zalogování postačuje plně zápis FAIL.

                testCase.LogFailResult();
                return false;
            }

            testCase.LogPassResult();
            return true;
        }

        /// <summary>
        /// Věcná kontrola obsahu hodnot položky Job.GestorUser.Image<br/>
        /// (Kdo bude na pohovoru za vedení).<br/>
        /// <br/><para>
        /// Called.FROM: <see cref="MainPrgRunner"/>.</para>
        /// </summary>
        private static bool T40_Check_JobGestImg()
        {
            var testCase = new TestCase(ref _rs, "Věcná kontrola položky Job.GestUser.Image");

            try
            {
                // Formální kontrola na NOT NULL or empty value
                // proběhla v testCase T29_Check_ResponseData().

                // Věcná kontrola položky.
                if (string.IsNullOrEmpty(_jobGestUser.Image)
                )
                {
                    // Otázkou je, co je považováno za vyplněný obsah.
                    // Předpokládám, že se jedná o cokoliv, validace
                    // není blíže specifikována, resp. požadována.
                    //

                    throw new Exception(
                        "Položka Job.GestUser.Image není vyplněna");
                }
            }
            catch
            {
                // U chybového statusu se již error text neuvádí,
                // pro zalogování postačuje plně zápis FAIL.

                testCase.LogFailResult();
                return false;
            }

            testCase.LogPassResult();
            return true;
        }

        #endregion
    }
}
