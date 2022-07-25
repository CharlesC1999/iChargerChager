using System;
using System.Threading.Tasks;
using backend.Models.Power;
using backend.Services;
using backend.util;
using backend.utils;
using backend.ViewModels;
using backend.ViewModels.Power;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace backend.Controllers.Power
{
    [ApiController]
    [Route("api/[controller]")]
    public class PowerController : ControllerBase
    {
        private readonly appSettings _appSettings;
        private readonly IHttpContextAccessor _HttpContextAccessor;
        private readonly ILogger<PowerController> _logger;
        private readonly PowerService _service;
        private string _AccountNumber;
        private readonly string _programname = "充電模組";

        public PowerController(IOptions<appSettings> appSettings, IHttpContextAccessor HttpContextAccessor, PowerService PowerService)
        {
            _appSettings = appSettings.Value;
            _service = PowerService;

            // 解析Token
            _HttpContextAccessor = HttpContextAccessor ??
                throw new ArgumentNullException(nameof(HttpContextAccessor));
            tokenEnCode TokenEnCode = new tokenEnCode(HttpContextAccessor.HttpContext);
            var PayLoad = TokenEnCode.GetPayLoad();
            _AccountNumber = PayLoad is null ? "" : PayLoad["account"].ToString();
        }

        /// <summary>
        /// 使用者掃描QRCODE回傳充電樁資訊
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("QRCode")]
        public IActionResult GetQRCodeByKey([FromQuery] string Key)
        {
            try
            {
                if (this._AccountNumber == "")
                {
                    return Unauthorized(new ResultViewModel<string>
                    {
                        isSuccess = false,
                        message = "登入期限已過期，請重新登入！",
                        Result = null,
                    });
                }

                var Result = _service.GetQRCodeByKey(Key);
                return Ok(new ResultViewModel<ChargerViewModel>
                {
                    isSuccess = true,
                    message = "充電成功",
                    Result = Result,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ResultViewModel<string>
                {
                    isSuccess = false,
                    message = e.Message.ToString(),
                    Result = null,
                });
            }
        }

        /// <summary>
        /// 取得目前使用者充電電量資訊(給SOCKET使用)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Now/Info")]
        public IActionResult GetChargerOrderNowInfo([FromQuery] string ChargerId, [FromQuery] string ChargerGunId)
        {
            try
            {
                if (this._AccountNumber == "")
                {
                    return Unauthorized(new ResultViewModel<string>
                    {
                        isSuccess = false,
                        message = "登入期限已過期，請重新登入！",
                        Result = null,
                    });
                }

                var Result = _service.GetChargerOrderNowInfo(ChargerId, ChargerGunId, _AccountNumber);
                return Ok(new ResultViewModel<ChargerInfoViewModel>
                {
                    isSuccess = true,
                    message = "取得成功",
                    Result = Result,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ResultViewModel<string>
                {
                    isSuccess = false,
                    message = e.Message.ToString(),
                    Result = null,
                });
            }
        }

        /// <summary>
        /// 取得目前使用者訂單狀態
        /// </summary>
        /// <returns>
        /// -1 訂單取消
        /// 0 訂單成立
        /// 1 充電完成(待付款)
        /// 2 訂單完成
        /// </returns>
        [HttpGet]
        [Route("Now")]
        public IActionResult GetChargerOrderNow()
        {
            try
            {
                if (this._AccountNumber == "")
                {
                    return Unauthorized(new ResultViewModel<string>
                    {
                        isSuccess = false,
                        message = "登入期限已過期，請重新登入！",
                        Result = null,
                    });
                }

                return Ok(new ResultViewModel<OrderViewModel>
                {
                    isSuccess = true,
                    message = "充電資訊取得成功",
                    Result = _service.GetChargerOrderNow(_AccountNumber),
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ResultViewModel<string>
                {
                    isSuccess = false,
                    message = e.Message.ToString(),
                    Result = null,
                });
            }
        }

        /// <summary>
        /// 取得使用者歷史訂單
        /// </summary>
        /// <returns>
        /// -1 訂單取消
        /// 0 訂單建立
        /// 1 充電完成(待付款)
        /// 2 訂單完成
        /// </returns>
        [HttpGet]
        public IActionResult GetChargerOrder([FromQuery] OrderGetModel model)
        {
            try
            {
                if (this._AccountNumber == "")
                {
                    return Unauthorized(new ResultViewModel<string>
                    {
                        isSuccess = false,
                        message = "登入期限已過期，請重新登入！",
                        Result = null,
                    });
                }

                return Ok(new ResultViewModel<OrderListViewModel>
                {
                    isSuccess = true,
                    message = "充電資訊取得成功",
                    Result = _service.GetChargerOrder(
                        model.Page,
                        model.PageCount,
                        model.Order,
                        _AccountNumber),
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ResultViewModel<string>
                {
                    isSuccess = false,
                    message = e.Message.ToString(),
                    Result = null,
                });
            }
        }

        /// <summary>
        /// 使用者建立訂單
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> PostChargerOrder([FromBody] PowerPostModel model)
        {
            try
            {
                if (this._AccountNumber == "")
                {
                    return Unauthorized(new ResultViewModel<string>
                    {
                        isSuccess = false,
                        message = "登入期限已過期，請重新登入！",
                        Result = null,
                    });
                }
                // 建立訂單
                bool Result = _service.PostChargerOrder(
                    model,
                    _AccountNumber
                );
                if (!Result)
                {
                    return BadRequest(new ResultViewModel<string>
                    {
                        isSuccess = false,
                        message = "充電錯誤",
                        Result = null,
                    });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new ResultViewModel<string>
                {
                    isSuccess = false,
                    message = e.Message.ToString(),
                    Result = null,
                });
            }

            try
            {
                // 啟動充電槍
                // await _service.PostCharger(model.Key, _AccountNumber);
                return Ok(new ResultViewModel<string>
                {
                    isSuccess = true,
                    message = "充電槍啟動成功",
                    Result = null,
                });
            }
            catch (Exception e)
            {
                _service.CancelChargerOrder(_AccountNumber);
                return BadRequest(new ResultViewModel<string>
                {
                    isSuccess = false,
                    message = "充電槍啟動失敗",
                    Result = null,
                });
            }
        }

        /// <summary>
        /// 使用者結單
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpPost]
        [Route("Finish")]
        public IActionResult PostChargerOrderFinish([FromBody] PowerFinishPostModel model)
        {
            try
            {
                if (this._AccountNumber == "")
                {
                    return Unauthorized(new ResultViewModel<string>
                    {
                        isSuccess = false,
                        message = "登入期限已過期，請重新登入！",
                        Result = null,
                    });
                }
                // 建立訂單
                bool Result = _service.PostChargerOrderFinish(
                    model
                );
                if (!Result)
                {
                    return BadRequest(new ResultViewModel<string>
                    {
                        isSuccess = false,
                        message = "充電錯誤",
                        Result = null,
                    });
                }
                return Ok(new ResultViewModel<string>
                {
                    isSuccess = true,
                    message = "充電槍啟動成功",
                    Result = null,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ResultViewModel<string>
                {
                    isSuccess = false,
                    message = e.Message.ToString(),
                    Result = null,
                });
            }
        }
    }
}