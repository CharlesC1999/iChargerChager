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
        /// 取得目前使用者充電電量資訊(SOCKET)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Now/Info")]
        public IActionResult GetChargerOrderNowInfo([FromQuery] int TransNo)
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

                var Result = _service.GetChargerOrderNowInfo(TransNo, _AccountNumber);
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
        /// 取得目前使用者充電狀態(SOCKET)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Now/Status")]
        public IActionResult GetChargerOrderNowStatus([FromQuery] int TransNo)
        {
            try
            {
                var Result = _service.GetChargerOrderNowStatus(TransNo);
                return Ok(new
                {
                    payload = Result
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
            int OrderId = 0;
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

                // 確認是否有訂單
                var OrderData = _service.GetOrderByKey(model.Key);
                if (OrderData is not null)
                {
                    return BadRequest(new ResultViewModel<string>
                    {
                        isSuccess = false,
                        message = "該充電槍目前已有訂單",
                        Result = null,
                    });
                }

                // 確認車號
                var CarData = _service.GetByCarId(model.CarId);
                if (CarData is null)
                {
                    return NotFound(new ResultViewModel<string>
                    {
                        isSuccess = false,
                        message = "查無此車號",
                        Result = null,
                    });
                }

                // 確認卡片
                var CardData = _service.GetByCardId(model.PayId, _AccountNumber);
                if (CardData is null)
                {
                    return NotFound(new ResultViewModel<string>
                    {
                        isSuccess = false,
                        message = "查無此卡片",
                        Result = null,
                    });
                }

                // 確認發票
                var ReceiveData = _service.GetByReceiveId(model.ReceiveId, _AccountNumber);
                if (ReceiveData is null)
                {
                    return NotFound(new ResultViewModel<string>
                    {
                        isSuccess = false,
                        message = "查無此發票",
                        Result = null,
                    });
                }

                // 建立訂單
                OrderId = _service.PostChargerOrder(
                    model,
                    _AccountNumber
                );
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
                // 傳送訂單建立通知
                await _service.PostNotification(OrderId, _AccountNumber, 0);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Notification Error: 'OrderId = {OrderId}, Account = {_AccountNumber}, Message = {e}'");
            }

            try
            {
                // 啟動充電槍
                // await _service.PostChargerStart(model.Key, _AccountNumber);

                // 改變狀態為目前充電中
                _service.UpdateChargerOrderStatus(OrderId, 1, _AccountNumber);

                // 改變充電樁狀態
                _service.UpdateChargerGunStatus(model.Key, 2);
            }
            catch (Exception e)
            {
                // 改變狀態為目前充電失敗
                _service.CancelChargerOrder(_AccountNumber);
                // 傳送訂單建立通知
                await _service.PostCancelNotification(_AccountNumber);
                return BadRequest(new ResultViewModel<string>
                {
                    isSuccess = false,
                    message = "充電槍啟動失敗",
                    Result = null,
                });
            }

            try
            {
                // 傳送訂單建立通知
                await _service.PostNotification(OrderId, _AccountNumber, 1);
                return Ok(new ResultViewModel<object>
                {
                    isSuccess = true,
                    message = "充電槍啟動成功",
                    Result = new
                    {
                        OrderId = OrderId,
                    },
                });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Notification Error: 'OrderId = {OrderId}, Account = {_AccountNumber}, Message = {e}'");
                return Ok(new ResultViewModel<object>
                {
                    isSuccess = true,
                    message = "充電槍啟動成功",
                    Result = new
                    {
                        OrderId = OrderId,
                    },
                });
            }
        }

        /// <summary>
        /// 使用者開始充電(SOCKET)
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpGet]
        [Route("Start")]
        public IActionResult GetChargerOrderStartSocket([FromQuery] int TransNo)
        {
            try
            {
                // 開始充電訂單
                _service.UpdateChargerOrderStatus(TransNo, 1, _AccountNumber);
                return Ok(new ResultViewModel<string>
                {
                    isSuccess = true,
                    message = "充電成功",
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

        /// <summary>
        /// 使用者錯誤結單(SOCKET)
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpGet]
        [Route("Error")]
        public async Task<IActionResult> GetChargerOrderFinishErrorSocket([FromQuery] int TransNo)
        {
            try
            {
                // 結束訂單
                _service.UpdateChargerOrderStatus(TransNo, -1);
                // 傳送訂單建立通知
                await _service.PostNotification(TransNo, -1);
                // 改變充電樁狀態
                _service.UpdateChargerGunStatus(TransNo, 1);
                return Ok(new ResultViewModel<string>
                {
                    isSuccess = true,
                    message = "結束充電成功",
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

        /// <summary>
        /// 使用者結單(SOCKET)
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpGet]
        [Route("Finish")]
        public async Task<IActionResult> GetChargerOrderFinishSocket([FromQuery] int TransNo)
        {
            try
            {
                // 結束訂單
                _service.UpdateChargerOrderStatus(TransNo, 2);
                // 傳送訂單建立通知
                await _service.PostNotification(TransNo, 2);
                // 改變充電樁狀態
                _service.UpdateChargerGunStatus(TransNo, 1);

                return Ok(new ResultViewModel<string>
                {
                    isSuccess = true,
                    message = "結束充電成功",
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

        /// <summary>
        /// 使用者結單(手動)
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpPost]
        [Route("Finish/Manual")]
        public async Task<IActionResult> PostChargerOrderFinish([FromBody] PowerFinishPostModel model)
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

                var OrderData = _service.GetOrderById(model.TransNo);
                if (OrderData is null)
                {
                    return BadRequest(new ResultViewModel<string>
                    {
                        isSuccess = false,
                        message = "訂單錯誤",
                        Result = null,
                    });
                }
                if (OrderData.status == 2)
                {
                    return BadRequest(new ResultViewModel<string>
                    {
                        isSuccess = false,
                        message = "訂單目前正在結單付款中，請稍候！",
                        Result = null,
                    });
                }

                // 結束充電槍
                // await _service.PostChargerEnd(
                //     new ChargerPostModel
                //     {
                //         station_id = model.ChargerId,
                //         charger_id = model.ChargerGunId,
                //         trans_no = model.TransNo
                //     }
                // );

                return Ok(new ResultViewModel<string>
                {
                    isSuccess = true,
                    message = "結束充電成功",
                    Result = null,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ResultViewModel<string>
                {
                    isSuccess = false,
                    message = "結束充電失敗",
                    Result = null,
                });
            }
        }

        /// <summary>
        /// 使用者開始預約
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpPost]
        [Route("Reserve")]
        public async Task<IActionResult> PostChargerReserve([FromBody] PowerReservePostModel model)
        {
            int OrderId = 0;
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

                // 確認是否有訂單
                var OrderData = _service.GetOrderByGunId(model.ChargerGunId);
                if (OrderData is not null)
                {
                    return BadRequest(new ResultViewModel<string>
                    {
                        isSuccess = false,
                        message = "該充電槍目前已有訂單",
                        Result = null,
                    });
                }

                // 確認槍是否被預約
                var GunData = _service.GetGunById(model.ChargerGunId);
                if (GunData.status == 3)
                {
                    return BadRequest(new ResultViewModel<string>
                    {
                        isSuccess = false,
                        message = "該充電槍目前已被預約",
                        Result = null,
                    });
                }

                // 確認車號
                var CarData = _service.GetByCarId(model.CarId);
                if (CarData is null)
                {
                    return NotFound(new ResultViewModel<string>
                    {
                        isSuccess = false,
                        message = "查無此車號",
                        Result = null,
                    });
                }

                // 確認卡片
                var CardData = _service.GetByCardId(model.PayId, _AccountNumber);
                if (CardData is null)
                {
                    return NotFound(new ResultViewModel<string>
                    {
                        isSuccess = false,
                        message = "查無此卡片",
                        Result = null,
                    });
                }

                // 確認發票
                var ReceiveData = _service.GetByReceiveId(model.ReceiveId, _AccountNumber);
                if (ReceiveData is null)
                {
                    return NotFound(new ResultViewModel<string>
                    {
                        isSuccess = false,
                        message = "查無此發票",
                        Result = null,
                    });
                }

                // 建立預約訂單
                OrderId = _service.PostChargerReserve(
                    model,
                    _AccountNumber
                );
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
                // 傳送訂單建立通知
                await _service.PostReserveNotification(OrderId, _AccountNumber, 0);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Notification Error: 'OrderId = {OrderId}, Account = {_AccountNumber}, Message = {e}'");
            }

            return Ok(new ResultViewModel<object>
            {
                isSuccess = true,
                message = "充電槍預約成功",
                Result = new
                {
                    OrderId = OrderId,
                },
            });
        }

        /// <summary>
        /// 使用者預約結單
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpPost]
        [Route("Reserve/Finish")]
        public async Task<IActionResult> PostChargerReserveFinish([FromBody] PowerFinishReservePostModel model)
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

                var OrderData = _service.GetReserveOrderById(model.OrderId);
                if (OrderData is null)
                {
                    return BadRequest(new ResultViewModel<string>
                    {
                        isSuccess = false,
                        message = "訂單錯誤",
                        Result = null,
                    });
                }
                if (OrderData.status == 1)
                {
                    return BadRequest(new ResultViewModel<string>
                    {
                        isSuccess = false,
                        message = "預約訂單目前正在結單付款中，請稍候！",
                        Result = null,
                    });
                }

                // 結束訂單
                _service.PostChargerReserveEnd(model.OrderId);

                // 計算費用
                var OrderData2 = _service.GetReserveOrderById(model.OrderId);
                _service.PostChargerReserveFinish(model.OrderId, OrderData2.reserve_start, OrderData2.reserve_end);

                // 付款
                await _service.PostReserveOrderFee(model.OrderId);

                return Ok(new ResultViewModel<string>
                {
                    isSuccess = true,
                    message = "結束預約成功",
                    Result = null,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ResultViewModel<string>
                {
                    isSuccess = false,
                    message = "結束預約失敗",
                    Result = null,
                });
            }
        }
    }
}