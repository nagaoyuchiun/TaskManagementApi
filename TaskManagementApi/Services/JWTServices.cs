﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagementApi.Models;

namespace TaskManagementApi.Services
{
    public interface JWTBase
    {
        /// <summary>
        ///  產生JWT Token
        /// </summary>
        /// <param name="jWTCliam">Token 資訊聲明內容物件</param>
        /// <param name="secretKey">加密金鑰，用來做加密簽章用</param>
        /// <param name="issur">Token 發行者資訊</param>
        /// <param name="expireMinutes">Token 有效期限(分鐘)</param>
        /// <returns>回應內容物件，內容屬性jwt放置Token字串</returns>
        RunStatus GetJWT(
            JWTCliam jWTCliam,
            string secretKey,
            string issuer,
            int expireMinutes = 30
        );
    }

    public class JWTServices : JWTBase
    {
        private readonly ILogger<JWTServices> log;

        public JWTServices(ILogger<JWTServices> logger)
        {
            log = logger;
        }

        public RunStatus GetJWT(
            JWTCliam jWTCliam,
            string secretKey,
            string issuer,
            int expireMinutes = 30
        )
        {
            RunStatus response = new RunStatus();

            try
            {
                #region Step 1. 取得資訊聲明(claims)集合

                List<Claim> claims = GenCliams(jWTCliam);

                #endregion

                #region  Step 2. 建置資訊聲明(claims)物件實體，依據上面步驟產生Data來做

                ClaimsIdentity userClaimsIdentity = new ClaimsIdentity(claims);

                #endregion

                #region Step 3. 建立Token加密用金鑰

                SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

                #endregion

                #region Step 4. 建立簽章，依據金鑰

                // 使用HmacSha256進行加密
                SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

                #endregion

                #region  Step 5. 建立Token內容實體

                SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
                {
                    Issuer = issuer, // 設置發行者資訊
                    Audience = issuer, // 設置驗證發行者對象，如果需要驗證Token發行者，需要設定此項目
                    NotBefore = DateTime.Now, // 設置可用時間， 預設值就是 DateTime.Now
                    IssuedAt = DateTime.Now, // 設置發行時間，預設值就是 DateTime.Now
                    Subject = userClaimsIdentity, // Token 針對User資訊內容物件
                    Expires = DateTime.Now.AddMinutes(expireMinutes), // 建立Token有效期限
                    SigningCredentials = signingCredentials // Token簽章
                };

                #endregion

                #region Step 6. 產生JWT Token並轉換成字串

                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler(); // 建立一個JWT Token處理容器
                SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);  // 將Token內容實體放入JWT Token處理容器
                string serializeToken = tokenHandler.WriteToken(securityToken); // 最後將JWT Token處理容器序列化，這一個就是最後會需要的Token 字串

                #endregion

                response.isSuccess = true; // 告訴使用此請求一方Token成功產生
                response.jwt = serializeToken; // 放置產生的Token字串
                response.msg = "Done.";
            }
            catch (Exception ex)
            {
                log.LogError($"{ex.Message}\n{ex.StackTrace}");
                response.isSuccess = false;
                response.jwt = string.Empty;
                response.msg = "產生Token 過程發生錯誤.";
            }

            return response;
        }

        // https://auth0.com/docs/tokens/json-web-tokens/json-web-token-claims#custom-claims
        /// <summary>
        /// 建置資訊聲明集合
        /// </summary>
        /// <param name="jWTCliam">Token資訊聲明內容物件</param>
        /// <returns>一組收集資訊聲明集合</returns>
        private List<Claim> GenCliams(JWTCliam jWTCliam)
        {
            List<Claim> claims = new List<Claim>();

            // (audience)
            // 設定Token接受者，用在驗證接收者驗證是否相符
            if (jWTCliam.aud != null && jWTCliam.aud != string.Empty)
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Aud, jWTCliam.aud));
            }

            // (expiration time)
            // Token過期時間，一但超過這時間此Token就失效
            if (jWTCliam.exp != null && jWTCliam.exp != string.Empty)
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Exp, jWTCliam.exp));
            }

            //  (issued at time)
            // Token發行時間，用在後面檢查Token發行多久
            if (jWTCliam.iat != null && jWTCliam.iat != string.Empty)
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Iat, jWTCliam.iat));
            }

            // (issuer)
            // 發行者資訊
            if (jWTCliam.iss != null && jWTCliam.iss != string.Empty)
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Iss, jWTCliam.iss));
            }

            // (JWT ID)
            // Token ID，避免Token重複在被套用
            if (jWTCliam.jti != null && jWTCliam.jti != string.Empty)
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Jti, jWTCliam.jti));
            }

            // (not before time)
            // Token有效起始時間，用來驗證Token可用時間
            if (jWTCliam.nbf != null && jWTCliam.nbf != string.Empty)
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, jWTCliam.nbf));
            }

            // (subject)
            // Token 主題，放置該User內容
            if (jWTCliam.sub != null && jWTCliam.sub != string.Empty)
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, jWTCliam.sub));
            }

            return claims;
        }
    }

    public class RunStatus
    {
        /// <summary>
        /// Token是否有成功產生狀態
        /// </summary>
        /// <value>布林值</value>
        public bool isSuccess { set; get; }

        /// <summary>
        /// Token
        /// </summary>
        /// <value>字串</value>
        public string jwt { set; get; }

        /// <summary>
        /// 處理訊息
        /// </summary>
        /// <value>字串</value>
        public string msg { set; get; }
    }

}
