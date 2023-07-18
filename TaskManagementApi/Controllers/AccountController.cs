using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TaskManagementApi.Data;
using TaskManagementApi.Models;
using TaskManagementApi.Services;
using TaskManagementApi.ViewModels;

namespace TaskManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> log;
        private readonly IConfiguration config;
        private readonly JWTConfig jwtConfig;
        private readonly JWTBase jwtHelper;
        private readonly TaskManagementApiContext _context;
        public AccountController(
            ILogger<AccountController> logger,
            IOptions<JWTConfig> jwtOptions,
            IConfiguration configuration,
            JWTBase jWTServices,
            TaskManagementApiContext context
        )
        {
            log = logger;
            jwtConfig = jwtOptions.Value;
            config = configuration;
            jwtHelper = jWTServices;
            _context = context;
        }

        [HttpGet("{id}")]
        public ActionResult<Account> GetAccount(int id)
        {
            if (_context.Account == null)
            {
                return NotFound();
            }
            var account = _context.Account.Find(id);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }

        [HttpPost]
        public ActionResult<Account> CreateAccount(Account account)
        {
            if (_context.Account == null)
            {
                return Problem("Entity set 'WebApiTestContext.Account'  is null.");
            }
            _context.Account.Add(account);
            _context.SaveChanges();

            return CreatedAtAction("GetAccount", new { id = account.Id }, account);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            if (_context.Account == null)
            {
                return NotFound();
            }
            var account = await _context.Account.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            _context.Account.Remove(account);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [Route("signin")]
        [HttpPost]
        [AllowAnonymous]
        public ActionResult<AccountResponse> PostSignIn(string username, string password)
        {
            AccountResponse res = new AccountResponse();
            JWTCliam cliam = new JWTCliam();

            try
            {
                var user = _context.Account.FirstOrDefault(u => u.UserName == username && u.Password == password);

                if (user == null)
                {
                    res.Status = false;
                    res.JwtToken = string.Empty;
                    res.Msg = "帳號密碼驗證失敗.";

                    return res;
                }

                DateTime getExpireDateTime = DateTime.UtcNow.AddMinutes(jwtConfig.ExpireDateTime);

                cliam.iss = jwtConfig.Issuer;
                cliam.sub = user.Id.ToString(); // 放User內容，這裡放自動產生Guid值
                cliam.iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                cliam.nbf = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                cliam.exp = getExpireDateTime.ToString();

                // 執行產生JWT，取得回應結果
                RunStatus getJWTResponse = jwtHelper.GetJWT(
                    cliam,
                    jwtConfig.SignKey,
                    jwtConfig.Issuer,
                    jwtConfig.ExpireDateTime
                );

                switch (getJWTResponse.isSuccess)
                {
                    case true:

                        res.Status = true;
                        res.JwtToken = getJWTResponse.jwt;
                        res.Msg = "Done.";

                        break;

                    case false:

                        res.Status = false;
                        res.JwtToken = string.Empty;
                        res.Msg = "驗證過程發生錯誤.";

                        break;
                }
            }
            catch (Exception ex)
            {
                log.LogError($"{ex.Message}/{ex.StackTrace}");
                res.Status = false;
                res.JwtToken = string.Empty;
                res.Msg = "處理過程發生錯誤.";
            }

            return res;
        }

        //[HttpGet]
        //[Authorize]
        //public ActionResult<AccountResponse> GetTokenInfo()
        //{
        //    AccountResponse res = new AccountResponse();

        //    try
        //    {
        //        var getIssuer = User.Claims.Where(
        //            data => data.Type == "iss"
        //        ).FirstOrDefault();

        //        res.Status = true;
        //        res.Msg = getIssuer.Value.ToString();
        //        res.JwtToken = string.Empty;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.LogError($"{ex.Message}\n{ex.StackTrace}");
        //    }

        //    return res;
        //}
    }
}
