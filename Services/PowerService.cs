using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using backend.dao;
using backend.Models;
using backend.Models.Power;
using backend.util;
using backend.ViewModels.Power;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;

namespace backend.Services
{
    public class PowerService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly PowerDao _PowerDao;
        private readonly appSettings _appSettings;
        public PowerService(IHttpClientFactory clientFactory, PowerDao PowerDao, IOptions<appSettings> appSettings)
        {
            this._PowerDao = PowerDao;
            this._appSettings = appSettings.Value;
            this._clientFactory = clientFactory;
        }

        public ChargerViewModel GetQRCodeByKey(string Key)
        {
            ChargerViewModel Result = new ChargerViewModel();
            ChargerModel Data = _PowerDao.GetQRCodeByKey(Key);
            Result = new ChargerViewModel
            {
                id = Data.id,
                name = Data.name,
                address = Data.address,
                chargersupplier_name = Data.chargersupplier_name,
                chargerlocation_name = Data.chargerlocation_name,
                chargerlocation_address = Data.chargerlocation_address,
                chargerlocation_geom = JsonConvert.DeserializeObject<GeometryModel>(Data.chargerlocation_geom),
                chargergun_type = Data.chargergun_type,
                chargergun_type_power = Data.chargergun_type_power,
                chargergun_fee = Data.chargergun_fee,
                chargergun_name = Data.chargergun_name,
            };
            return Result;
        }

        public OrderViewModel GetChargerOrderNow(string Account)
        {
            OrderViewModel Result = new OrderViewModel();
            OrderModel Data = _PowerDao.GetChargerOrderNow(Account);
            Result = new OrderViewModel()
            {
                id = Data.id,
                account = Data.account,
                car_id = Data.car_id,
                car = new CarViewModel()
                {
                    id = Data.car_id,
                    plate = Data.car_plate,
                    vehiclestyle_id = Data.car_vehiclestyle_id,
                    vehiclestyle = new CarVehicleStyleViewModel()
                    {
                        id = Data.car_vehiclestyle_id,
                        brand = Data.car_vehiclestyle_brand,
                        model = Data.car_vehiclestyle_model,
                    }
                },
                charger_id = Data.charger_id,
                charger = new ChargerViewModel
                {
                    id = Data.charger_id,
                    name = Data.charger_name,
                    address = Data.charger_address,
                    chargersupplier_name = Data.chargersupplier_name,
                    chargerlocation_name = Data.chargerlocation_name,
                    chargerlocation_address = Data.chargerlocation_address,
                    chargerlocation_geom = JsonConvert.DeserializeObject<GeometryModel>(Data.chargerlocation_geom),
                    chargergun_type = Data.chargergun_type,
                    chargergun_type_power = Data.chargergun_type_power,
                    chargergun_fee = Data.chargergun_fee,
                    chargergun_name = Data.chargergun_name,
                },
                status = Data.status,
                createid = Data.createid,
                createat = Data.createat,
                updateid = Data.updateid,
                updateat = Data.updateat,
            };
            return Result;
        }

        public ChargerInfoViewModel GetChargerOrderNowInfo(int TransNo, string Account)
        {
            ChargerInfoViewModel Result = new ChargerInfoViewModel();
            ChargerInfoModel Data = _PowerDao.GetChargerOrderNowInfo(TransNo, Account);
            Result = Data is null ? null : new ChargerInfoViewModel()
            {
                id = Data.id,
                charger_id = Data.charger_id,
                chargergun_id = Data.chargergun_id,
                charge_time = Data.charge_time,
                charge_current = (float)Math.Round(Data.charge_current, 2),
                charge_voltage = (float)Math.Round(Data.charge_voltage, 2),
                charge_kw = (float)Math.Round(Data.charge_kw, 2),
                current_kw = (float)Math.Round(Data.current_kw, 2),
                soc = Data.soc,
                time = Data.time,
                trans_no = TransNo,
            };
            return Result;
        }

        public ChargerStatusViewModel GetChargerOrderNowStatus(int TransNo)
        {
            ChargerStatusViewModel Result = new ChargerStatusViewModel();
            ChargerStatusModel Data = _PowerDao.GetChargerOrderNowStatus(TransNo);
            Result = Data is null ? null : new ChargerStatusViewModel()
            {
                charger_id = Data.charger_id,
                chargergun_id = Data.chargergun_id,
                trans_no = Data.trans_no,
                vendor_error_code = Data.vendor_error_code,
                status = Data.status,
                time = Data.time.ToString("yyyy-MM-dd HH:mm:ss"),
            };
            return Result;
        }

        public OrderModel GetOrderByKey(string Key)
        {
            OrderModel Result = _PowerDao.GetOrderByKey(Key);
            return Result;
        }

        public ChargerGunModel GetGunById(string GunId)
        {
            ChargerGunModel Result = _PowerDao.GetGunById(GunId);
            return Result;
        }

        public OrderModel GetOrderByGunId(string GunId)
        {
            OrderModel Result = _PowerDao.GetOrderByGunId(GunId);
            return Result;
        }

        public OrderModel GetOrderById(int OrderId)
        {
            OrderModel Result = _PowerDao.GetOrderById(OrderId);
            return Result;
        }

        public ReserveOrderModel GetReserveOrderById(int OrderId)
        {
            ReserveOrderModel Result = _PowerDao.GetReserveOrderById(OrderId);
            return Result;
        }

        public void UpdateChargerGunStatus(int TransNo, int Status)
        {
            _PowerDao.UpdateChargerGunStatus(TransNo, Status);
        }

        public void UpdateChargerGunStatus(string Key, int Status)
        {
            _PowerDao.UpdateChargerGunStatus(Key, Status);
        }

        public int PostChargerOrder(PowerPostModel model, string Account)
        {
            // 新增訂單
            int OrderId = _PowerDao.PostChargerOrder(
                model.PayId,
                model.CarId,
                model.ReceiveId,
                model.Key,
                Account
            );

            //初始化資料
            _PowerDao.PostChargerInfo(
                model.Key,
                OrderId
            );
            return OrderId;
        }

        public bool PostChargerOrderFinish(PowerFinishPostModel model)
        {
            // 新增訂單
            _PowerDao.PostChargerOrderFinish(
                model.ChargerId,
                model.ChargerGunId,
                model.TransNo
            );

            // 測試-新增結單資訊
            // _PowerDao.PostChargerOrderFinishInfo(
            //     model.ChargerId,
            //     model.ChargerGunId,
            //     model.TransNo
            // );
            return true;
        }

        public async Task PostChargerEnd(ChargerPostModel Data)
        {
            if (Data is null) throw new Exception("無法結束充電槍");
            if (Data.trans_no == 0) throw new Exception("無法結束充電槍");

            var url = "https://www.gochabar.com/etgapi/api/ev_stop";
            var client = _clientFactory.CreateClient();
            string authValue = "HDREAPIKEY";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authValue);
            var body = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(Data),
                Encoding.UTF8,
                Application.Json);
            var response = await client.PostAsync(url, body);
            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStringAsync();
                var responseObject = System.Text.Json.JsonSerializer.Deserialize<ChargerResponseViewModel>(responseStream);
                if (responseObject.resdata != "SUCCESS")
                {
                    throw new Exception("無法結束充電槍");
                }
            }
            else
            {
                throw new Exception("無法結束充電槍");
            }
        }

        public async Task PostChargerStart(string Key, string Account)
        {
            ChargerPostModel Data = _PowerDao.GetChargerPostByKey(Key, Account);
            if (Data is null) throw new Exception("無法啟動充電槍");
            if (Data.trans_no == 0) throw new Exception("無法啟動充電槍");

            var url = "https://www.gochabar.com/etgapi/api/ev_start";
            var client = _clientFactory.CreateClient();
            string authValue = "HDREAPIKEY";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authValue);
            var body = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(Data),
                Encoding.UTF8,
                Application.Json);
            var response = await client.PostAsync(url, body);
            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStringAsync();
                var responseObject = System.Text.Json.JsonSerializer.Deserialize<ChargerResponseViewModel>(responseStream);
                if (responseObject.resdata != "SUCCESS")
                {
                    throw new Exception("無法啟動充電槍");
                }
            }
            else
            {
                throw new Exception("無法啟動充電槍");
            }
        }

        public int PostChargerReserve(PowerReservePostModel model, string Account)
        {
            // 新增訂單
            int OrderId = _PowerDao.PostChargerReserve(
                model.PayId,
                model.CarId,
                model.ReceiveId,
                model.ChargerGunId,
                Account
            );
            return OrderId;
        }

        public bool PostChargerReserveEnd(int OrderId)
        {
            // 結束訂單
            _PowerDao.PostChargerReserveEnd(OrderId);
            return true;
        }

        public bool PostChargerReserveFinish(int OrderId, DateTime StartTime, DateTime EndTime)
        {
            var Minutes = 0;
            var Temp = (EndTime - StartTime).TotalMinutes;
            Minutes += (int)Temp;

            // 結束訂單
            _PowerDao.PostChargerReserveFinish(OrderId, Minutes);
            return true;
        }

        public async Task PostCancelNotification(string Account)
        {
            // 取得手機Token
            List<MemberNotifyModel> NotifyData = _PowerDao.GetNotifyTokenByAccount(Account);

            foreach (var item in NotifyData)
            {
                var url = "https://fcm.googleapis.com/fcm/send";
                var client = _clientFactory.CreateClient();
                string authValue = "AAAAudnf1KU:APA91bHYDceYngjMAbf32Tkgecv3uKWfHy7FiQ7QToLSPbDwli3gT1YC0rakLAAA4vJ8KJzdsiotNk7y8Nh25rSTXRZsBwCcgBDuDI4TqcorIpEbpPFihQgU1swucQrlY7AuKyR7cwA5";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + authValue);

                var body = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(
                        new
                        {
                            to = item.token,
                            notification = new
                            {
                                title = "充電失敗",
                                body = @$"很抱歉，因目前充電樁連線異常，系統目前無法幫您進行充電，請再試一次或來電客服將由專人為您服務，若有充電問題歡迎來電客服：0800-868-885，我們將為您服務"
                            }
                        }),
                    Encoding.UTF8,
                    Application.Json);
                var response = await client.PostAsync(url, body);
                if (response.IsSuccessStatusCode)
                {
                    var responseStream = await response.Content.ReadAsStringAsync();
                    var responseObject = System.Text.Json.JsonSerializer.Deserialize<ChargerResponseViewModel>(responseStream);
                }
                else
                {
                    throw new Exception("傳送失敗");
                }
            }
        }

        public async Task PostNotification(int OrderId, string Account, int Status)
        {
            OrderModel Data = _PowerDao.GetChargerOrderNow(Account);
            if (Data is null) throw new Exception("查無此訂單");
            if (Data.account != Account) throw new Exception("查無此訂單");
            if (Data.id != OrderId) throw new Exception("查無此訂單");
            if (Data.status != Status) throw new Exception("訂單狀態錯誤");

            // 取得手機Token
            List<MemberNotifyModel> NotifyData = _PowerDao.GetNotifyTokenByAccount(Account);

            foreach (var item in NotifyData)
            {
                var url = "https://fcm.googleapis.com/fcm/send";
                var client = _clientFactory.CreateClient();
                string authValue = "AAAAudnf1KU:APA91bHYDceYngjMAbf32Tkgecv3uKWfHy7FiQ7QToLSPbDwli3gT1YC0rakLAAA4vJ8KJzdsiotNk7y8Nh25rSTXRZsBwCcgBDuDI4TqcorIpEbpPFihQgU1swucQrlY7AuKyR7cwA5";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + authValue);

                var body = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(
                        Status == 0 ?
                        new
                        {
                            to = item.token,
                            notification = new
                            {
                                title = "準備充電",
                                body = @$"正在準備為您的車輛「{Data.car_plate}」充電，請稍等1-2分鐘，若有充電問題歡迎來電客服：0800-868-885，我們將為您服務"
                            }
                        } : Status == 1 ? new
                        {
                            to = item.token,
                            notification = new
                            {
                                title = "開始充電",
                                body = @$"您的車輛「{Data.car_plate}」已經開始充電，感謝您的使用，若有充電問題歡迎來電客服：0800-868-885，我們將為您服務"
                            }
                        } : Status == 2 ? new
                        {
                            to = item.token,
                            notification = new
                            {
                                title = "結束充電",
                                body = @$"您的車輛「{Data.car_plate}」已經充電結束，本次充電預估充電時間為「{(DateTime.Now - Convert.ToDateTime(Data.createat)).Minutes} 分鐘」，詳細訂單資訊請至「充電記錄查詢」功能瀏覽，感謝您本次的使用，若有充電問題歡迎來電客服：0800-868-885，我們將為您服務"
                            }
                        } : new
                        {
                            to = item.token,
                            notification = new
                            {
                                title = "充電異常",
                                body = @$"您的車輛「{Data.car_plate}」目前發生充電異常，因此系統已幫您自動取消或結束訂單，請您至車輛確認目前充電樁狀態並重新開始充電流程，很抱歉造成您本次的困擾，若發生充電異常等相關問題時歡迎來電客服：0800-868-885，我們將為您盡快解決問題"
                            }
                        }),
                    Encoding.UTF8,
                    Application.Json);
                var response = await client.PostAsync(url, body);
                if (response.IsSuccessStatusCode)
                {
                    var responseStream = await response.Content.ReadAsStringAsync();
                    var responseObject = System.Text.Json.JsonSerializer.Deserialize<ChargerResponseViewModel>(responseStream);
                }
                else
                { }
            }
        }

        public async Task PostNotification(int OrderId, int Status)
        {
            OrderModel Data = _PowerDao.GetChargerOrderById(OrderId.ToString());
            if (Data is null) throw new Exception("查無此訂單");
            if (Data.status != Status) throw new Exception("訂單狀態錯誤");

            // 取得手機Token
            List<MemberNotifyModel> NotifyData = _PowerDao.GetNotifyTokenByAccount(Data.account);

            foreach (var item in NotifyData)
            {
                var url = "https://fcm.googleapis.com/fcm/send";
                var client = _clientFactory.CreateClient();
                string authValue = "AAAAudnf1KU:APA91bHYDceYngjMAbf32Tkgecv3uKWfHy7FiQ7QToLSPbDwli3gT1YC0rakLAAA4vJ8KJzdsiotNk7y8Nh25rSTXRZsBwCcgBDuDI4TqcorIpEbpPFihQgU1swucQrlY7AuKyR7cwA5";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + authValue);

                var body = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(
                        Status == 0 ?
                        new
                        {
                            to = item.token,
                            notification = new
                            {
                                title = "準備充電",
                                body = @$"正在準備為您的車輛「{Data.car_plate}」充電，請稍等1-2分鐘，若有充電問題歡迎來電客服：0800-868-885，我們將為您服務"
                            }
                        } : Status == 1 ? new
                        {
                            to = item.token,
                            notification = new
                            {
                                title = "開始充電",
                                body = @$"您的車輛「{Data.car_plate}」已經開始充電，感謝您的使用，若有充電問題歡迎來電客服：0800-868-885，我們將為您服務"
                            }
                        } : Status == 2 ? new
                        {
                            to = item.token,
                            notification = new
                            {
                                title = "結束充電",
                                body = @$"您的車輛「{Data.car_plate}」已經充電結束，本次充電預估充電時間為「{(DateTime.Now - Convert.ToDateTime(Data.createat)).Minutes} 分鐘」，詳細訂單資訊請至「充電記錄查詢」功能瀏覽，感謝您本次的使用，若有充電問題歡迎來電客服：0800-868-885，我們將為您服務"
                            }
                        } : new
                        {
                            to = item.token,
                            notification = new
                            {
                                title = "充電異常",
                                body = @$"您的車輛「{Data.car_plate}」目前發生充電異常，因此系統已幫您自動取消或結束訂單，請您至車輛確認目前充電樁狀態並重新開始充電流程，很抱歉造成您本次的困擾，若發生充電異常等相關問題時歡迎來電客服：0800-868-885，我們將為您盡快解決問題"
                            }
                        }),
                    Encoding.UTF8,
                    Application.Json);
                var response = await client.PostAsync(url, body);
                if (response.IsSuccessStatusCode)
                {
                    var responseStream = await response.Content.ReadAsStringAsync();
                    var responseObject = System.Text.Json.JsonSerializer.Deserialize<ChargerResponseViewModel>(responseStream);
                }
                else
                {
                    throw new Exception("傳送失敗");
                }
            }
        }

        public async Task PostReserveNotification(int OrderId, string Account, int Status)
        {
            ReserveOrderModel Data = _PowerDao.GetReserveOrderNow(Account);
            if (Data is null) throw new Exception("查無此訂單");
            if (Data.account != Account) throw new Exception("查無此訂單");
            if (Data.id != OrderId) throw new Exception("查無此訂單");
            if (Data.status != Status) throw new Exception("訂單狀態錯誤");

            // 取得手機Token
            List<MemberNotifyModel> NotifyData = _PowerDao.GetNotifyTokenByAccount(Account);

            foreach (var item in NotifyData)
            {
                var url = "https://fcm.googleapis.com/fcm/send";
                var client = _clientFactory.CreateClient();
                string authValue = "AAAAudnf1KU:APA91bHYDceYngjMAbf32Tkgecv3uKWfHy7FiQ7QToLSPbDwli3gT1YC0rakLAAA4vJ8KJzdsiotNk7y8Nh25rSTXRZsBwCcgBDuDI4TqcorIpEbpPFihQgU1swucQrlY7AuKyR7cwA5";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + authValue);

                var body = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(
                        Status == 0 ?
                        new
                        {
                            to = item.token,
                            notification = new
                            {
                                title = "開始預約",
                                body = @$"您的預約已成功，目前可直接入場充電，若有充電問題歡迎來電客服：0800-868-885，我們將為您服務"
                            }
                        } : Status == 1 ? new
                        {
                            to = item.token,
                            notification = new
                            {
                                title = "結束預約",
                                body = @$"您的預約已經結束，本次預約預估時間為「{(DateTime.Now - Convert.ToDateTime(Data.reserve_start)).Minutes} 分鐘」，詳細訂單資訊請至「預估記錄查詢」功能瀏覽，感謝您本次的使用，若有充電問題歡迎來電客服：0800-868-885，我們將為您服務"
                            }
                        } : new
                        {
                            to = item.token,
                            notification = new
                            {
                                title = "預約異常",
                                body = @$"您的預約目前發生異常，因此系統已幫您自動取消或結束訂單，很抱歉造成您本次的困擾，若發生異常等相關問題時歡迎來電客服：0800-868-885，我們將為您盡快解決問題"
                            }
                        }),
                    Encoding.UTF8,
                    Application.Json);
                var response = await client.PostAsync(url, body);
                if (response.IsSuccessStatusCode)
                {
                    var responseStream = await response.Content.ReadAsStringAsync();
                    var responseObject = System.Text.Json.JsonSerializer.Deserialize<ChargerResponseViewModel>(responseStream);
                }
                else
                { }
            }
        }

        public void UpdateChargerOrderStatus(int OrderId, int Status, string Account)
        {
            _PowerDao.UpdateChargerOrderStatus(OrderId, Status, Account);
        }

        public void UpdateChargerOrderStatus(int OrderId, int Status)
        {
            _PowerDao.UpdateChargerOrderStatus(OrderId, Status);
        }

        public void CancelChargerOrder(string Account)
        {
            _PowerDao.CancelChargerOrder(Account);
        }

        public CarViewModel GetByCarId(string CarId)
        {
            CarViewModel Result = _PowerDao.GetCarById(CarId);
            return Result;
        }

        public PayViewModel GetByCardId(string CardId, string Account)
        {
            PayViewModel Result = _PowerDao.GetByCardId(CardId, Account);
            return Result;
        }

        public ReceiveViewModel GetByReceiveId(string ReceiveId, string Account)
        {
            ReceiveViewModel Result = _PowerDao.GetByReceiveId(ReceiveId, Account);
            return Result;
        }

        public async Task PostReserveOrderFee(int OrderId)
        {
            await this.PostReserveFee(OrderId);
        }

        public async Task PostReserveFee(int OrderId)
        {
            var url = "http://192.168.1.164:13010/api/Pay/Reserve";
            var client = _clientFactory.CreateClient();
            var body = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(new
                {
                    OrderId = OrderId
                }),
                Encoding.UTF8,
                Application.Json);
            var response = await client.PostAsync(url, body);
        }
    }
}